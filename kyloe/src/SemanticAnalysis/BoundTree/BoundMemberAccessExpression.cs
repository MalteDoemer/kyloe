using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundMemberAccessExpression : BoundExpression
    {
        public BoundMemberAccessExpression(BoundExpression expression, Symbol symbol)
        {
            Expression = expression;
            Symbol = symbol;
        }

        public BoundExpression Expression { get; }
        public Symbol Symbol { get; }

        public override TypeSpecifier ResultType => Symbol.Type;

        public override ValueCategory ValueCategory
        {
            get
            {
                switch (Symbol.Kind)
                {
                    case SymbolKind.NamespaceSymbol:
                        return ValueCategory.None;
                    case SymbolKind.TypeNameSymbol:
                        return ValueCategory.None;
                    case SymbolKind.MethodGroupSymbol:
                        return ValueCategory.ReadableValue;
                    case SymbolKind.LocalVariableSymbol:
                        return ((LocalVariableSymbol)Symbol).IsConst ? ValueCategory.ReadableValue : ValueCategory.ModifiableValue;
                    case SymbolKind.FieldSymbol:
                        return ((FieldSymbol)Symbol).IsReadonly ? ValueCategory.ReadableValue : ValueCategory.ModifiableValue;
                    case SymbolKind.PropertySymbol:
                        var property = (PropertySymbol)Symbol;
                        return property.SetMethod is not null && property.SetMethod.AccessModifiers == AccessModifiers.Public ? ValueCategory.ModifiableValue : ValueCategory.ReadableValue;
                    case SymbolKind.OperationSymbol:
                        return ValueCategory.ReadableValue;
                    default:
                        throw new System.Exception($"unexpected symbol kind '{Symbol.Kind}'");
                }
            }
        }

        public override BoundNodeType Type => BoundNodeType.BoundMemberAccessExpression;
    }
}