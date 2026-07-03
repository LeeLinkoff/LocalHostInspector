namespace LocalHostInspector.Core.Models;


public class DnsResult
{
    /// <summary>
    /// System DNS lookup (default resolver).
    /// </summary>
    public string SystemLookup { get; set; } = string.Empty;

    /// <summary>
    /// Google Public DNS lookup.
    /// </summary>
    public string GoogleLookup { get; set; } = string.Empty;

    /// <summary>
    /// Cloudflare Public DNS lookup.
    /// </summary>
    public string CloudflareLookup { get; set; } = string.Empty;

    /// <summary>
    /// Authoritative name server lookup.
    /// </summary>
    public string AuthoritativeLookup { get; set; } = string.Empty;

    /// <summary>
    /// IP address returned by the system-configured DNS resolver.
    /// </summary>
    public string? SystemAddress { get; set; }

    /// <summary>
    /// IP address returned by Google Public DNS.
    /// </summary>
    public string? GoogleAddress { get; set; }

    /// <summary>
    /// IP address returned by Cloudflare Public DNS.
    /// </summary>
    public string? CloudflareAddress { get; set; }

    /// <summary>
    /// IP address returned by the authoritative name server.
    /// </summary>
    public string? AuthoritativeAddress { get; set; }

    /// <summary>
    /// Authoritative DNS provider hosting the DNS zone.
    /// </summary>
    public AuthoritativeDnsProvider? AuthoritativeProvider { get; set; }

    /// <summary>
    /// Authoritative name servers for the DNS zone.
    /// </summary>
    public List<string> AuthoritativeNameServers { get; set; } = new();
}