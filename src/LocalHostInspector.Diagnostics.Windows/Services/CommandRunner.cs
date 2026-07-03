using System.Diagnostics;
using LocalHostInspector.Core.Interfaces;


namespace LocalHostInspector.Diagnostics.Windows.Services;


public class CommandRunner : ICommandRunner
{
    public async Task<string> RunAsync(string executable, string arguments)
    {
        using var process = new Process();

        process.StartInfo = new ProcessStartInfo
        {
            FileName = executable,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        process.Start();

        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        return output + error;
    }
}