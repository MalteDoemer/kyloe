using System.Collections.Immutable;

namespace Kyloe.Grammar
{
    internal sealed class BoundGrammar
    {
        public BoundGrammar(ImmutableDictionary<TokenKind, TerminalDefinition> terminals, ImmutableDictionary<TokenKind, ProductionRule> rules)
        {
            Terminals = terminals;
            Rules = rules;
        }

        public ImmutableDictionary<TokenKind, TerminalDefinition> Terminals { get; }
        public ImmutableDictionary<TokenKind, ProductionRule> Rules { get; }
    }
}