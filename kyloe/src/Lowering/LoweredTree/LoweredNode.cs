using System.Collections.Generic;

namespace Kyloe.Lowering
{
    public abstract class LoweredNode
    {
        public abstract LoweredNodeKind Kind { get; }

        public abstract IEnumerable<LoweredNode> Children();
    }
}
