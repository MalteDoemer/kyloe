using System.Linq;
using System.Collections.Immutable;
using System.Collections.Generic;

namespace Kyloe.Diagnostics
{

    public class DiagnosticResult
    {
        private ImmutableArray<Diagnostic> diagnostics;

        internal DiagnosticResult(ImmutableArray<Diagnostic> diagnostics)
        {
            this.diagnostics = diagnostics;
        }

        public IEnumerable<Diagnostic> GetAll() => diagnostics;
        public IEnumerable<Diagnostic> GetErrors() => diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error);
        public IEnumerable<Diagnostic> GetWarnings() => diagnostics.Where(d => d.Severity == DiagnosticSeverity.Warn);

        public bool HasDiagnostics() => diagnostics.Count() != 0;
        public bool HasErrors() => GetErrors().Count() != 0;
        public bool HasWarnings() => GetWarnings().Count() != 0;
    }

}