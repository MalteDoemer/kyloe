using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundNamespaceMemberAccessExpression : BoundExpression
    {
        public BoundNamespaceMemberAccessExpression(INamespaceSymbol namespaceSymbol, BoundExpression expression, string name)
        {
            NamespaceSymbol = namespaceSymbol;
            Expression = expression;
            Name = name;
        }

        public INamespaceSymbol NamespaceSymbol { get ;}
        public BoundExpression Expression { get; }
        public string Name { get; }

        public override ISymbol ResultSymbol => NamespaceSymbol;

        public override ValueCategory ValueCategory => ValueCategory.None;

        public override BoundNodeType Type => BoundNodeType.BoundNamespaceMemberAccessExpression;
    }

    internal sealed class BoundTypeNameMemberAccessExpression : BoundExpression
    {
        public BoundTypeNameMemberAccessExpression(ITypeSymbol typeSymbol, BoundExpression expression, string name)
        {
            TypeSymbol = typeSymbol;
            Expression = expression;
            Name = name;
        }

        public ITypeSymbol TypeSymbol { get ;}
        public BoundExpression Expression { get; }
        public string Name { get; }

        public override ISymbol ResultSymbol => TypeSymbol;

        public override ValueCategory ValueCategory => ValueCategory.None;

        public override BoundNodeType Type => BoundNodeType.BoundTypeNameMemberAccessExpression;
    }

    internal sealed class BoundInvalidMemberAccessExpression : BoundExpression
    {
        private readonly ITypeSymbol errorType;

        public BoundInvalidMemberAccessExpression(TypeSystem typeSystem, BoundExpression expression, string name)
        {
            errorType = typeSystem.Error;
            Expression = expression;
            Name = name;
        }

        public BoundExpression Expression { get; }
        public string Name { get; }

        public override ISymbol ResultSymbol => errorType;

        public override ValueCategory ValueCategory => ValueCategory.None;

        public override BoundNodeType Type => BoundNodeType.BoundInvalidMemberAccessExpression;
    }
}