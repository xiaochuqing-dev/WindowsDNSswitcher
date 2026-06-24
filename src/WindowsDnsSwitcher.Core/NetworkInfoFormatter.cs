namespace WindowsDnsSwitcher.Core;

public static class NetworkInfoFormatter
{
    public static string FormatIpv6Status(AdapterInfo? adapter)
    {
        return FormatIpv6Status(adapter, AppLanguage.Chinese);
    }

    public static string FormatIpv6Status(AdapterInfo? adapter, AppLanguage language)
    {
        if (adapter is null)
        {
            return language == AppLanguage.English ? "IPv6: detection failed" : "IPv6：检测失败";
        }

        if (language == AppLanguage.English)
        {
            return adapter.HasIpv6Address ? "IPv6: enabled" : "IPv6: disabled";
        }

        return adapter.HasIpv6Address ? "IPv6：已启用" : "IPv6：未启用";
    }

    public static string FormatAdapterInfo(AdapterInfo adapter)
    {
        return FormatAdapterInfo(adapter, AppLanguage.Chinese);
    }

    public static string FormatAdapterInfo(AdapterInfo adapter, AppLanguage language)
    {
        if (language == AppLanguage.English)
        {
            return string.Join(Environment.NewLine, new[]
            {
                $"Adapter name: {adapter.Name}",
                $"Description: {ValueOrDash(adapter.Description)}",
                $"Adapter type: {adapter.Type}",
                $"IPv4 address: {JoinOrDash(adapter.Ipv4Addresses)}",
                $"Default gateway: {JoinOrDash(adapter.Gateways)}",
                $"Current DNS servers: {JoinOrDash(adapter.DnsServers)}",
                $"IPv6 enabled: {(adapter.HasIpv6Address ? "yes" : "no")}",
                $"IPv6 address present: {(adapter.HasIpv6Address ? "yes" : "no")}",
            });
        }

        return string.Join(Environment.NewLine, new[]
        {
            $"网卡名称：{adapter.Name}",
            $"网卡描述：{ValueOrDash(adapter.Description)}",
            $"网卡类型：{adapter.Type}",
            $"IPv4 地址：{JoinOrDash(adapter.Ipv4Addresses)}",
            $"默认网关：{JoinOrDash(adapter.Gateways)}",
            $"当前 DNS 服务器：{JoinOrDash(adapter.DnsServers)}",
            $"IPv6 是否启用：{(adapter.HasIpv6Address ? "是" : "否")}",
            $"是否存在 IPv6 地址：{(adapter.HasIpv6Address ? "是" : "否")}",
        });
    }

    public static string FormatMode(DnsMode mode)
    {
        return FormatMode(mode, AppLanguage.Chinese);
    }

    public static string FormatMode(DnsMode mode, AppLanguage language)
    {
        if (language == AppLanguage.English)
        {
            return mode switch
            {
                DnsMode.AutomaticDns => "Current mode: Automatic DNS Mode",
                DnsMode.CustomDns => "Current mode: Custom DNS Mode",
                DnsMode.OtherDns => "Current mode: Other DNS",
                _ => "Current mode: detection failed",
            };
        }

        return mode switch
        {
            DnsMode.AutomaticDns => "当前模式：自动 DNS 模式",
            DnsMode.CustomDns => "当前模式：自定义 DNS 模式",
            DnsMode.OtherDns => "当前模式：其他 DNS",
            _ => "当前模式：检测失败",
        };
    }

    private static string JoinOrDash(IReadOnlyList<string> values)
    {
        return values.Count == 0 ? "-" : string.Join(", ", values);
    }

    private static string ValueOrDash(string value)
    {
        return string.IsNullOrWhiteSpace(value) ? "-" : value;
    }
}


