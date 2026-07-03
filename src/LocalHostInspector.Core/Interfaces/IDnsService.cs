using LocalHostInspector.Core.Models;


namespace LocalHostInspector.Core.Interfaces;


public interface IDnsService
{
    Task<DnsResult> InspectAsync(string host);
}