using System.Collections.Immutable;
using System.IO;

using Kyloe.Syntax;
using Kyloe.Utility;
using Kyloe.Diagnostics;

namespace Kyloe
{
    public class SyntaxTree
    {
        private readonly SyntaxNode root;
        private readonly SourceText sourceText;
        private readonly DiagnosticResult diagnostics;

        private SyntaxTree(SyntaxNode root, SourceText sourceText, DiagnosticResult diagnostics)
        {
            this.root = root;
            this.sourceText = sourceText;
            this.diagnostics = diagnostics;
        }

        public SyntaxNode GetRoot() => root;

        public SourceText GetSourceText() => sourceText;

        public DiagnosticResult GetDiagnostics() => diagnostics;

        public static SyntaxTree ParseStatement(string text) => ParseStatement(SourceText.FromText(text));

        public static SyntaxTree ParseStatement(SourceText sourceText)
        {
            using (var reader = sourceText.GetReader())
            {
                var collector = new DiagnosticCollector(sourceText);
                var lexer = new Lexer(reader, collector);
                var parser = new Parser(lexer, collector);

                var tree = parser.ParseStatement();
                var result = collector.ToResult();

                return new SyntaxTree(tree, sourceText, result);
            }
        }

        public static SyntaxTree ParseExpression(string text) => ParseExpression(SourceText.FromText(text));

        public static SyntaxTree ParseExpression(SourceText sourceText)
        {
            using (var reader = sourceText.GetReader())
            {
                var collector = new DiagnosticCollector(sourceText);
                var lexer = new Lexer(reader, collector);
                var parser = new Parser(lexer, collector);

                var tree = parser.ParseExpression();
                var result = collector.ToResult();

                return new SyntaxTree(tree, sourceText, result);
            }
        }

        public static SyntaxTree Parse(string text) => Parse(SourceText.FromText(text));

        public static SyntaxTree Parse(SourceText sourceText) 
        {
            using (var reader = sourceText.GetReader())
            {
                var collector = new DiagnosticCollector(sourceText);
                var lexer = new Lexer(reader, collector);
                var parser = new Parser(lexer, collector);

                var tree = parser.Parse();
                var result = collector.ToResult();

                return new SyntaxTree(tree, sourceText, result);
            }
        }

        public static (ImmutableArray<SyntaxToken>, DiagnosticResult) Tokenize(string text) => Tokenize(SourceText.FromText(text));

        public static (ImmutableArray<SyntaxToken>, DiagnosticResult) Tokenize(SourceText sourceText)
        {
            using (var reader = sourceText.GetReader())
            {
                var collector = new DiagnosticCollector(sourceText);
                var lexer = new Lexer(reader, collector);

                var array = lexer.Tokens().ToImmutableArray();

                return (array, collector.ToResult());
            }
        }
    }
}