using System.Linq;
using System.Collections.Generic;

namespace Kyloe.Diagnostics
{
    public class DiagnosticCollecter
    {
        private readonly List<Diagnostic> diagnostics;

        public DiagnosticCollecter()
        {
            diagnostics = new List<Diagnostic>();
        }

        public void Add(Diagnostic diagnostic)
        {
            this.diagnostics.Add(diagnostic);
        }

        public IEnumerable<Diagnostic> GetAll() => diagnostics;
        public IEnumerable<Diagnostic> GetErrors() => diagnostics.FindAll(d => d.Type == DiagnosticType.Error);
        public IEnumerable<Diagnostic> GetWarnings() => diagnostics.FindAll(d => d.Type == DiagnosticType.Warn);

        public bool HasDiagnostics() => diagnostics.Count() != 0;
        public bool HasErrors() => GetErrors().Count() != 0;
        public bool HasWarnings() => GetWarnings().Count() != 0;
    }
}