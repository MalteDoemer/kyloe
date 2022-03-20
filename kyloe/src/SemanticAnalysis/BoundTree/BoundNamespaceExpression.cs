using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundNamespaceExpression : BoundExpression
    {
        public BoundNamespaceExpression(TypeSystem typeSystem, INamespaceSymbol namespaceSymbol)
        {
            NamespaceSymbol = namespaceSymbol;
            ResultType = typeSystem.Empty;
        }

        public INamespaceSymbol NamespaceSymbol { get; }

        public override ITypeSymbol ResultType { get; }

        public override ISymbol ResultSymbol => NamespaceSymbol;

        public override ValueCategory ValueCategory => ValueCategory.None;

        public override BoundNodeType Type => BoundNodeType.BoundNamespaceExpression;
    }
}