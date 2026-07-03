using LocalHostInspector.Core.Models;


namespace LocalHostInspector.Core.Interfaces;


public interface IWebServerService
{
    Task<WebServerResult> InspectAsync(string host);
}