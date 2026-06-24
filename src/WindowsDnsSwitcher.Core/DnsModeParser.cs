using System.Net;
using System.Text.RegularExpressions;

namespace WindowsDnsSwitcher.Core;

public static partial class DnsModeParser
{
    private static readonly string[] DhcpMarkers =
    [
        "dhcp",
        "自动",
    ];

    public static DnsMode Parse(bool showCommandSucceeded, string output)
    {
        return Parse(showCommandSucceeded, output, CustomDnsSettings.Empty);
    }

    public static DnsMode Parse(bool showCommandSucceeded, string output, CustomDnsSettings customDnsSettings)
    {
        if (!showCommandSucceeded || string.IsNullOrWhiteSpace(output))
        {
            return DnsMode.DetectionFailed;
        }

        if (DhcpMarkers.Any(marker => output.Contains(marker, StringComparison.OrdinalIgnoreCase)))
        {
            return DnsMode.AutomaticDns;
        }

        var dnsServers = ExtractIpv4Addresses(output).ToArray();
        if (dnsServers.Length == 0)
        {
            return DnsMode.DetectionFailed;
        }

        return IsConfiguredCustomDns(dnsServers, customDnsSettings)
            ? DnsMode.CustomDns
            : DnsMode.OtherDns;
    }

    private static bool IsConfiguredCustomDns(IReadOnlyList<string> dnsServers, CustomDnsSettings settings)
    {
        var normalized = settings.Normalize();
        if (!CustomDnsValidator.IsValid(normalized))
        {
            return false;
        }

        var expected = string.IsNullOrWhiteSpace(normalized.SecondaryDns)
            ? [normalized.PrimaryDns]
            : new[] { normalized.PrimaryDns, normalized.SecondaryDns };

        return dnsServers.SequenceEqual(expected, StringComparer.OrdinalIgnoreCase);
    }

    private static IEnumerable<string> ExtractIpv4Addresses(string output)
    {
        foreach (Match match in Ipv4Regex().Matches(output))
        {
            var value = match.Value;
            if (IPAddress.TryParse(value, out var parsed) && parsed.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                yield return value;
            }
        }
    }

    [GeneratedRegex(@"\b(?:(?:25[0-5]|2[0-4]\d|1?\d?\d)\.){3}(?:25[0-5]|2[0-4]\d|1?\d?\d)\b")]
    private static partial Regex Ipv4Regex();
}


