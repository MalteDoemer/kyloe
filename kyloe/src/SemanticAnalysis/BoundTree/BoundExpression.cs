using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal abstract class BoundExpression : BoundNode
    {
        public abstract TypeInfo ResultType { get; }

        public abstract ValueCategory ValueCategory { get; }

        public bool IsValue => ValueCategory == ValueCategory.ReadableValue || ValueCategory == ValueCategory.ModifiableValue;

        public bool IsModifiableValue => ValueCategory == ValueCategory.ModifiableValue;

        public bool IsTypeName => ValueCategory == ValueCategory.TypeName;
    }
}