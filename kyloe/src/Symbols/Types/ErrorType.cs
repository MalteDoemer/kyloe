using System;

namespace Kyloe.Symbols
{
    internal sealed class ErrorType : TypeInfo
    {
        public override IReadOnlySymbolScope? ReadOnlyScope => null;

        public override TypeKind Kind => TypeKind.ErrorType;

        public override string FullName() => "<error-type>";
    }
}