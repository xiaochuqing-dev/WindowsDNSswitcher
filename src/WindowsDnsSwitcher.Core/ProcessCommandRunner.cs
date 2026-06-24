using System.Diagnostics;
using System.Text;

namespace WindowsDnsSwitcher.Core;

public sealed class ProcessCommandRunner
{
    public async Task<CommandResult> RunAsync(CommandSpec command, CancellationToken cancellationToken = default)
    {
        var startInfo = new ProcessStartInfo(command.FileName)
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.Default,
            StandardErrorEncoding = Encoding.Default,
        };

        foreach (var argument in command.Arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var process = Process.Start(startInfo);
        if (process is null)
        {
            return new CommandResult(false, -1, string.Empty, "无法启动命令。");
        }

        var outputTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var errorTask = process.StandardError.ReadToEndAsync(cancellationToken);

        await process.WaitForExitAsync(cancellationToken);

        var output = await outputTask;
        var error = await errorTask;

        return new CommandResult(process.ExitCode == 0, process.ExitCode, output.Trim(), error.Trim());
    }
}


