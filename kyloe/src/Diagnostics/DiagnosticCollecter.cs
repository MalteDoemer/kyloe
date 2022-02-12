using System.Collections.Immutable;

namespace Kyloe.Diagnostics
{
    internal class DiagnosticCollector
    {
        private readonly ImmutableArray<Diagnostic>.Builder diagnostics;

        public DiagnosticCollector()
        {
            diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();
        }

        public void Add(Diagnostic diagnostic)
        {
            diagnostics.Add(diagnostic);
        }

        public DiagnosticResult ToResult()
        {
            return new DiagnosticResult(diagnostics.ToImmutable());
        }
    }
}