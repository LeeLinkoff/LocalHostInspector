using LocalHostInspector.Core.Models;


namespace LocalHostInspector.Core.Interfaces;


public interface IHostsFileService
{
    Task<HostsFileResult> InspectAsync(string host);
}