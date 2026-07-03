using LocalHostInspector.Core.Interfaces;
using LocalHostInspector.Core.Models;


namespace LocalHostInspector.Core.Rules;


public sealed class PublicDnsLoopbackRule : IDiagnosisRule
{
    public string Conclusion => "Hostname resolves to localhost via public DNS.";

    public string Evidence => "The domain's public DNS A record resolves to 127.0.0.1 (localhost). The A record is managed by the authoritative DNS provider shown below.";

    public bool Evaluate(DiagnosticResult result)
    {
        return !result.HostsFile.HasMatches
            && !result.WebServer.IsMatch
            && result.Dns.GoogleAddress == "127.0.0.1"
            && result.Dns.CloudflareAddress == "127.0.0.1";
    }
}