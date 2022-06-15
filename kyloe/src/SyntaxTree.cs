using System.Collections.Immutable;
using System.IO;

using Kyloe.Syntax;
using Kyloe.Utility;
using Kyloe.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace Kyloe
{
    public class SyntaxTree
    {
        private readonly SyntaxToken root;
        private readonly IEnumerable<SourceText> sources;
        private readonly DiagnosticResult diagnostics;

        private SyntaxTree(SyntaxToken root, IEnumerable<SourceText> sources, DiagnosticResult diagnostics)
        {
            this.root = root;
            this.sources = sources;
            this.diagnostics = diagnostics;
        }

        public void WriteTo(TextWriter writer)
        {
            var treeWriter = new TreeWriter(writer);
            treeWriter.Write(root);
        }

        public SyntaxToken GetRoot() => root;

        public IEnumerable<SourceText> GetSources() => sources;

        public DiagnosticResult GetDiagnostics() => diagnostics;

        public static SyntaxTree Parse(string text) => Parse(new[] { SourceText.FromText(text) }, new DiagnosticCollector());

        public static SyntaxTree Parse(IEnumerable<SourceText> sources, DiagnosticCollector collector)
        {
            var builder = ImmutableArray.CreateBuilder<SyntaxToken>();

            foreach (var sourceText in sources)
            {
                var parser = new Parser(sourceText, collector);
                var tree = parser.Parse();
                builder.Add(tree);
            }

            var compilationUnit = new SyntaxNode(SyntaxTokenKind.CompilationUnit, builder.ToImmutable());

            return new SyntaxTree(compilationUnit, sources, collector.ToResult());
        }

        public static ImmutableArray<SyntaxTerminal> Terminals(string text) => Terminals(SourceText.FromText(text));

        public static ImmutableArray<SyntaxTerminal> Terminals(SourceText sourceText)
        {
            var lexer = new Lexer(sourceText);
            return lexer.Terminals().ToImmutableArray();
        }
    }
}