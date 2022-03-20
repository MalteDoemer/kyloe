namespace Kyloe.Symbols
{
    public interface IPropertySymbol : IMemberSymbol
    {
        ITypeSymbol Type { get; }

        bool IsStatic { get; }

        IMethodSymbol? GetterMethod { get; }
        IMethodSymbol? SetterMethod { get; }
    }
}