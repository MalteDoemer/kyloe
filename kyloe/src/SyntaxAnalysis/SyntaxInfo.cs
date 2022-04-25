namespace Kyloe.Syntax
{
    public static class SyntaxInfo
    {   /// This function returns the string that
        /// corresponds to the SyntaxTokenType.
        /// It returns null for more complex tokens such as IntLiteral
        /* public static string? GetSimpleTokenString(SyntaxTokenKind kind)
        {
            switch (kind)
            {
                case SyntaxTokenKind.Equals:
                    return "=";
                case SyntaxTokenKind.PlusEquals:
                    return "+=";
                case SyntaxTokenKind.MinusEquals:
                    return "-=";
                case SyntaxTokenKind.StarEquals:
                    return "*=";
                case SyntaxTokenKind.SlashEquals:
                    return "/=";
                case SyntaxTokenKind.PercentEquals:
                    return "%=";
                case SyntaxTokenKind.AmpersandEquals:
                    return "&=";
                case SyntaxTokenKind.PipeEquals:
                    return "|=";
                case SyntaxTokenKind.HatEquals:
                    return "^=";
                case SyntaxTokenKind.Less:
                    return "<";
                case SyntaxTokenKind.Greater:
                    return ">";
                case SyntaxTokenKind.DoubleEqual:
                    return "==";
                case SyntaxTokenKind.LessEqual:
                    return "<=";
                case SyntaxTokenKind.GreaterEqual:
                    return ">=";
                case SyntaxTokenKind.NotEqual:
                    return "!=";
                case SyntaxTokenKind.Plus:
                    return "+";
                case SyntaxTokenKind.Minus:
                    return "-";
                case SyntaxTokenKind.Star:
                    return "*";
                case SyntaxTokenKind.Slash:
                    return "/";
                case SyntaxTokenKind.Percent:
                    return "%";
                case SyntaxTokenKind.Ampersand:
                    return "&";
                case SyntaxTokenKind.DoubleAmpersand:
                    return "&&";
                case SyntaxTokenKind.Pipe:
                    return "|";
                case SyntaxTokenKind.DoublePipe:
                    return "||";
                case SyntaxTokenKind.Hat:
                    return "^";
                case SyntaxTokenKind.Tilde:
                    return "~";
                case SyntaxTokenKind.Bang:
                    return "!";
                case SyntaxTokenKind.RightParen:
                    return ")";
                case SyntaxTokenKind.LeftParen:
                    return "(";
                case SyntaxTokenKind.RightSquare:
                    return "]";
                case SyntaxTokenKind.LeftSquare:
                    return "[";
                case SyntaxTokenKind.RightCurly:
                    return "}";
                case SyntaxTokenKind.LeftCurly:
                    return "{";
                case SyntaxTokenKind.Comma:
                    return ",";
                case SyntaxTokenKind.Dot:
                    return ".";
                case SyntaxTokenKind.Colon:
                    return ":";
                case SyntaxTokenKind.SemiColon:
                    return ";";
                case SyntaxTokenKind.SmallArrow:
                    return "->";
                default:
                    return null;
            }
        }

        public static object? GetDefaultValue(SyntaxTokenKind kind)
        {
            switch (kind)
            {
                case SyntaxTokenKind.Invalid:
                    return '\0';
                case SyntaxTokenKind.IntLiteral:
                    return 0;
                case SyntaxTokenKind.FloatLiteral:
                    return 0.0;
                case SyntaxTokenKind.BoolLiteral:
                    return false;
                case SyntaxTokenKind.StringLiteral:
                    return string.Empty;
                case SyntaxTokenKind.Identifier:
                    return string.Empty;
                default:
                    return null;
            }
        }

        public static string TokenKindString(this SyntaxTokenKind kind)
        {
            if (GetSimpleTokenString(kind) is string s)
                return s;

            if  (System.Enum.GetName(kind) is string n)
                return n;

            return string.Empty;
        }*/

    }
}