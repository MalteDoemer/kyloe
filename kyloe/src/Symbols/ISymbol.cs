using System.Collections.Generic;

namespace Kyloe.Symbols
{

    public enum SymbolKind
    {
        NamespaceSymbol,
        TypeSymbol,
        MethodSymbol,
        ParameterSymbol,
    }

    public interface ISymbol
    {
        string Name { get; }
        SymbolKind Kind { get; }
    }

    public interface ISymbolContainer
    {
        IEnumerable<ISymbol> Members { get; }
        IEnumerable<ISymbol> LookupMembers(string name);
    }

    public interface INamespaceSymbol : ISymbol, ISymbolContainer
    {
        IEnumerable<ITypeSymbol> Types { get; }
        IEnumerable<INamespaceSymbol> Namespaces { get; }
    }

    public interface ITypeSymbol : ISymbol, ISymbolContainer
    {
        IEnumerable<IMethodSymbol> Methods { get; }
    }

    public interface IMethodSymbol : ISymbol
    {
        ITypeSymbol ReturnType { get; }
        IEnumerable<IParameterSymbol> Parameters { get; }

        bool IsOperator { get; }
        bool IsBuiltinOperator { get; }
    }

    public interface IParameterSymbol : ISymbol
    {
        ITypeSymbol Type { get; }
    }
}