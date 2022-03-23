using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundSymbolExpression : BoundExpression
    {
        public BoundSymbolExpression(Symbol symbol)
        {
            Symbol = symbol;
        }

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
                        var localSymbol = (LocalVariableSymbol)Symbol;
                        return localSymbol.IsConst ? ValueCategory.ReadableValue : ValueCategory.ModifiableValue;
                    case SymbolKind.FieldSymbol:
                        var fieldSymbol = (FieldSymbol)Symbol;
                        return fieldSymbol.IsReadonly ? ValueCategory.ReadableValue : ValueCategory.ModifiableValue;
                    case SymbolKind.PropertySymbol:
                        var propertySymbol = (PropertySymbol)Symbol;
                        return propertySymbol.SetMethod is not null && propertySymbol.SetMethod.AccessModifiers == AccessModifiers.Public ? ValueCategory.ModifiableValue : ValueCategory.ReadableValue;

                    default:
                        throw new System.Exception($"unexpected Symbol kind: {Symbol.Kind}");
                }
            }
        }

        public override BoundNodeType Type => BoundNodeType.BoundSymbolExpression;
    }
}