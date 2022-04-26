using System.Collections.Immutable;
using System.IO;

using Kyloe.Syntax;
using Kyloe.Utility;
using Kyloe.Diagnostics;
using System.Collections.Generic;

namespace Kyloe
{
    public class SyntaxTree
    {
        private readonly SyntaxToken? root;
        private readonly SourceText sourceText;
        private readonly DiagnosticResult diagnostics;

        private SyntaxTree(SyntaxToken? root, SourceText sourceText, DiagnosticResult diagnostics)
        {
            this.root = root;
            this.sourceText = sourceText;
            this.diagnostics = diagnostics;
        }

        public void WriteTo(TextWriter writer) 
        {
            var treeWriter = new TreeWriter(writer);
            treeWriter.Write(root);
        }

        public SourceText GetSourceText() => sourceText;

        public DiagnosticResult GetDiagnostics() => diagnostics;

        public static SyntaxTree Parse(string text) => Parse(SourceText.FromText(text));

        public static SyntaxTree Parse(SourceText sourceText) 
        {
            var errors = new List<SyntaxError>();
            var parser = new Parser(sourceText.GetAllText(), errors);
            var tree = parser.Parse();

            var collector = new DiagnosticCollector(sourceText);
            collector.AddRange(errors);

            return new SyntaxTree(tree, sourceText, collector.ToResult());
        }

        public static ImmutableArray<SyntaxTerminal> Terminals(string text) => Terminals(SourceText.FromText(text));

        public static ImmutableArray<SyntaxTerminal> Terminals(SourceText sourceText)
        {
            var lexer = new Lexer(sourceText.GetAllText());
            return lexer.Terminals().ToImmutableArray();
        }
    }
}