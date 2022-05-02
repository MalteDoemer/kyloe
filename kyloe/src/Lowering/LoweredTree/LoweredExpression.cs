using Kyloe.Semantics;
using Kyloe.Symbols;

namespace Kyloe.Lowering
{
    internal abstract class LoweredExpression : LoweredNode
    {
        public abstract TypeSpecifier Type { get; }

        public abstract bool HasKnownValue { get; }

        public abstract ValueCategory ValueCategory { get; }

        public bool IsValue => ValueCategory == ValueCategory.ReadableValue || ValueCategory == ValueCategory.ModifiableValue;

        public bool IsModifiableValue => ValueCategory == ValueCategory.ModifiableValue;

        public bool IsTypeName => ValueCategory == ValueCategory.TypeName;
    }
}
