using LocalHostInspector.Core.Interfaces;
using LocalHostInspector.Core.Models;


namespace LocalHostInspector.Core.Rules;


public class PublicDnsRule : IDiagnosisRule
{
    public string Conclusion => "Public DNS does not resolve this host.";

    public string Evidence => "Both Google Public DNS and Cloudflare Public DNS reported that the hostname does not exist on the public Internet (NXDOMAIN). This indicates the hostname is not publicly resolvable and the request is likely being resolved by a local or private configuration.";

    public bool Evaluate(DiagnosticResult result)
    {
        return result.Dns.GoogleLookup.Contains("Non-existent domain", StringComparison.OrdinalIgnoreCase)
               &&
               result.Dns.CloudflareLookup.Contains("Non-existent domain", StringComparison.OrdinalIgnoreCase);
    }
}