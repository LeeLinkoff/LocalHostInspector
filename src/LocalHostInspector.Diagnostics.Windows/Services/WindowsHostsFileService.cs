using LocalHostInspector.Core.Interfaces;
using LocalHostInspector.Core.Models;


namespace LocalHostInspector.Diagnostics.Windows.Services;


public class WindowsHostsFileService : IHostsFileService
{
    private const string HostsFile = @"C:\Windows\System32\drivers\etc\hosts";

    public async Task<HostsFileResult> InspectAsync(string host)
    {
        var result = new HostsFileResult
        {
            FilePath = HostsFile
        };

        if (!File.Exists(HostsFile))
            return result;

        var lines = await File.ReadAllLinesAsync(HostsFile);

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];

            // Remove comments.
            var entry = line.Split('#')[0].Trim();

            if (string.IsNullOrWhiteSpace(entry))
                continue;

            // Split into IP + hostnames.
            var tokens = entry.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            if (tokens.Length < 2)
                continue;

            // Compare only hostnames, not comments.
            for (int j = 1; j < tokens.Length; j++)
            {
                if (!string.Equals(tokens[j], host, StringComparison.OrdinalIgnoreCase))
                    continue;

                result.Matches.Add(line);

                if (result.LineNumber == null)
                    result.LineNumber = i + 1;

                break;
            }
        }

        return result;
    }
}