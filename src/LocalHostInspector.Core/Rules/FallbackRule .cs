using LocalHostInspector.Core.Interfaces;
using LocalHostInspector.Core.Models;


namespace LocalHostInspector.Core.Rules;


public class FallbackRule : IDiagnosisRule
{
    public string Conclusion => "No definitive cause could be determined.";

    public string Evidence => "The available diagnostic evidence did not match any known rule. Review the collected DNS, hosts file, and web server information to continue troubleshooting.";

    public bool Evaluate(DiagnosticResult result)
    {
        // This rule should always be registered LAST.
        return true;
    }
}