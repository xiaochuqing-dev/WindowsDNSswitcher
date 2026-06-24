namespace WindowsDnsSwitcher.Core;

public static class DnsCommandFactory
{
    public static IEnumerable<CommandSpec> CreateAutomaticDnsModeCommands(string adapterName)
    {
        yield return new CommandSpec(
            "netsh",
            ["interface", "ipv4", "set", "dnsservers", $"name={adapterName}", "source=dhcp"]);

        yield return CreateFlushDnsCommand();
    }

    public static IEnumerable<CommandSpec> CreateCustomDnsModeCommands(string adapterName, CustomDnsSettings settings)
    {
        var normalized = settings.Normalize();

        yield return new CommandSpec(
            "netsh",
            ["interface", "ipv4", "set", "dnsservers", $"name={adapterName}", "static", normalized.PrimaryDns, "primary"]);

        if (!string.IsNullOrWhiteSpace(normalized.SecondaryDns))
        {
            yield return new CommandSpec(
                "netsh",
                ["interface", "ipv4", "add", "dnsservers", $"name={adapterName}", normalized.SecondaryDns, "index=2"]);
        }

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
}


