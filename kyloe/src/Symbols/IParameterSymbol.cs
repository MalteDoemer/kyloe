namespace Kyloe.Symbols
{
    public interface IParameterSymbol : ISymbol
    {
        ITypeSymbol Type { get; }
    }
}