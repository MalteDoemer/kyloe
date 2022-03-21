using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundNamespaceExpression : BoundExpression
    {
        public BoundNamespaceExpression(TypeSystem typeSystem, NamespaceSymbol namespaceSymbol)
        {
            NamespaceSymbol = namespaceSymbol;
        }

        public NamespaceSymbol NamespaceSymbol { get; }

        public override TypeSpecifier ResultType => NamespaceSymbol.Type;

        public override ValueCategory ValueCategory => ValueCategory.None;

        public override BoundNodeType Type => BoundNodeType.BoundNamespaceExpression;
    }
}