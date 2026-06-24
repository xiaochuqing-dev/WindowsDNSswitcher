namespace WindowsDnsSwitcher.Core;

public static class NetworkInfoFormatter
{
    public static string FormatIpv6Status(AdapterInfo? adapter)
    {
        if (adapter is null)
        {
            return "IPv6：检测失败";
        }

        return adapter.HasIpv6Address ? "IPv6：已启用" : "IPv6：未启用";
    }

    public static string FormatAdapterInfo(AdapterInfo adapter)
    {
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


