using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundNamespaceExpression : BoundExpression
    {
        public BoundNamespaceExpression(INamespaceSymbol namespaceSymbol)
        {
            NamespaceSymbol = namespaceSymbol;
        }

        public INamespaceSymbol NamespaceSymbol { get; }

        public override ISymbol ResultSymbol => NamespaceSymbol;

        public override ValueCategory ValueCategory => ValueCategory.None;

        public override BoundNodeType Type => BoundNodeType.BoundNamespaceExpression;
    }
}