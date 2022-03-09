namespace Kyloe.Symbols
{
    public interface ILocalVariableSymbol : ISymbol
    {
        ITypeSymbol Type { get; }
        bool IsConst { get; }
    }
}