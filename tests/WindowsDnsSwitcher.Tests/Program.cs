using System.Net.NetworkInformation;
using WindowsDnsSwitcher.Core;

var tests = new (string Name, Action Body)[]
{
    ("Ranks real wireless adapters before virtual adapters", RanksRealWirelessAdaptersBeforeVirtualAdapters),
    ("Flags high-risk virtual adapters strongly", FlagsHighRiskVirtualAdaptersStrongly),
    ("Filters binding components from adapter dropdown", FiltersBindingComponentsFromAdapterDropdown),
    ("Show all adapters still excludes binding components", ShowAllAdaptersStillExcludesBindingComponents),
    ("Honors saved adapter selection when available", HonorsSavedAdapterSelectionWhenAvailable),
    ("Parses automatic DNS mode from netsh output", ParsesAutomaticDnsModeFromNetshOutput),
    ("Parses custom DNS mode from saved settings", ParsesCustomDnsModeFromSavedSettings),
    ("Parses other static DNS mode from netsh output", ParsesOtherStaticDnsModeFromNetshOutput),
    ("Does not treat Google DNS as custom without saved settings", DoesNotTreatGoogleDnsAsCustomWithoutSavedSettings),
    ("Validates custom DNS settings", ValidatesCustomDnsSettings),
    ("Builds automatic DNS mode commands with safe argument arrays", BuildsAutomaticDnsModeCommandsWithSafeArgumentArrays),
    ("Builds custom DNS mode commands with primary only", BuildsCustomDnsModeCommandsWithPrimaryOnly),
    ("Builds custom DNS mode commands with secondary", BuildsCustomDnsModeCommandsWithSecondary),
    ("Requires confirmation before switching DNS modes", RequiresConfirmationBeforeSwitchingDnsModes),
    ("Saves and loads config", SavesAndLoadsConfig),
    ("Saves and loads language preference", SavesAndLoadsLanguagePreference),
    ("Loads legacy adapter config", LoadsLegacyAdapterConfig),
    ("Falls back when config JSON is invalid", FallsBackWhenConfigJsonIsInvalid),
    ("Formats English UI text", FormatsEnglishUiText),
    ("Formats public mode names", FormatsPublicModeNames),
    ("Formats adapter information and IPv6 status", FormatsAdapterInformationAndIpv6Status),
};

var failed = 0;

foreach (var test in tests)
{
    try
    {
        test.Body();
        Console.WriteLine($"PASS {test.Name}");
    }
    catch (Exception ex)
    {
        failed++;
        Console.WriteLine($"FAIL {test.Name}: {ex.Message}");
    }
}

if (failed > 0)
{
    Console.WriteLine($"{failed} test(s) failed.");
    return 1;
}

Console.WriteLine($"All {tests.Length} tests passed.");
return 0;

void RanksRealWirelessAdaptersBeforeVirtualAdapters()
{
    var adapters = new[]
    {
        new AdapterInfo("sample_tun", "Sample Tunnel Adapter", NetworkInterfaceType.Tunnel, OperationalStatus.Up, [], [], [], true),
        new AdapterInfo("WLAN", "Intel(R) Wi-Fi 6 AX201 160MHz", NetworkInterfaceType.Wireless80211, OperationalStatus.Up, ["192.168.1.12"], ["192.168.1.1"], ["8.8.8.8"], true),
    };

    var visible = AdapterClassifier.GetDisplayAdapters(adapters, showAllAdapters: false, lastAdapterName: null).ToArray();

    AssertEqual(1, visible.Length);
    AssertEqual("WLAN", visible[0].Name);
}

void FlagsHighRiskVirtualAdaptersStrongly()
{
    var adapter = new AdapterInfo("sample_tun", "Sample Tunnel Adapter", NetworkInterfaceType.Tunnel, OperationalStatus.Up, [], [], ["172.19.0.2"], false);

    AssertTrue(AdapterClassifier.IsLikelyVirtual(adapter), "sample tunnel adapter should be virtual");
    AssertTrue(AdapterClassifier.IsHighRiskVirtualAdapter(adapter), "sample tunnel adapter should be strongly flagged");
}

void FiltersBindingComponentsFromAdapterDropdown()
{
    var adapters = new[]
    {
        new AdapterInfo("WLAN", "Intel(R) Wi-Fi 6 AX200 160MHz", NetworkInterfaceType.Wireless80211, OperationalStatus.Up, ["192.168.1.12"], ["192.168.1.1"], [], true),
        new AdapterInfo("WLAN-Native WiFi Filter Driver-0000", "Intel(R) Wi-Fi 6 AX200 160MHz-Native WiFi Filter Driver", NetworkInterfaceType.Wireless80211, OperationalStatus.Up, [], [], [], false),
        new AdapterInfo("WLAN-QoS Packet Scheduler-0000", "Intel(R) Wi-Fi 6 AX200 160MHz-QoS Packet Scheduler", NetworkInterfaceType.Wireless80211, OperationalStatus.Up, [], [], [], false),
        new AdapterInfo("WLAN-WFP Native MAC Layer Lightweight Filter-0000", "Intel(R) Wi-Fi 6 AX200 160MHz-WFP Native MAC Layer Lightweight Filter", NetworkInterfaceType.Wireless80211, OperationalStatus.Up, [], [], [], false),
        new AdapterInfo("本地连接* 10", "WAN Miniport (Network Monitor)", NetworkInterfaceType.Ethernet, OperationalStatus.Down, [], [], [], false),
    };

    var visible = AdapterClassifier.GetDisplayAdapters(adapters, showAllAdapters: false, lastAdapterName: null).ToArray();

    AssertEqual(1, visible.Length);
    AssertEqual("WLAN", visible[0].Name);
}

void ShowAllAdaptersStillExcludesBindingComponents()
{
    var adapters = new[]
    {
        new AdapterInfo("WLAN", "Intel(R) Wi-Fi 6 AX200 160MHz", NetworkInterfaceType.Wireless80211, OperationalStatus.Up, [], [], [], true),
        new AdapterInfo("sample_tun", "Sample Tunnel Adapter", NetworkInterfaceType.Tunnel, OperationalStatus.Up, [], [], [], false),
        new AdapterInfo("WLAN-Virtual WiFi Filter Driver-0000", "Intel(R) Wi-Fi 6 AX200 160MHz-Virtual WiFi Filter Driver", NetworkInterfaceType.Wireless80211, OperationalStatus.Up, [], [], [], false),
    };

    var visible = AdapterClassifier.GetDisplayAdapters(adapters, showAllAdapters: true, lastAdapterName: null)
        .Select(adapter => adapter.Name)
        .ToArray();

    AssertSequence(["WLAN", "sample_tun"], visible);
}

void HonorsSavedAdapterSelectionWhenAvailable()
{
    var adapters = new[]
    {
        new AdapterInfo("WLAN", "Intel(R) Wi-Fi 6", NetworkInterfaceType.Wireless80211, OperationalStatus.Up, ["192.168.1.10"], ["192.168.1.1"], [], true),
        new AdapterInfo("Wi-Fi 2", "USB Wireless", NetworkInterfaceType.Wireless80211, OperationalStatus.Up, ["192.168.2.10"], ["192.168.2.1"], [], true),
    };

    var selected = AdapterClassifier.SelectDefaultAdapter(adapters, showAllAdapters: false, lastAdapterName: "Wi-Fi 2");

    AssertEqual("Wi-Fi 2", selected?.Name);
}

void ParsesAutomaticDnsModeFromNetshOutput()
{
    var output = """
    Configuration for interface "WLAN"
        DNS servers configured through DHCP: 192.168.1.1
        Register with which suffix: Primary only
    """;

    AssertEqual(DnsMode.AutomaticDns, DnsModeParser.Parse(showCommandSucceeded: true, output));
}

void ParsesCustomDnsModeFromSavedSettings()
{
    var output = """
    Configuration for interface "WLAN"
        Statically Configured DNS Servers: 9.9.9.9
                                          149.112.112.112
    """;

    AssertEqual(DnsMode.CustomDns, DnsModeParser.Parse(showCommandSucceeded: true, output, new CustomDnsSettings("9.9.9.9", "149.112.112.112")));

    var primaryOnlyOutput = """
    Configuration for interface "WLAN"
        Statically Configured DNS Servers: 9.9.9.9
    """;

    AssertEqual(DnsMode.CustomDns, DnsModeParser.Parse(showCommandSucceeded: true, primaryOnlyOutput, new CustomDnsSettings("9.9.9.9", "")));
}

void ParsesOtherStaticDnsModeFromNetshOutput()
{
    var output = """
    Configuration for interface "WLAN"
        Statically Configured DNS Servers: 114.114.114.114
                                          223.5.5.5
    """;

    AssertEqual(DnsMode.OtherDns, DnsModeParser.Parse(showCommandSucceeded: true, output));
    AssertEqual(DnsMode.DetectionFailed, DnsModeParser.Parse(showCommandSucceeded: false, output));
}

void DoesNotTreatGoogleDnsAsCustomWithoutSavedSettings()
{
    var output = """
    Configuration for interface "WLAN"
        Statically Configured DNS Servers: 8.8.8.8
                                          1.1.1.1
    """;

    AssertEqual(DnsMode.OtherDns, DnsModeParser.Parse(showCommandSucceeded: true, output, CustomDnsSettings.Empty));
}

void ValidatesCustomDnsSettings()
{
    AssertEqual(CustomDnsValidationStatus.MissingPrimary, CustomDnsValidator.Validate("", ""));
    AssertEqual(CustomDnsValidationStatus.InvalidAddress, CustomDnsValidator.Validate("not dns", ""));
    AssertEqual(CustomDnsValidationStatus.InvalidAddress, CustomDnsValidator.Validate("9.9.9.9", "2001:4860:4860::8888"));
    AssertEqual(CustomDnsValidationStatus.Valid, CustomDnsValidator.Validate("9.9.9.9", ""));
    AssertEqual(CustomDnsValidationStatus.Valid, CustomDnsValidator.Validate("9.9.9.9", "149.112.112.112"));
}

void BuildsAutomaticDnsModeCommandsWithSafeArgumentArrays()
{
    var commands = DnsCommandFactory.CreateAutomaticDnsModeCommands("WLAN 中文").ToArray();

    AssertEqual(2, commands.Length);
    AssertEqual("netsh", commands[0].FileName);
    AssertSequence(
        ["interface", "ipv4", "set", "dnsservers", "name=WLAN 中文", "source=dhcp"],
        commands[0].Arguments);
    AssertEqual("ipconfig", commands[1].FileName);
    AssertSequence(["/flushdns"], commands[1].Arguments);
}

void BuildsCustomDnsModeCommandsWithPrimaryOnly()
{
    var commands = DnsCommandFactory.CreateCustomDnsModeCommands("Wi-Fi 2", new CustomDnsSettings("9.9.9.9", "")).ToArray();

    AssertEqual(2, commands.Length);
    AssertSequence(
        ["interface", "ipv4", "set", "dnsservers", "name=Wi-Fi 2", "static", "9.9.9.9", "primary"],
        commands[0].Arguments);
    AssertSequence(["/flushdns"], commands[1].Arguments);
}

void BuildsCustomDnsModeCommandsWithSecondary()
{
    var commands = DnsCommandFactory.CreateCustomDnsModeCommands("Wi-Fi 2", new CustomDnsSettings("9.9.9.9", "149.112.112.112")).ToArray();

    AssertEqual(3, commands.Length);
    AssertSequence(
        ["interface", "ipv4", "set", "dnsservers", "name=Wi-Fi 2", "static", "9.9.9.9", "primary"],
        commands[0].Arguments);
    AssertSequence(
        ["interface", "ipv4", "add", "dnsservers", "name=Wi-Fi 2", "149.112.112.112", "index=2"],
        commands[1].Arguments);
    AssertSequence(["/flushdns"], commands[2].Arguments);
}

void RequiresConfirmationBeforeSwitchingDnsModes()
{
    var automatic = ModeSwitchConfirmation.ForMode(DnsMode.AutomaticDns, "WLAN");
    var custom = ModeSwitchConfirmation.ForMode(DnsMode.CustomDns, "WLAN");

    AssertEqual("确认切换 DNS 模式", automatic.Title);
    AssertContains("自动 DNS 模式", automatic.Message);
    AssertContains("WLAN", automatic.Message);
    AssertContains("IPv4 DNS", automatic.Message);
    AssertEqual("确认切换 DNS 模式", custom.Title);
    AssertContains("自定义 DNS 模式", custom.Message);
    AssertContains("WLAN", custom.Message);
    AssertContains("IPv4 DNS", custom.Message);
}

void SavesAndLoadsConfig()
{
    var path = Path.Combine(Path.GetTempPath(), $"windows-dns-switcher-{Guid.NewGuid():N}.json");

    try
    {
        AppConfig.Save(path, new AppConfig("WLAN 中文", "9.9.9.9", "149.112.112.112", true));
        var loaded = AppConfig.Load(path);

        AssertEqual("WLAN 中文", loaded.LastAdapterName);
        AssertEqual("WLAN 中文", loaded.SelectedAdapter);
        AssertEqual("9.9.9.9", loaded.PrimaryDns);
        AssertEqual("149.112.112.112", loaded.SecondaryDns);
        AssertEqual(true, loaded.ShowAllAdapters);
    }
    finally
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}

void SavesAndLoadsLanguagePreference()
{
    var path = Path.Combine(Path.GetTempPath(), $"windows-dns-switcher-{Guid.NewGuid():N}.json");

    try
    {
        AppConfig.Save(path, new AppConfig("WLAN", "9.9.9.9", "", false, "en"));
        var loaded = AppConfig.Load(path);

        AssertEqual("en", loaded.Language);
        AssertEqual(AppLanguage.English, loaded.AppLanguage);
    }
    finally
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}

void LoadsLegacyAdapterConfig()
{
    var path = Path.Combine(Path.GetTempPath(), $"windows-dns-switcher-{Guid.NewGuid():N}.json");

    try
    {
        File.WriteAllText(path, """
        {
          "lastAdapterName": "WLAN"
        }
        """);

        var loaded = AppConfig.Load(path);

        AssertEqual("WLAN", loaded.LastAdapterName);
        AssertEqual("WLAN", loaded.SelectedAdapter);
        AssertEqual("", loaded.PrimaryDns);
        AssertEqual("", loaded.SecondaryDns);
        AssertEqual(false, loaded.ShowAllAdapters);
    }
    finally
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}

void FallsBackWhenConfigJsonIsInvalid()
{
    var path = Path.Combine(Path.GetTempPath(), $"windows-dns-switcher-{Guid.NewGuid():N}.json");

    try
    {
        File.WriteAllText(path, "{not json");
        var loaded = AppConfig.Load(path);

        AssertEqual(null, loaded.LastAdapterName);
    }
    finally
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}

void FormatsEnglishUiText()
{
    AssertEqual("Windows DNS Mode Switcher", UiText.Title(AppLanguage.English));
    AssertEqual("Current mode: Automatic DNS Mode", NetworkInfoFormatter.FormatMode(DnsMode.AutomaticDns, AppLanguage.English));

    var confirmation = ModeSwitchConfirmation.ForMode(DnsMode.CustomDns, "WLAN", AppLanguage.English);

    AssertEqual("Confirm DNS Mode Switch", confirmation.Title);
    AssertContains("Custom DNS Mode", confirmation.Message);
    AssertContains("WLAN", confirmation.Message);
    AssertContains("IPv4 DNS", confirmation.Message);
}

void FormatsPublicModeNames()
{
    AssertEqual("当前模式：自动 DNS 模式", NetworkInfoFormatter.FormatMode(DnsMode.AutomaticDns));
    AssertEqual("当前模式：自定义 DNS 模式", NetworkInfoFormatter.FormatMode(DnsMode.CustomDns));
    AssertEqual("当前模式：其他 DNS", NetworkInfoFormatter.FormatMode(DnsMode.OtherDns));
    AssertEqual("当前模式：检测失败", NetworkInfoFormatter.FormatMode(DnsMode.DetectionFailed));
}

void FormatsAdapterInformationAndIpv6Status()
{
    var adapter = new AdapterInfo(
        "WLAN",
        "Intel(R) Wi-Fi 6",
        NetworkInterfaceType.Wireless80211,
        OperationalStatus.Up,
        ["192.168.1.20"],
        ["192.168.1.1"],
        ["8.8.8.8", "1.1.1.1"],
        true);

    var info = NetworkInfoFormatter.FormatAdapterInfo(adapter);

    AssertEqual("IPv6：已启用", NetworkInfoFormatter.FormatIpv6Status(adapter));
    AssertContains("网卡名称：WLAN", info);
    AssertContains("IPv4 地址：192.168.1.20", info);
    AssertContains("当前 DNS 服务器：8.8.8.8, 1.1.1.1", info);
}

void AssertEqual<T>(T expected, T actual)
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
    {
        throw new InvalidOperationException($"Expected '{expected}', got '{actual}'.");
    }
}

void AssertTrue(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}

void AssertContains(string expectedPart, string actual)
{
    if (!actual.Contains(expectedPart, StringComparison.Ordinal))
    {
        throw new InvalidOperationException($"Expected text to contain '{expectedPart}', got '{actual}'.");
    }
}

void AssertSequence<T>(IReadOnlyList<T> expected, IReadOnlyList<T> actual)
{
    AssertEqual(expected.Count, actual.Count);

    for (var index = 0; index < expected.Count; index++)
    {
        AssertEqual(expected[index], actual[index]);
    }
}


