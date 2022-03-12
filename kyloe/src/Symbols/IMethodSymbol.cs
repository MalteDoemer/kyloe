using System.Collections.Generic;

namespace Kyloe.Symbols
{
    public interface IMethodSymbol : ISymbol
    {
        ITypeSymbol ReturnType { get; }
        IEnumerable<IParameterSymbol> Parameters { get; }

        bool IsOperator { get; }
        bool IsBuiltinOperator { get; }
        bool IsStatic { get; }
    }
}