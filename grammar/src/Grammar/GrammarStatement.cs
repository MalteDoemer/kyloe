namespace Kyloe.Grammar
{
    internal sealed class GrammarStatement
    {
        public GrammarStatement(GrammarToken nameToken, GrammarNode node)
        {
            NameToken = nameToken;
            Node = node;
        }

        public GrammarToken NameToken { get; }
        public GrammarNode Node { get; }

        public override string ToString() => $"{NameToken.Text} = {Node};";
    }
}