using System.Collections.Immutable;
using System.Linq;

namespace Kyloe.Grammar
{
    internal sealed class GrammarParser
    {
        private readonly ImmutableArray<GrammarToken> tokens;
        private int pos;

        public GrammarParser(string text)
        {
            var lexer = new GrammarLexer(text);
            this.tokens = lexer.Tokens().ToImmutableArray();
            this.pos = 0;
        }

        private GrammarToken current => pos < tokens.Length ? tokens[pos] : tokens.Last();

        private GrammarToken Advance()
        {
            var temp = current;
            pos += 1;
            return temp;
        }

        private GrammarToken Expect(GrammarTokenKind kind)
        {
            if (current.Kind == kind)
                return Advance();

            throw new GrammarException($"expected {kind}, got {current.Kind}", current.Location);
        }

        private GrammarStatement ParseStatement()
        {
            var name = Expect(GrammarTokenKind.Identifier);
            Expect(GrammarTokenKind.Equal);
            var node = ParseNode();
            Expect(GrammarTokenKind.SemiColon);
            return new GrammarStatement(name, node);
        }

        private GrammarNode ParseNode()
        {
            return ParseConcat();
        }

        private GrammarNode ParseConcat()
        {
            var left = ParseOr();

            while (current.Kind == GrammarTokenKind.Comma)
            {
                var op = Advance();
                var right = ParseOr();
                left = new ConcatGrammarNode(left, right);
            }

            return left;
        }

        private GrammarNode ParseOr()
        {
            var left = ParsePrimary();

            while (current.Kind == GrammarTokenKind.Or)
            {
                var op = Advance();
                var right = ParsePrimary();
                left = new OrGrammarNode(left, right);
            }

            return left;
        }


        private GrammarNode ParsePrimary()
        {
            if (current.Kind == GrammarTokenKind.Identifier)
            {
                return new NameGrammarNode(Advance());
            }
            else if (current.Kind == GrammarTokenKind.RegexString)
            {
                return new LiteralGrammarNode(Advance(), isRegex: true);
            }
            else if (current.Kind == GrammarTokenKind.LiteralString)
            {
                return new LiteralGrammarNode(Advance(), isRegex: false);
            }
            else if (current.Kind == GrammarTokenKind.Hash)
            {
                return new OptionalGrammarNode(Advance());
            }
            else if (current.Kind == GrammarTokenKind.LeftParen)
            {
                Advance();
                var node = ParseNode();
                Expect(GrammarTokenKind.RightParen);
                return node;
            }
            else
            {
                throw new GrammarException($"unexpected token: '{current.Kind}'", current.Location);
            }
        }

        public ParsedGrammar Parse()
        {
            var builder = ImmutableArray.CreateBuilder<GrammarStatement>();

            while (current.Kind != GrammarTokenKind.End)
                builder.Add(ParseStatement());

            return new ParsedGrammar(builder.ToImmutable());
        }
    }
}