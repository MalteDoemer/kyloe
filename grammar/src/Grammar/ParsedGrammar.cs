using System.Collections.Immutable;

namespace Kyloe.Grammar
{
    internal sealed class ParsedGrammar 
    {
        public ParsedGrammar(ImmutableArray<GrammarStatement> statements)
        {
            Statements = statements;
        }

        public ImmutableArray<GrammarStatement> Statements { get; }
    }
}