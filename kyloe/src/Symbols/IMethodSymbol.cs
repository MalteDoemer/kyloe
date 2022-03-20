using System.Collections.Generic;

namespace Kyloe.Symbols
{
    public interface IMethodSymbol : IMemberSymbol
    {
        ITypeSymbol ReturnType { get; }
        IEnumerable<IParameterSymbol> Parameters { get; }

        bool IsStatic { get; }
    }
}