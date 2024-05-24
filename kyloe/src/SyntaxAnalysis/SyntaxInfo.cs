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
                case SyntaxTokenKind.FuncKeyword:
                case SyntaxTokenKind.IfKeyword:
                case SyntaxTokenKind.ElseKeyword:
                case SyntaxTokenKind.ElifKeyword:
                case SyntaxTokenKind.WhileKeyword:
                case SyntaxTokenKind.ForKeyword:
                case SyntaxTokenKind.BreakKeyword:
                case SyntaxTokenKind.ContinueKeyword:
                case SyntaxTokenKind.ReturnKeyword:
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
                case SyntaxTokenKind.FuncKeyword:
                    return "func";
                case SyntaxTokenKind.IfKeyword:
                    return "if";
                case SyntaxTokenKind.ElseKeyword:
                    return "else";
                case SyntaxTokenKind.ElifKeyword:
                    return "elif";
                case SyntaxTokenKind.WhileKeyword:
                    return "while";
                case SyntaxTokenKind.ForKeyword:
                    return "for";
                case SyntaxTokenKind.BreakKeyword:
                    return "break";
                case SyntaxTokenKind.ContinueKeyword:
                    return "continue";
                case SyntaxTokenKind.ReturnKeyword:
                    return "return";
                default:
                    return null;
            }
        }

        public static string GetSymbolOrName(this SyntaxTokenKind kind)
        {
            var sym = GetSimpleTerminalString(kind);

            if (sym is not null)
                return sym;

            return kind.ToString();
        }
    }
}