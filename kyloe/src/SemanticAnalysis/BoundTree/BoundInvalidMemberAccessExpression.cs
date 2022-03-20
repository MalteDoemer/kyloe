using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundPropertyAccessExpression : BoundExpression
    {
        public BoundPropertyAccessExpression(IPropertySymbol propertySymbol, BoundExpression expression, string name)
        {
            PropertySymbol = propertySymbol;
            Expression = expression;
            Name = name;
        }

        public IPropertySymbol PropertySymbol { get; }
        public BoundExpression Expression { get; }
        public string Name { get; }

        public override ITypeSymbol ResultType => PropertySymbol.Type;

        public override ISymbol ResultSymbol => PropertySymbol;

        public override ValueCategory ValueCategory => PropertySymbol.SetterMethod is not null && PropertySymbol.SetterMethod.AccessModifiers == AccessModifiers.Public ? ValueCategory.ModifiableValue : ValueCategory.ReadableValue;

        public override BoundNodeType Type => BoundNodeType.BoundPropertyMemberAccessExpression;
    }

    internal sealed class BoundFieldAccessExpression : BoundExpression
    {
        public BoundFieldAccessExpression(IFieldSymbol fieldSymbol, BoundExpression expression, string name)
        {
            FieldSymbol = fieldSymbol;
            Expression = expression;
            Name = name;
        }

        public IFieldSymbol FieldSymbol { get; }
        public BoundExpression Expression { get; }
        public string Name { get; }

        public override ITypeSymbol ResultType => FieldSymbol.Type;

        public override ISymbol ResultSymbol => FieldSymbol;

        public override ValueCategory ValueCategory => FieldSymbol.IsReadonly ? ValueCategory.ReadableValue : ValueCategory.ModifiableValue;

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

        public override ISymbol ResultSymbol => NamespaceSymbol;

        public override ValueCategory ValueCategory => ValueCategory.None;

        public override BoundNodeType Type => BoundNodeType.BoundNamespaceMemberAccessExpression;
    }

    internal sealed class BoundTypeNameMemberAccessExpression : BoundExpression
    {
        public BoundTypeNameMemberAccessExpression(ITypeSymbol typeSymbol, BoundExpression expression, string name)
        {
            ResultType = typeSymbol;
            Expression = expression;
            Name = name;
        }

        public BoundExpression Expression { get; }
        public string Name { get; }

        public override ITypeSymbol ResultType { get; }

        public override ISymbol ResultSymbol => ResultType;

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

        public override ISymbol ResultSymbol => ResultType;

        public override ValueCategory ValueCategory => ValueCategory.None;

        public override BoundNodeType Type => BoundNodeType.BoundInvalidMemberAccessExpression;
    }
}