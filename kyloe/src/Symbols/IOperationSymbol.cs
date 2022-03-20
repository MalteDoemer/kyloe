using Kyloe.Semantics;


namespace Kyloe.Symbols
{
    public interface IOperationSymbol : IMemberSymbol
    {
        IMethodSymbol UnderlyingMethod { get; }

        BoundOperation Operation { get; }

        bool IsBuiltin { get; }
    }
}