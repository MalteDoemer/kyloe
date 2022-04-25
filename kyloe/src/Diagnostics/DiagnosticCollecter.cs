using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal sealed class DiagnosticCollector : ICollection<Diagnostic>
    {
        private readonly ImmutableArray<Diagnostic>.Builder diagnostics;
        private readonly SourceText sourceText;

        public DiagnosticCollector(SourceText sourceText)
        {
            diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();
            this.sourceText = sourceText;
        }

        public int Count => diagnostics.Count;

        public bool IsReadOnly => ((ICollection<Diagnostic>)diagnostics).IsReadOnly;

        public void Add(Diagnostic item)
        {
            diagnostics.Add(item);
        }

        public void AddRange(IEnumerable<Diagnostic> items)
        {
            diagnostics.AddRange(items);
        }

        public void Clear()
        {
            diagnostics.Clear();
        }

        public bool Contains(Diagnostic item)
        {
            return diagnostics.Contains(item);
        }

        public void CopyTo(Diagnostic[] array, int arrayIndex)
        {
            diagnostics.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Diagnostic> GetEnumerator()
        {
            return ((IEnumerable<Diagnostic>)diagnostics).GetEnumerator();
        }

        public bool Remove(Diagnostic item)
        {
            return diagnostics.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)diagnostics).GetEnumerator();
        }

        public DiagnosticResult ToResult()
        {
            return new DiagnosticResult(sourceText, diagnostics.ToImmutable());
        }
    }
}