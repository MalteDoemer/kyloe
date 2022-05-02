using System.Collections.Generic;
using System.Threading;

namespace Kyloe.Lowering
{
    internal sealed class LoweredLabel
    {
        private static int Counter = 0;

        public static LoweredLabel Create()
        {
            var id = Interlocked.Increment(ref Counter);
            return new LoweredLabel(id);
        }

        private LoweredLabel(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }
}
