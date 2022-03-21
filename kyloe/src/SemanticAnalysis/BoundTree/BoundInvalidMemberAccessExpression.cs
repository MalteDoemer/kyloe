using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundPropertyAccessExpression : BoundExpression
    {
        public BoundPropertyAccessExpression(PropertySymbol propertySymbol, BoundExpression expression, string name)
        {
            PropertySymbol = propertySymbol;
            Expression = expression;
            Name = name;
        }

        public PropertySymbol PropertySymbol { get; }
        public BoundExpression Expression { get; }
        public string Name { get; }

        public override TypeSpecifier ResultType => PropertySymbol.Type;

        public override ValueCategory ValueCategory => PropertySymbol.SetMethod is not null && PropertySymbol.SetMethod.AccessModifiers == AccessModifiers.Public ? ValueCategory.ModifiableValue : ValueCategory.ReadableValue;

        public override BoundNodeType Type => BoundNodeType.BoundPropertyMemberAccessExpression;
    }

    internal sealed class BoundFieldAccessExpression : BoundExpression
    {
        public BoundFieldAccessExpression(FieldSymbol fieldSymbol, BoundExpression expression, string name)
        {
            FieldSymbol = fieldSymbol;
            Expression = expression;
            Name = name;
        }

        public FieldSymbol FieldSymbol { get; }
        public BoundExpression Expression { get; }
        public string Name { get; }

        public override TypeSpecifier ResultType => FieldSymbol.Type;

        public override ValueCategory ValueCategory => FieldSymbol.IsReadonly ? ValueCategory.ReadableValue : ValueCategory.ModifiableValue;

        public override BoundNodeType Type => BoundNodeType.BoundFieldMemberAccessExpression;
    }

    internal sealed class BoundNamespaceMemberAccessExpression : BoundExpression
    {
        public BoundNamespaceMemberAccessExpression(TypeSystem typeSystem, NamespaceSymbol namespaceSymbol, BoundExpression expression, string name)
        {
            NamespaceSymbol = namespaceSymbol;
            Expression = expression;
            Name = name;
        }

        public NamespaceSymbol NamespaceSymbol { get; }
        public BoundExpression Expression { get; }
        public string Name { get; }

        public override TypeSpecifier ResultType => NamespaceSymbol.Type;

        public override ValueCategory ValueCategory => ValueCategory.None;

        public override BoundNodeType Type => BoundNodeType.BoundNamespaceMemberAccessExpression;
    }

    internal sealed class BoundTypeNameMemberAccessExpression : BoundExpression
    {
        public BoundTypeNameMemberAccessExpression(TypeSpecifier typeSymbol, BoundExpression expression, string name)
        {
            ResultType = typeSymbol;
            Expression = expression;
            Name = name;
        }

        public BoundExpression Expression { get; }
        public string Name { get; }

        public override TypeSpecifier ResultType { get; }

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

        public override TypeSpecifier ResultType { get; }

        public override ValueCategory ValueCategory => ValueCategory.None;

        public override BoundNodeType Type => BoundNodeType.BoundInvalidMemberAccessExpression;
    }
}