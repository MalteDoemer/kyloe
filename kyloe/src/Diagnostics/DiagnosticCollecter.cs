using System.Collections.Generic;
using System.Collections.Immutable;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal class DiagnosticCollector
    {
        private readonly ImmutableArray<Diagnostic>.Builder diagnostics;
        private readonly SourceText sourceText;

        public DiagnosticCollector(SourceText sourceText)
        {
            diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();
            this.sourceText = sourceText;
        }

        public void Add(Diagnostic diagnostic)
        {
            diagnostics.Add(diagnostic);
        }

        public void AddRange(IEnumerable<Diagnostic> range)
        {
            diagnostics.AddRange(range);
        }

        public DiagnosticResult ToResult()
        {
            return new DiagnosticResult(sourceText, diagnostics.ToImmutable());
        }
    }
}