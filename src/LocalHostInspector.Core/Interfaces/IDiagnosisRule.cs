using LocalHostInspector.Core.Models;


namespace LocalHostInspector.Core.Interfaces;


public interface IDiagnosisRule
{
    bool Evaluate(DiagnosticResult result);

    string Conclusion { get; }

    string Evidence { get; }
}