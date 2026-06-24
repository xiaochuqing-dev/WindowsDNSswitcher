namespace WindowsDnsSwitcher.Core;

public sealed record ModeSwitchConfirmation(string Title, string Message)
{
    public static ModeSwitchConfirmation ForMode(DnsMode targetMode, string adapterName)
    {
        return ForMode(targetMode, adapterName, AppLanguage.Chinese);
    }

    public static ModeSwitchConfirmation ForMode(DnsMode targetMode, string adapterName, AppLanguage language)
    {
        var modeName = targetMode switch
        {
            DnsMode.AutomaticDns => language == AppLanguage.English ? "Automatic DNS Mode" : "自动 DNS 模式",
            DnsMode.CustomDns => language == AppLanguage.English ? "Custom DNS Mode" : "自定义 DNS 模式",
            _ => throw new ArgumentOutOfRangeException(nameof(targetMode), targetMode, "Unsupported target mode."),
        };

        if (language == AppLanguage.English)
        {
            return new ModeSwitchConfirmation(
                "Confirm DNS Mode Switch",
                $"The adapter \"{adapterName}\" will be switched to {modeName}. This will modify the adapter's IPv4 DNS settings.\r\n\r\nContinue?");
        }

        return new ModeSwitchConfirmation(
            "确认切换 DNS 模式",
            $"即将把网卡“{adapterName}”切换到{modeName}，这会修改该网卡的 IPv4 DNS 设置。\r\n\r\n确定要继续吗？");
    }
}


