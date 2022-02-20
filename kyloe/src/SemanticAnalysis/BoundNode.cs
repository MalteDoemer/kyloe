using Kyloe.Utility;


namespace Kyloe.Semantics
{
    public abstract class BoundNode
    {
        public abstract BoundNodeType Type { get; }

        public abstract SourceLocation Location { get; }

        public abstract TypeInfo TypeInfo { get; }
    }

    internal abstract class BoundStatement : BoundNode {}

    internal abstract class BoundExpression : BoundNode {}
}