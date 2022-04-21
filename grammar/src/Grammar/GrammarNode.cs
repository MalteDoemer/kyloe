namespace Kyloe.Grammar
{
    internal abstract class GrammarNode
    {
        public abstract GrammarLocation Location { get; }
    }

    internal sealed class NameGrammarNode : GrammarNode
    {
        public NameGrammarNode(GrammarToken nameToken)
        {
            NameToken = nameToken;
        }

        public GrammarToken NameToken { get; }

        public override GrammarLocation Location => NameToken.Location;

        public override string ToString() => NameToken.Text;
    }

    internal sealed class LiteralGrammarNode : GrammarNode
    {
        public LiteralGrammarNode(GrammarToken literalToken)
        {
            LiteralToken = literalToken;
        }

        public GrammarToken LiteralToken { get; }

        public override GrammarLocation Location => LiteralToken.Location;

        public override string ToString() => $"'{LiteralToken.Text}'";
    }

    internal sealed class OptionalGrammarNode : GrammarNode
    {
        public OptionalGrammarNode(GrammarToken hashToken)
        {
            HashToken = hashToken;
        }

        public GrammarToken HashToken { get; }

        public override GrammarLocation Location => HashToken.Location;

        public override string ToString() => "#";
    }

    internal sealed class OrGrammarNode : GrammarNode
    {
        public OrGrammarNode(GrammarNode left, GrammarNode right)
        {
            Left = left;
            Right = right;
        }

        public GrammarNode Left { get; }
        public GrammarNode Right { get; }

        public override GrammarLocation Location => GrammarLocation.CreateAround(Left.Location, Right.Location);

        public override string ToString() => $"({Left} | {Right})";
    }

    internal sealed class ConcatGrammarNode : GrammarNode
    {
        public ConcatGrammarNode(GrammarNode left, GrammarNode right)
        {
            Left = left;
            Right = right;
        }

        public GrammarNode Left { get; }
        public GrammarNode Right { get; }

        public override GrammarLocation Location => GrammarLocation.CreateAround(Left.Location, Right.Location);

        public override string ToString() => $"({Left}, {Right})";
    }
}