using System.Collections.Generic;
using Kyloe.Semantics;
using Kyloe.Symbols;

namespace Kyloe.Lowering
{
    internal sealed class LoweredLiteralExpression : LoweredExpression
    {
        public LoweredLiteralExpression(TypeSpecifier type, object value)
        {
            Value = value;
            Type = type;
        }

        public object Value { get;  }

        public override TypeSpecifier Type { get; }

        public override ValueCategory ValueCategory => ValueCategory.ReadableValue;

        public override LoweredNodeKind Kind => LoweredNodeKind.LoweredLiteralExpression;

        public override IEnumerable<LoweredNode> Children()
        {
            yield break;
        }
    }
}
