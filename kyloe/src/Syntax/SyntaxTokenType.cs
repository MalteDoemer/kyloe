namespace Kyloe.Syntax
{
    // TODO: make SyntaxTokenType internal
    public enum SyntaxTokenType
    {
        // Special Tokens
        Invalid = 0,
        End,

        // Literal Tokens
        IntLiteral,
        FloatLiteral,
        BoolLiteral,
        StringLiteral,

        Identifier,

        // Assignment Operators
        Equals,
        PlusEquals,
        MinusEquals,
        StarEquals,
        SlashEquals,
        PercentEquals,
        AmpersandEquals,
        PipeEquals,
        HatEquals,

        // Comparison Operators
        Less,
        Greater,
        DoubleEqual,
        LessEqual,
        GreaterEqual,
        NotEqual,

        // Binary Operators
        Plus,
        Minus,
        Star,
        Slash,
        Percent,
        Ampersand,
        DoubleAmpersand,
        Pipe,
        DoublePipe,
        Hat,

        // Unary Operators
        Tilde,
        Bang,

        // Parentheses
        RightParen,
        LeftParen,
        RightSquare,
        LeftSquare,
        RightBracket,
        LeftBracket,

        // Punctuation
        Comma,
        Dot,
        Colon,
        SemiColon,
        SmallArrow,

        // Keywords
        VarKeyword,
        ConstKeyword,
    }
}