using System.Collections.Generic;

namespace Kyloe.Symbols
{
    public interface ITypeSymbol : IMemberSymbol, IMemberContainer
    {
        IEnumerable<IOperationSymbol> Operations { get; }
        IEnumerable<IMethodSymbol> Methods { get; }
        IEnumerable<IPropertySymbol> Properties { get; }
        IEnumerable<IFieldSymbol> Fields { get; }
    }
}