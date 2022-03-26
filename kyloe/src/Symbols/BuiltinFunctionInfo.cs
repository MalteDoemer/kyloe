using System.Collections.Immutable;

namespace Kyloe.Symbols
{
    internal partial class TypeSystem
    {
        private static class BuiltinFunctionInfo
        {
            public static readonly ImmutableArray<(string name, BuiltinTypeKind ret, ImmutableArray<(string name, BuiltinTypeKind type)> parameter)> BuiltinFunctions = ImmutableArray.Create<(string name, BuiltinTypeKind ret, ImmutableArray<(string name, BuiltinTypeKind type)> parameter)>(
                ("println", BuiltinTypeKind.Void, ImmutableArray.Create<(string name, BuiltinTypeKind type)>(("arg", BuiltinTypeKind.String))),
                ("println", BuiltinTypeKind.Void, ImmutableArray.Create<(string name, BuiltinTypeKind type)>(("arg", BuiltinTypeKind.I64))),
                ("println", BuiltinTypeKind.Void, ImmutableArray.Create<(string name, BuiltinTypeKind type)>(("arg", BuiltinTypeKind.Double)))
            );
        }
    }


}