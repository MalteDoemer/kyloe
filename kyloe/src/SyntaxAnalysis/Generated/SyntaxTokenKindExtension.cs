
namespace Kyloe.Syntax
{
    public static class SyntaxTokenKindExtension
    {
        public static bool IsTerminal(this SyntaxTokenKind kind)
        {
            return (int)kind < 0;
        }
        
        public static bool IsNonTerminal(this SyntaxTokenKind kind)
        {
            return (int)kind > 0;
        }
    }
}
