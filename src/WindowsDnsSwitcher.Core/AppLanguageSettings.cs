namespace WindowsDnsSwitcher.Core;

public static class AppLanguageSettings
{
    public const string DefaultConfigValue = "zh-CN";

    public static AppLanguage Parse(string? value)
    {
        return value?.Trim().ToLowerInvariant() switch
        {
            "en" or "en-us" or "english" => AppLanguage.English,
            _ => AppLanguage.Chinese,
        };
    }

    public static string ToConfigValue(AppLanguage language)
    {
        return language == AppLanguage.English ? "en" : DefaultConfigValue;
    }
}

