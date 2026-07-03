namespace LocalHostInspector.Core.Models;


public class WebServerResult
{
    /// <summary>
    /// True if the host was handled by this web server.
    /// </summary>
    public bool IsMatch { get; set; }

    /// <summary>
    /// IIS, Apache, Nginx, Kestrel, Kubernetes Ingress, etc.
    /// </summary>
    public string ServerType { get; set; } = string.Empty;

    /// <summary>
    /// Website, VirtualHost, Ingress, etc.
    /// </summary>
    public string SiteName { get; set; } = string.Empty;

    /// <summary>
    /// The matching binding, host header, or virtual host.
    /// </summary>
    public string Binding { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable evidence explaining the match.
    /// </summary>
    public List<string> Evidence { get; set; } = [];

    /// <summary>
    /// Raw diagnostic output for display.
    /// </summary>
    public string RawOutput { get; set; } = string.Empty;
}