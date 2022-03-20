using System.Collections.Generic;

namespace Kyloe.Symbols
{
    public partial class TypeSystem
    {
        private sealed class ByRefTypeSymbol : IByRefTypeSymbol
        {
            public ByRefTypeSymbol(ITypeSymbol underlyingType)
            {
                UnderlyingType = underlyingType;
                Name = UnderlyingType.Name + "&";
            }

            public string Name { get; }

            public ITypeSymbol UnderlyingType { get; }

            public SymbolKind Kind => SymbolKind.ByRefTypeSymbol;

            public AccessModifiers AccessModifiers => UnderlyingType.AccessModifiers;

            public IEnumerable<IOperationSymbol> Operations => UnderlyingType.Operations;

            public IEnumerable<IMethodSymbol> Methods => UnderlyingType.Methods;

            public IEnumerable<IPropertySymbol> Properties => UnderlyingType.Properties;

            public IEnumerable<IFieldSymbol> Fields => UnderlyingType.Fields;

            public IEnumerable<IMemberSymbol> Members => UnderlyingType.Members;

            public bool Equals(ISymbol? other) => other is IByRefTypeSymbol byref && byref.UnderlyingType.Equals(UnderlyingType);

            public IEnumerable<IMemberSymbol> LookupMembers(string name) => UnderlyingType.LookupMembers(name);
        }
    }
}