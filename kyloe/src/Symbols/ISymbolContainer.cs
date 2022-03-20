using System.Collections.Generic;

namespace Kyloe.Symbols
{
    public interface IMemberContainer
    {
        IEnumerable<IMemberSymbol> Members { get; }
        IEnumerable<IMemberSymbol> LookupMembers(string name);
    }
}