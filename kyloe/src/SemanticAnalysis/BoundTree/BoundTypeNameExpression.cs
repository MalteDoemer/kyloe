using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal sealed class BoundTypeNameExpression : BoundExpression
    {
        public BoundTypeNameExpression(TypeNameSymbol typeName)
        {
            TypeName = typeName;
        }

        public TypeNameSymbol TypeName { get; }

        public override TypeSpecifier ResultType => TypeName.Type;

        public override ValueCategory ValueCategory => ValueCategory.None;

        public override BoundNodeType Type => BoundNodeType.BoundTypeNameExpression;
    }
}