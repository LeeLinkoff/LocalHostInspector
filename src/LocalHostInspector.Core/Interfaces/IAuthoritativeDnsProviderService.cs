using LocalHostInspector.Core.Models;

namespace LocalHostInspector.Core.Interfaces;


public interface IAuthoritativeDnsProviderService
{
    Task<AuthoritativeDnsProvider?> GetProviderAsync(IEnumerable<string> nameServers);
}