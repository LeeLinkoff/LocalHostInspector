namespace LocalHostInspector.Core.Interfaces;


public interface ICommandRunner
{
    Task<string> RunAsync(string executable, string arguments);
}