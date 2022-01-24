namespace Kyloe
{
    static class SyntaxInfo
    {
        public static bool IsIdentStartChar(char c)
        {
            return c == '_' || char.IsLetter(c);
        }

        public static bool IsIdentSubsequentChar(char c)
        {
            return c == '_' || char.IsLetterOrDigit(c);
        }

        public static bool IsStringQuote(char c)
        {
            return c == '\'' || c == '"';
        }

        public static SyntaxToken? IsKeyword(string ident)
        {
            switch (ident)
            {
                case "true":
                    return new SyntaxToken(SyntaxTokenType.BoolLiteral, true);
                case "false":
                    return new SyntaxToken(SyntaxTokenType.BoolLiteral, false);
                default:
                    return null;
            }
        }

        public static bool IsLiteralToken(this SyntaxTokenType type)
        {
            switch (type)
            {
                case SyntaxTokenType.IntLiteral:
                case SyntaxTokenType.FloatLiteral:
                case SyntaxTokenType.StringLiteral:
                case SyntaxTokenType.BoolLiteral:
                    return true;
                default:
                    return false;
            }
        }
    }
}