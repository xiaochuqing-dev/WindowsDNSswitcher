using System.Net;
using System.Net.Sockets;

namespace WindowsDnsSwitcher.Core;

public sealed record CustomDnsSettings(string PrimaryDns, string SecondaryDns)
{
    public static CustomDnsSettings Empty { get; } = new(string.Empty, string.Empty);

    public bool HasPrimaryDns => !string.IsNullOrWhiteSpace(PrimaryDns);

    public CustomDnsSettings Normalize()
    {
        return new CustomDnsSettings(PrimaryDns.Trim(), SecondaryDns.Trim());
    }
}

public enum CustomDnsValidationStatus
{
    Valid,
    MissingPrimary,
    InvalidAddress,
}

public static class CustomDnsValidator
{
    public static CustomDnsValidationStatus Validate(string? primaryDns, string? secondaryDns)
    {
        var primary = primaryDns?.Trim() ?? string.Empty;
        var secondary = secondaryDns?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(primary))
        {
            return CustomDnsValidationStatus.MissingPrimary;
        }

        if (!IsValidIpv4(primary))
        {
            return CustomDnsValidationStatus.InvalidAddress;
        }

        if (!string.IsNullOrWhiteSpace(secondary) && !IsValidIpv4(secondary))
        {
            return CustomDnsValidationStatus.InvalidAddress;
        }

        return CustomDnsValidationStatus.Valid;
    }

    public static bool IsValid(CustomDnsSettings settings)
    {
        var normalized = settings.Normalize();
        return Validate(normalized.PrimaryDns, normalized.SecondaryDns) == CustomDnsValidationStatus.Valid;
    }

    private static bool IsValidIpv4(string value)
    {
        return IPAddress.TryParse(value, out var address)
            && address.AddressFamily == AddressFamily.InterNetwork
            && value.Count(character => character == '.') == 3;
    }
}


