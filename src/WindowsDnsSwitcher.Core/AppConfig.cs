using System.Text.Json;
using System.Text.Json.Serialization;

namespace WindowsDnsSwitcher.Core;

public sealed class AppConfig
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public AppConfig()
        : this(null, string.Empty, string.Empty, false, AppLanguageSettings.DefaultConfigValue)
    {
    }

    public AppConfig(string? selectedAdapter)
        : this(selectedAdapter, string.Empty, string.Empty, false, AppLanguageSettings.DefaultConfigValue)
    {
    }

    [JsonConstructor]
    public AppConfig(string? selectedAdapter, string? primaryDns, string? secondaryDns, bool showAllAdapters, string? language = null)
    {
        SelectedAdapter = string.IsNullOrWhiteSpace(selectedAdapter) ? null : selectedAdapter;
        PrimaryDns = primaryDns?.Trim() ?? string.Empty;
        SecondaryDns = secondaryDns?.Trim() ?? string.Empty;
        ShowAllAdapters = showAllAdapters;
        AppLanguage = AppLanguageSettings.Parse(language);
        Language = AppLanguageSettings.ToConfigValue(AppLanguage);
    }

    [JsonPropertyName("selectedAdapter")]
    public string? SelectedAdapter { get; init; }

    [JsonPropertyName("primaryDns")]
    public string PrimaryDns { get; init; } = string.Empty;

    [JsonPropertyName("secondaryDns")]
    public string SecondaryDns { get; init; } = string.Empty;

    [JsonPropertyName("showAllAdapters")]
    public bool ShowAllAdapters { get; init; }

    [JsonPropertyName("language")]
    public string Language { get; init; } = AppLanguageSettings.DefaultConfigValue;

    [JsonIgnore]
    public AppLanguage AppLanguage { get; init; } = AppLanguage.Chinese;

    [JsonPropertyName("lastAdapterName")]
    public string? LegacyLastAdapterName { get; init; }

    [JsonIgnore]
    public string? LastAdapterName => SelectedAdapter ?? LegacyLastAdapterName;

    [JsonIgnore]
    public CustomDnsSettings CustomDnsSettings => new(PrimaryDns, SecondaryDns);

    public AppConfig WithSelectedAdapter(string? selectedAdapter)
    {
        return new AppConfig(selectedAdapter, PrimaryDns, SecondaryDns, ShowAllAdapters, Language);
    }

    public AppConfig WithCustomDns(CustomDnsSettings settings)
    {
        var normalized = settings.Normalize();
        return new AppConfig(LastAdapterName, normalized.PrimaryDns, normalized.SecondaryDns, ShowAllAdapters, Language);
    }

    public AppConfig WithShowAllAdapters(bool showAllAdapters)
    {
        return new AppConfig(LastAdapterName, PrimaryDns, SecondaryDns, showAllAdapters, Language);
    }

    public AppConfig WithLanguage(AppLanguage language)
    {
        return new AppConfig(LastAdapterName, PrimaryDns, SecondaryDns, ShowAllAdapters, AppLanguageSettings.ToConfigValue(language));
    }

    public static AppConfig Load(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                return new AppConfig();
            }

            var json = File.ReadAllText(path);
            var loaded = JsonSerializer.Deserialize<AppConfig>(json, JsonOptions) ?? new AppConfig();
            return new AppConfig(loaded.LastAdapterName, loaded.PrimaryDns, loaded.SecondaryDns, loaded.ShowAllAdapters, loaded.Language);
        }
        catch
        {
            return new AppConfig();
        }
    }

    public static void Save(string path, AppConfig config)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(path, JsonSerializer.Serialize(config, JsonOptions));
    }
}


