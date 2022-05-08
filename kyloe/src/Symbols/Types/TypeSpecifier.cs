using System;
using System.Collections.Generic;

namespace Kyloe.Symbols
{
    internal abstract class TypeInfo
    {
        public abstract TypeKind Kind { get; }

        public abstract IReadOnlySymbolScope? ReadOnlyScope { get; }

        public abstract string FullName();

        public override string ToString() => FullName();
    }


    internal abstract class ValueTypeInfo : TypeInfo { }

    internal abstract class FunctionTypeInfo : TypeInfo 
    {
        public abstract FunctionGroupType Group { get; }
        public abstract TypeInfo ReturnType { get; }

        public abstract IEnumerable<TypeInfo> Parameters { get; }

        public override IReadOnlySymbolScope? ReadOnlyScope => null;
    }
}