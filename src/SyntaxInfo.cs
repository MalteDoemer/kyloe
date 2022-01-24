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
    }
}