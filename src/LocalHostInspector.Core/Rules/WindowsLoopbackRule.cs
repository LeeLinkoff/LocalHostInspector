using LocalHostInspector.Core.Interfaces;
using LocalHostInspector.
    Core.Models;

namespace LocalHostInspector.Core.Rules;


public class WindowsLoopbackRule : IDiagnosisRule
{
    public bool Evaluate(DiagnosticResult result)
    {
        return (string.Equals(result.Host, "localhost", StringComparison.OrdinalIgnoreCase))
                &&
               (result.HostsFile.Matches.Count == 0)
                &&
               (!result.WebServer.IsMatch);
    }

    public string Conclusion => "The hostname resolved using the Windows networking stack.";

    public string Evidence => "The requested hostname is 'localhost'. No matching hosts file entry was found and no IIS site binding matched the hostname. Windows reserves 'localhost' as a loopback hostname and resolves it to the local computer without requiring a hosts file entry.";
}