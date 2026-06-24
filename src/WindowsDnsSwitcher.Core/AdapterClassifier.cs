using System.Net.NetworkInformation;

namespace WindowsDnsSwitcher.Core;

public static class AdapterClassifier
{
    private static readonly string[] WirelessMarkers =
    [
        "wlan",
        "wi-fi",
        "wifi",
        "wireless",
        "802.11",
    ];

    private static readonly string[] VirtualMarkers =
    [
        "singbox_tun",
        "sing-tun tunnel",
        "xray_tun",
        "wintun",
        "tap",
        "vethernet",
        "vmware",
        "virtualbox",
        "hyper-v",
        "bluetooth",
        "wi-fi direct",
        "wifi direct",
        "loopback",
    ];

    private static readonly string[] BindingComponentMarkers =
    [
        "native wifi filter driver",
        "native wi-fi filter driver",
        "virtual wifi filter driver",
        "virtual wi-fi filter driver",
        "qos packet scheduler",
        "lightweight filter",
        "filter driver",
        "wan miniport",
        "-wfp",
        "wfp ",
    ];

    private static readonly string[] ProxyTunMarkers =
    [
        "singbox_tun",
        "sing-tun tunnel",
        "xray_tun",
    ];

    public static IEnumerable<AdapterInfo> GetDisplayAdapters(
        IEnumerable<AdapterInfo> adapters,
        bool showAllAdapters,
        string? lastAdapterName)
    {
        var ncpaLevelAdapters = adapters.Where(adapter => !IsBindingComponent(adapter));
        var candidates = showAllAdapters
            ? ncpaLevelAdapters
            : ncpaLevelAdapters.Where(adapter => !IsLikelyVirtual(adapter));

        return SortAdapters(candidates, lastAdapterName);
    }

    public static AdapterInfo? SelectDefaultAdapter(
        IEnumerable<AdapterInfo> adapters,
        bool showAllAdapters,
        string? lastAdapterName)
    {
        return GetDisplayAdapters(adapters, showAllAdapters, lastAdapterName).FirstOrDefault();
    }

    public static bool IsLikelyWireless(AdapterInfo adapter)
    {
        return adapter.Type == NetworkInterfaceType.Wireless80211
            || ContainsAnyMarker(adapter, WirelessMarkers);
    }

    public static bool IsLikelyVirtual(AdapterInfo adapter)
    {
        return adapter.Type is NetworkInterfaceType.Loopback or NetworkInterfaceType.Tunnel
            || ContainsAnyMarker(adapter, VirtualMarkers);
    }

    public static bool IsProxyTunAdapter(AdapterInfo adapter)
    {
        return ContainsAnyMarker(adapter, ProxyTunMarkers);
    }

    private static bool IsBindingComponent(AdapterInfo adapter)
    {
        return ContainsAnyMarker(adapter, BindingComponentMarkers);
    }

    private static IEnumerable<AdapterInfo> SortAdapters(
        IEnumerable<AdapterInfo> adapters,
        string? lastAdapterName)
    {
        return adapters
            .OrderByDescending(adapter => IsLastSelected(adapter, lastAdapterName))
            .ThenByDescending(adapter => IsLikelyWireless(adapter))
            .ThenByDescending(adapter => adapter.Status == OperationalStatus.Up)
            .ThenBy(adapter => IsLikelyVirtual(adapter))
            .ThenBy(adapter => adapter.Name, StringComparer.CurrentCultureIgnoreCase);
    }

    private static bool IsLastSelected(AdapterInfo adapter, string? lastAdapterName)
    {
        return !string.IsNullOrWhiteSpace(lastAdapterName)
            && string.Equals(adapter.Name, lastAdapterName, StringComparison.OrdinalIgnoreCase);
    }

    private static bool ContainsAnyMarker(AdapterInfo adapter, IEnumerable<string> markers)
    {
        var text = $"{adapter.Name} {adapter.Description}".ToLowerInvariant();
        return markers.Any(text.Contains);
    }
}


