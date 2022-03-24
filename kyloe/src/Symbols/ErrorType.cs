namespace Kyloe.Symbols
{
    internal sealed class ErrorType : TypeSpecifier
    {
        public override IReadOnlySymbolScope? ReadOnlyScope => null;

        public override TypeKind Kind => TypeKind.ErrorType;

        public override bool Equals(TypeSpecifier? other) => other is ErrorType;

        public override string FullName() => "<error-type>";
    }
}