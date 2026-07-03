using LocalHostInspector.Core.Interfaces;
using LocalHostInspector.Core.Models;

namespace LocalHostInspector.Core.Engine;

public class DiagnosisEngine
{
    private readonly IDnsService _dnsService;
    private readonly IHostsFileService _hostsFileService;
    private readonly IWebServerService _webServerService;
    private readonly IEnumerable<IDiagnosisRule> _rules;

    public DiagnosisEngine(
        IDnsService dnsService,
        IHostsFileService hostsFileService,
        IWebServerService webServerService,
        IEnumerable<IDiagnosisRule> rules)
    {
        _dnsService = dnsService;
        _hostsFileService = hostsFileService;
        _webServerService = webServerService;
        _rules = rules;
    }

    public async Task<DiagnosticResult> InspectAsync(string host)
    {
        var result = new DiagnosticResult
        {
            Host = host
        };

        result.Dns = await _dnsService.InspectAsync(host);
        result.HostsFile = await _hostsFileService.InspectAsync(host);
        result.WebServer = await _webServerService.InspectAsync(host);

        foreach (var rule in _rules)
        {
            if (!rule.Evaluate(result))
                continue;

            result.Conclusion = rule.Conclusion;
            result.Evidence.Add(rule.Evidence);

            break;
        }

        return result;
    }
}