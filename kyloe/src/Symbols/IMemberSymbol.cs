namespace Kyloe.Symbols
{
    public interface IMemberSymbol : ISymbol 
    {
        AccessModifiers AccessModifiers { get; }
    }
}