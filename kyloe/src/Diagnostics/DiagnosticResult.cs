using System.Linq;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.IO;
using System;
using Kyloe.Utility;
using System.Collections;

namespace Kyloe.Diagnostics
{

    public sealed class DiagnosticResult : IEnumerable<Diagnostic>
    {
        private readonly SourceText sourceText;
        private ImmutableArray<Diagnostic> diagnostics;

        internal DiagnosticResult(SourceText sourceText, ImmutableArray<Diagnostic> diagnostics)
        {
            this.sourceText = sourceText;
            this.diagnostics = diagnostics;
        }

        public IEnumerable<Diagnostic> GetAll() => diagnostics;
        public IEnumerable<Diagnostic> GetErrors() => diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error);

        public void WriteTo(TextWriter writer)
        {
            var colorMode = DiagnosticWriter.ColorMode.None;

            if (object.ReferenceEquals(writer, Console.Out))
                colorMode = DiagnosticWriter.ColorMode.ConsoleColor;

            var diagnosticWriter = new DiagnosticWriter(writer, sourceText, colorMode);
            diagnosticWriter.Write(this);
        }

        public IEnumerable<Diagnostic> GetWarnings() => diagnostics.Where(d => d.Severity == DiagnosticSeverity.Warn);

        public bool HasDiagnostics() => diagnostics.Count() != 0;
        public bool HasErrors() => GetErrors().Count() != 0;
        public bool HasWarnings() => GetWarnings().Count() != 0;

        public IEnumerator<Diagnostic> GetEnumerator()
        {
            return ((IEnumerable<Diagnostic>)diagnostics).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)diagnostics).GetEnumerator();
        }
    }

}