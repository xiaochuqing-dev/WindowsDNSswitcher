using System.Net.NetworkInformation;

namespace WindowsDnsSwitcher.Core;

public sealed record AdapterInfo(
    string Name,
    string Description,
    NetworkInterfaceType Type,
    OperationalStatus Status,
    IReadOnlyList<string> Ipv4Addresses,
    IReadOnlyList<string> Gateways,
    IReadOnlyList<string> DnsServers,
    bool HasIpv6Address)
{
    public string DisplayName => string.IsNullOrWhiteSpace(Description)
        ? Name
        : $"{Name} - {Description}";
}


