using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace WindowsDnsSwitcher.Core;

public static class NetworkAdapterReader
{
    public static IReadOnlyList<AdapterInfo> GetAdapters()
    {
        return NetworkInterface.GetAllNetworkInterfaces()
            .Select(CreateAdapterInfo)
            .ToArray();
    }

    private static AdapterInfo CreateAdapterInfo(NetworkInterface networkInterface)
    {
        var properties = networkInterface.GetIPProperties();

        var ipv4Addresses = properties.UnicastAddresses
            .Where(address => address.Address.AddressFamily == AddressFamily.InterNetwork)
            .Select(address => address.Address.ToString())
            .ToArray();

        var hasIpv6Address = properties.UnicastAddresses
            .Any(address => address.Address.AddressFamily == AddressFamily.InterNetworkV6);

        var gateways = properties.GatewayAddresses
            .Where(gateway => gateway.Address.AddressFamily == AddressFamily.InterNetwork)
            .Select(gateway => gateway.Address.ToString())
            .ToArray();

        var dnsServers = properties.DnsAddresses
            .Where(address => address.AddressFamily == AddressFamily.InterNetwork)
            .Select(address => address.ToString())
            .ToArray();

        return new AdapterInfo(
            networkInterface.Name,
            networkInterface.Description,
            networkInterface.NetworkInterfaceType,
            networkInterface.OperationalStatus,
            ipv4Addresses,
            gateways,
            dnsServers,
            hasIpv6Address);
    }
}


