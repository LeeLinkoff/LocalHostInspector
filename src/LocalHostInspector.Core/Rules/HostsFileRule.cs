using LocalHostInspector.Core.Interfaces;
using LocalHostInspector.Core.Models;


namespace LocalHostInspector.Core.Rules;


public class HostsFileRule : IDiagnosisRule
{
    public string Conclusion => "Hosts file contains a matching entry.";

    public string Evidence => "A matching entry was found in the hosts file.";

    public bool Evaluate(DiagnosticResult result)
    {
        return result.HostsFile.HasMatches;
    }
}