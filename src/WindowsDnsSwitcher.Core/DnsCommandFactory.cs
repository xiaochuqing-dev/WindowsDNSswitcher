namespace WindowsDnsSwitcher.Core;

public static class DnsCommandFactory
{
    private const string PowerShell = "powershell.exe";

    public static IEnumerable<CommandSpec> CreateAutomaticDnsModeCommands(string adapterName)
    {
        var escapedAdapterName = ToPowerShellSingleQuotedString(adapterName);

        yield return new CommandSpec(
            PowerShell,
            [
                "-NoProfile",
                "-NonInteractive",
                "-Command",
                $"Set-DnsClientServerAddress -InterfaceAlias {escapedAdapterName} -ResetServerAddresses",
            ]);

        yield return CreateFlushDnsCommand();
    }

    public static IEnumerable<CommandSpec> CreateCustomDnsModeCommands(string adapterName, CustomDnsSettings settings)
    {
        var normalized = settings.Normalize();
        var escapedAdapterName = ToPowerShellSingleQuotedString(adapterName);
        var dnsServers = string.IsNullOrWhiteSpace(normalized.SecondaryDns)
            ? ToPowerShellSingleQuotedString(normalized.PrimaryDns)
            : $"{ToPowerShellSingleQuotedString(normalized.PrimaryDns)}, {ToPowerShellSingleQuotedString(normalized.SecondaryDns)}";

        yield return new CommandSpec(
            PowerShell,
            [
                "-NoProfile",
                "-NonInteractive",
                "-Command",
                $"Set-DnsClientServerAddress -InterfaceAlias {escapedAdapterName} -ServerAddresses @({dnsServers})",
            ]);

        yield return CreateFlushDnsCommand();
    }

    public static CommandSpec CreateFlushDnsCommand()
    {
        return new CommandSpec("ipconfig", ["/flushdns"]);
    }

    public static CommandSpec CreateShowDnsServersCommand(string adapterName)
    {
        return new CommandSpec(
            "netsh",
            ["interface", "ipv4", "show", "dnsservers", $"name={adapterName}"]);
    }

    private static string ToPowerShellSingleQuotedString(string value)
    {
        return $"'{value.Replace("'", "''")}'";
    }
}


