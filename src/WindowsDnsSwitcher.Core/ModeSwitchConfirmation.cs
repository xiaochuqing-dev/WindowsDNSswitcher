namespace WindowsDnsSwitcher.Core;

public sealed record ModeSwitchConfirmation(string Title, string Message)
{
    public static ModeSwitchConfirmation ForMode(DnsMode targetMode, string adapterName)
    {
        var modeName = targetMode switch
        {
            DnsMode.AutomaticDns => "自动 DNS 模式",
            DnsMode.CustomDns => "自定义 DNS 模式",
            _ => throw new ArgumentOutOfRangeException(nameof(targetMode), targetMode, "Unsupported target mode."),
        };

        return new ModeSwitchConfirmation(
            "确认切换 DNS 模式",
            $"即将把网卡“{adapterName}”切换到{modeName}，这会修改该网卡的 IPv4 DNS 设置。\r\n\r\n确定要继续吗？");
    }
}


