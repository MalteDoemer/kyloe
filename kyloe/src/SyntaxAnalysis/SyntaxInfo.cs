namespace Kyloe.Syntax
{
    public static class SyntaxInfo
    {

        /// <summary>
        /// Check whether kind is a keyword.
        /// </summary>
        public static bool IsKeyword(this SyntaxTokenKind kind)
        {
            switch (kind)
            {
                case SyntaxTokenKind.VarKeyword:
                case SyntaxTokenKind.ConstKeyword:
                case SyntaxTokenKind.IfKeyword:
                case SyntaxTokenKind.ElseKeyword:
                case SyntaxTokenKind.FuncKeyword:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// For terminals that only have one single string representation, this string will be returned.
        /// For more compelx terminals, null will be returned.
        /// </summary>
        public static string? GetSimpleTerminalString(SyntaxTokenKind kind)
        {
            switch (kind)
            {
                case SyntaxTokenKind.Equal:
                    return "=";
                case SyntaxTokenKind.PlusEqual:
                    return "+=";
                case SyntaxTokenKind.MinusEqual:
                    return "-=";
                case SyntaxTokenKind.StarEqual:
                    return "*=";
                case SyntaxTokenKind.SlashEqual:
                    return "/=";
                case SyntaxTokenKind.PercentEqual:
                    return "%=";
                case SyntaxTokenKind.AmpersandEqual:
                    return "&=";
                case SyntaxTokenKind.PipeEqual:
                    return "|=";
                case SyntaxTokenKind.HatEqual:
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
                case SyntaxTokenKind.VarKeyword:
                    return "var";
                case SyntaxTokenKind.ConstKeyword:
                    return "const";
                case SyntaxTokenKind.IfKeyword:
                    return "if";
                case SyntaxTokenKind.ElseKeyword:
                    return "else";
                case SyntaxTokenKind.FuncKeyword:
                    return "func";
                default:
                    return null;
            }
        }

        /*

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