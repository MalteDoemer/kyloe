using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundFieldMemberAccessExpression : BoundExpression
    {
        public BoundFieldMemberAccessExpression(IFieldSymbol fieldSymbol, BoundExpression expression, string name)
        {
            FieldSymbol = fieldSymbol;
            Expression = expression;
            Name = name;
        }

        public IFieldSymbol FieldSymbol { get; }
        public BoundExpression Expression { get; }
        public string Name { get; }

        public override ITypeSymbol ResultType => FieldSymbol.Type;

        public override ValueCategory ValueCategory => FieldSymbol.IsReadonly ? ValueCategory.RValue : ValueCategory.LValue;

        public override BoundNodeType Type => BoundNodeType.BoundFieldMemberAccessExpression;
    }

    internal sealed class BoundNamespaceMemberAccessExpression : BoundExpression
    {
        public BoundNamespaceMemberAccessExpression(TypeSystem typeSystem, INamespaceSymbol namespaceSymbol, BoundExpression expression, string name)
        {
            NamespaceSymbol = namespaceSymbol;
            Expression = expression;
            Name = name;
            ResultType = typeSystem.Empty;
        }

        public INamespaceSymbol NamespaceSymbol { get; }
        public BoundExpression Expression { get; }
        public string Name { get; }

        public override ITypeSymbol ResultType { get; }

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

        public ITypeSymbol TypeSymbol { get; }
        public BoundExpression Expression { get; }
        public string Name { get; }

        public override ITypeSymbol ResultType => TypeSymbol;

        public override ValueCategory ValueCategory => ValueCategory.None;

        public override BoundNodeType Type => BoundNodeType.BoundTypeNameMemberAccessExpression;
    }

    internal sealed class BoundInvalidMemberAccessExpression : BoundExpression
    {
        public BoundInvalidMemberAccessExpression(TypeSystem typeSystem, BoundExpression expression, string name)
        {
            ResultType = typeSystem.Error;
            Expression = expression;
            Name = name;
        }

        public BoundExpression Expression { get; }
        public string Name { get; }

        public override ITypeSymbol ResultType { get; }

        public override ValueCategory ValueCategory => ValueCategory.None;

        public override BoundNodeType Type => BoundNodeType.BoundInvalidMemberAccessExpression;
    }
}