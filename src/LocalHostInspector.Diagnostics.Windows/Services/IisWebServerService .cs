using LocalHostInspector.Core.Interfaces;
using LocalHostInspector.Core.Models;


namespace LocalHostInspector.Diagnostics.Windows.Services;


public class IisWebServerService : IWebServerService
{
    public Task<WebServerResult> InspectAsync(string host)
    {
        return Task.FromResult(new WebServerResult{IsMatch = false, ServerType = "IIS"});
    }
}