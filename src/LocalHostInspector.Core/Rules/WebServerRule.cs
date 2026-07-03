using LocalHostInspector.Core.Interfaces;
using LocalHostInspector.Core.Models;


namespace LocalHostInspector.Core.Rules;


public class WebServerRule : IDiagnosisRule
{
    public string Conclusion => "A matching web server binding was found.";

    public string Evidence => "The configured web server contains a matching host binding.";

    public bool Evaluate(DiagnosticResult result)
    {
        return result.WebServer.IsMatch;
    }
}