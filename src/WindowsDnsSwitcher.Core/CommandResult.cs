namespace WindowsDnsSwitcher.Core;

public sealed record CommandResult(bool Succeeded, int ExitCode, string Output, string Error)
{
    public string CombinedOutput => string.Join(
        Environment.NewLine,
        new[] { Output, Error }.Where(text => !string.IsNullOrWhiteSpace(text)));
}


