namespace Kyloe.Syntax
{
    public static class SyntaxInfo
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

        public static bool IsOpenBracket(this SyntaxTokenType type)
        {
            switch (type)
            {
                case SyntaxTokenType.LeftCurly:
                case SyntaxTokenType.LeftSquare:
                case SyntaxTokenType.LeftParen:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsCloseBracket(this SyntaxTokenType type)
        {
            switch (type)
            {
                case SyntaxTokenType.RightCurly:
                case SyntaxTokenType.RightSquare:
                case SyntaxTokenType.RightParen:
                    return true;
                default:
                    return false;
            }
        }

        public static SyntaxTokenType GetCorrespondingBracket(SyntaxTokenType type)
        {
            switch (type)
            {
                case SyntaxTokenType.LeftCurly:
                    return SyntaxTokenType.RightCurly;
                case SyntaxTokenType.LeftSquare:
                    return SyntaxTokenType.RightSquare;
                case SyntaxTokenType.LeftParen:
                    return SyntaxTokenType.RightParen;
                case SyntaxTokenType.RightCurly:
                    return SyntaxTokenType.LeftCurly;
                case SyntaxTokenType.RightSquare:
                    return SyntaxTokenType.LeftSquare;
                case SyntaxTokenType.RightParen:
                    return SyntaxTokenType.LeftParen;

                default:
                    throw new System.Exception($"type was not a bracket: {type}");
            }
        }

        public const int MAX_PRECEDENCE = 9;

        // simmilar to https://en.cppreference.com/w/cpp/language/operator_precedence
        public static int BinaryOperatorPrecedence(this SyntaxTokenType type)
        {
            switch (type)
            {
                case SyntaxTokenType.Star:
                case SyntaxTokenType.Slash:
                case SyntaxTokenType.Percent:
                    return 1;

                case SyntaxTokenType.Plus:
                case SyntaxTokenType.Minus:
                    return 2;

                case SyntaxTokenType.Less:
                case SyntaxTokenType.Greater:
                case SyntaxTokenType.LessEqual:
                case SyntaxTokenType.GreaterEqual:
                    return 3;

                case SyntaxTokenType.NotEqual:
                case SyntaxTokenType.DoubleEqual:
                    return 4;

                case SyntaxTokenType.Ampersand:
                    return 5;

                case SyntaxTokenType.Hat:
                    return 6;

                case SyntaxTokenType.Pipe: return 7;

                case SyntaxTokenType.DoubleAmpersand:
                    return 8;

                case SyntaxTokenType.DoublePipe:
                    return 9;

                default:
                    return -1;
            }
        }

        public static bool IsBinaryOperator(this SyntaxTokenType type)
        {
            return type.BinaryOperatorPrecedence() != -1;
        }

        public static bool IsPrefixOperator(this SyntaxTokenType type)
        {
            switch (type)
            {
                case SyntaxTokenType.Bang:
                case SyntaxTokenType.Tilde:
                case SyntaxTokenType.Minus:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsPostfixOperator(this SyntaxTokenType type)
        {
            switch (type)
            {
                case SyntaxTokenType.LeftSquare:
                case SyntaxTokenType.LeftParen:
                case SyntaxTokenType.Dot:
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsAssignmentOperator(this SyntaxTokenType type)
        {
            switch (type)
            {
                case SyntaxTokenType.Equals:
                case SyntaxTokenType.PlusEquals:
                case SyntaxTokenType.MinusEquals:
                case SyntaxTokenType.StarEquals:
                case SyntaxTokenType.SlashEquals:
                case SyntaxTokenType.PercentEquals:
                case SyntaxTokenType.AmpersandEquals:
                case SyntaxTokenType.PipeEquals:
                case SyntaxTokenType.HatEquals:
                    return true;

                default:
                    return false;
            }
        }

        /// This function returns the string that
        /// corresponds to the SyntaxTokenType.
        /// It returns null for more complex tokens such as IntLiteral
        public static string? GetTokenTypeString(SyntaxTokenType type)
        {
            switch (type)
            {
                case SyntaxTokenType.Equals:
                    return "=";
                case SyntaxTokenType.PlusEquals:
                    return "+=";
                case SyntaxTokenType.MinusEquals:
                    return "-=";
                case SyntaxTokenType.StarEquals:
                    return "*=";
                case SyntaxTokenType.SlashEquals:
                    return "/=";
                case SyntaxTokenType.PercentEquals:
                    return "%=";
                case SyntaxTokenType.AmpersandEquals:
                    return "&=";
                case SyntaxTokenType.PipeEquals:
                    return "|=";
                case SyntaxTokenType.HatEquals:
                    return "^=";
                case SyntaxTokenType.Less:
                    return "<";
                case SyntaxTokenType.Greater:
                    return ">";
                case SyntaxTokenType.DoubleEqual:
                    return "==";
                case SyntaxTokenType.LessEqual:
                    return "<=";
                case SyntaxTokenType.GreaterEqual:
                    return ">=";
                case SyntaxTokenType.NotEqual:
                    return "!=";
                case SyntaxTokenType.Plus:
                    return "+";
                case SyntaxTokenType.Minus:
                    return "-";
                case SyntaxTokenType.Star:
                    return "*";
                case SyntaxTokenType.Slash:
                    return "/";
                case SyntaxTokenType.Percent:
                    return "%";
                case SyntaxTokenType.Ampersand:
                    return "&";
                case SyntaxTokenType.DoubleAmpersand:
                    return "&&";
                case SyntaxTokenType.Pipe:
                    return "|";
                case SyntaxTokenType.DoublePipe:
                    return "||";
                case SyntaxTokenType.Hat:
                    return "^";
                case SyntaxTokenType.Tilde:
                    return "~";
                case SyntaxTokenType.Bang:
                    return "!";
                case SyntaxTokenType.RightParen:
                    return ")";
                case SyntaxTokenType.LeftParen:
                    return "(";
                case SyntaxTokenType.RightSquare:
                    return "]";
                case SyntaxTokenType.LeftSquare:
                    return "[";
                case SyntaxTokenType.RightCurly:
                    return "}";
                case SyntaxTokenType.LeftCurly:
                    return "{";
                case SyntaxTokenType.Comma:
                    return ",";
                case SyntaxTokenType.Dot:
                    return ".";
                case SyntaxTokenType.Colon:
                    return ":";
                case SyntaxTokenType.SemiColon:
                    return ";";
                case SyntaxTokenType.SmallArrow:
                    return "->";
                default:
                    return null;
            }
        }
    }
}