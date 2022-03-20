namespace Kyloe.Symbols
{
    public interface IFieldSymbol : IMemberSymbol
    {
        ITypeSymbol Type { get; }

        bool IsReadonly { get; }
        bool IsStatic { get; }
    }
}