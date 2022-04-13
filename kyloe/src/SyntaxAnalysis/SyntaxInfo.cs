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

        public static bool IsLiteralToken(this SyntaxTokenKind kind)
        {
            switch (kind)
            {
                case SyntaxTokenKind.IntLiteral:
                case SyntaxTokenKind.FloatLiteral:
                case SyntaxTokenKind.StringLiteral:
                case SyntaxTokenKind.BoolLiteral:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsOpenBracket(this SyntaxTokenKind kind)
        {
            switch (kind)
            {
                case SyntaxTokenKind.LeftCurly:
                case SyntaxTokenKind.LeftSquare:
                case SyntaxTokenKind.LeftParen:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsCloseBracket(this SyntaxTokenKind kind)
        {
            switch (kind)
            {
                case SyntaxTokenKind.RightCurly:
                case SyntaxTokenKind.RightSquare:
                case SyntaxTokenKind.RightParen:
                    return true;
                default:
                    return false;
            }
        }

        public static SyntaxTokenKind GetCorrespondingBracket(SyntaxTokenKind kind)
        {
            switch (kind)
            {
                case SyntaxTokenKind.LeftCurly:
                    return SyntaxTokenKind.RightCurly;
                case SyntaxTokenKind.LeftSquare:
                    return SyntaxTokenKind.RightSquare;
                case SyntaxTokenKind.LeftParen:
                    return SyntaxTokenKind.RightParen;
                case SyntaxTokenKind.RightCurly:
                    return SyntaxTokenKind.LeftCurly;
                case SyntaxTokenKind.RightSquare:
                    return SyntaxTokenKind.LeftSquare;
                case SyntaxTokenKind.RightParen:
                    return SyntaxTokenKind.LeftParen;

                default:
                    throw new System.Exception($"kind was not a bracket: {kind}");
            }
        }

        public const int MAX_PRECEDENCE = 9;

        // simmilar to https://en.cppreference.com/w/cpp/language/operator_precedence
        public static int BinaryOperatorPrecedence(this SyntaxTokenKind kind)
        {
            switch (kind)
            {
                case SyntaxTokenKind.Star:
                case SyntaxTokenKind.Slash:
                case SyntaxTokenKind.Percent:
                    return 1;

                case SyntaxTokenKind.Plus:
                case SyntaxTokenKind.Minus:
                    return 2;

                case SyntaxTokenKind.Less:
                case SyntaxTokenKind.Greater:
                case SyntaxTokenKind.LessEqual:
                case SyntaxTokenKind.GreaterEqual:
                    return 3;

                case SyntaxTokenKind.NotEqual:
                case SyntaxTokenKind.DoubleEqual:
                    return 4;

                case SyntaxTokenKind.Ampersand:
                    return 5;

                case SyntaxTokenKind.Hat:
                    return 6;

                case SyntaxTokenKind.Pipe: return 7;

                case SyntaxTokenKind.DoubleAmpersand:
                    return 8;

                case SyntaxTokenKind.DoublePipe:
                    return 9;

                default:
                    return -1;
            }
        }

        public static bool IsBinaryOperator(this SyntaxTokenKind kind)
        {
            return kind.BinaryOperatorPrecedence() != -1;
        }

        public static bool IsPrefixOperator(this SyntaxTokenKind kind)
        {
            switch (kind)
            {
                case SyntaxTokenKind.Bang:
                case SyntaxTokenKind.Tilde:
                case SyntaxTokenKind.Minus:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsPostfixOperator(this SyntaxTokenKind kind)
        {
            switch (kind)
            {
                case SyntaxTokenKind.LeftSquare:
                case SyntaxTokenKind.LeftParen:
                case SyntaxTokenKind.Dot:
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsAssignmentOperator(this SyntaxTokenKind kind)
        {
            switch (kind)
            {
                case SyntaxTokenKind.Equals:
                case SyntaxTokenKind.PlusEquals:
                case SyntaxTokenKind.MinusEquals:
                case SyntaxTokenKind.StarEquals:
                case SyntaxTokenKind.SlashEquals:
                case SyntaxTokenKind.PercentEquals:
                case SyntaxTokenKind.AmpersandEquals:
                case SyntaxTokenKind.PipeEquals:
                case SyntaxTokenKind.HatEquals:
                    return true;

                default:
                    return false;
            }
        }

        /// This function returns the string that
        /// corresponds to the SyntaxTokenType.
        /// It returns null for more complex tokens such as IntLiteral
        public static string? GetTokenKindString(SyntaxTokenKind kind)
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
    

        public static string GetTokenKindStringOrName(SyntaxTokenKind kind) 
        {
            if (GetTokenKindString(kind) is string s)
                return s;

            return kind.ToString();
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
    }
}