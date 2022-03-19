using System.Collections.Generic;

namespace Kyloe.Symbols
{
    public enum AccessModifiers
    {
        Public,
        Internal,
        Protected,
        Private,
        InternalOrProtected,
        InternalAndProtected,
    }


    public interface IFieldSymbol : ISymbol
    {
        ITypeSymbol Type { get; }
        AccessModifiers AccessModifiers { get; }
    }

    public interface ITypeSymbol : ISymbol, ISymbolContainer
    {
        AccessModifiers AccessModifiers { get; }
        IEnumerable<IMethodSymbol> Methods { get; }
        IEnumerable<IFieldSymbol> Fields { get; }
        IEnumerable<IClassTypeSymbol> NestedClasses { get; }
    }

    public interface IArrayTypeSymbol : ITypeSymbol
    {
        ITypeSymbol ElementType { get; }
    }

    public interface IClassTypeSymbol : ITypeSymbol 
    {
    }

}