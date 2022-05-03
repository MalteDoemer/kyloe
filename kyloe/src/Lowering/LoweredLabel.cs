using System.Collections.Generic;
using System.Threading;

namespace Kyloe.Lowering
{
    internal sealed class LoweredLabel
    {
        private static int Counter = 0;

        public static LoweredLabel Create(string name)
        {
            var id = Interlocked.Increment(ref Counter);
            return new LoweredLabel(name, id);
        }

        private LoweredLabel(string name, int value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public int Value { get; }

        public override string ToString() => $"{Name}.{Value}";
    }
}
