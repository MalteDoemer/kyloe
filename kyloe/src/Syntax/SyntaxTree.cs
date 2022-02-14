using System.Collections.Immutable;
using System.IO;
using Kyloe.Diagnostics;

namespace Kyloe.Syntax
{
    public class SyntaxTree
    {
        private readonly SyntaxNode root;
        private readonly DiagnosticResult diagnostics;

        private SyntaxTree(SyntaxNode root, DiagnosticResult diagnostics)
        {
            this.root = root;
            this.diagnostics = diagnostics;
        }

        public SyntaxNode GetRoot() => root;

        public DiagnosticResult GetDiagnostics() => diagnostics;

        public static SyntaxTree Parse(string text) => Parse(new StringReader(text));

        public static SyntaxTree Parse(TextReader reader)
        {
            var collector = new DiagnosticCollector();
            var lexer = new Lexer(reader, collector);
            var parser = new Parser(lexer, collector);

            var tree = parser.Parse();
            var result = collector.ToResult();

            return new SyntaxTree(tree, result);
        }


        public static SyntaxTree ParseExpression(string text) => ParseExpression(new StringReader(text));

        public static SyntaxTree ParseExpression(TextReader reader)
        {
            var collector = new DiagnosticCollector();
            var lexer = new Lexer(reader, collector);
            var parser = new Parser(lexer, collector);

            var tree = parser.ParseExpression();
            var result = collector.ToResult();

            return new SyntaxTree(tree, result);
        }


        public static (ImmutableArray<SyntaxToken>, DiagnosticResult) Tokenize(string text) => Tokenize(new StringReader(text));

        public static (ImmutableArray<SyntaxToken>, DiagnosticResult) Tokenize(TextReader reader)
        {
            var collector = new DiagnosticCollector();
            var lexer = new Lexer(reader, collector);

            var array = lexer.Tokens().ToImmutableArray();

            return (array, collector.ToResult());
        }
    }
}