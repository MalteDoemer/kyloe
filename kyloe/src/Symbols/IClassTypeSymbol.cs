using System.Collections.Generic;

namespace Kyloe.Symbols
{
    public interface IArrayTypeSymbol : ITypeSymbol 
    {
        ITypeSymbol ElementType { get; }
    }

    public interface IPointerTypeSymbol : ITypeSymbol 
    {
        ITypeSymbol UnderlyingType { get; }
    }

    public interface IByRefTypeSymbol : ITypeSymbol 
    {
        ITypeSymbol UnderlyingType { get; }
    } 

    public interface IClassTypeSymbol : ITypeSymbol
    {
        IEnumerable<IMethodSymbol> Constructors { get; }
        IEnumerable<IClassTypeSymbol> NestedClasses { get; }
    }
}