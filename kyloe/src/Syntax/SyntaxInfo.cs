namespace Kyloe.Syntax
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
    }
}