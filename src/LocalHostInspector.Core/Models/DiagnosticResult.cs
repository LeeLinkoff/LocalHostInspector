namespace LocalHostInspector.Core.Models;


public class DiagnosticResult
{
    public string Host { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public string Conclusion { get; set; } = string.Empty;

    public List<string> Evidence { get; set; } = new();

    public DnsResult Dns { get; set; } = new();

    public HostsFileResult HostsFile { get; set; } = new();

    public WebServerResult WebServer { get; set; } = new();
}