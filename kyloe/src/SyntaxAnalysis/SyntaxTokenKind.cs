namespace Kyloe.Syntax
{
    public enum SyntaxTokenKind
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

        // Brackets
        RightParen,
        LeftParen,
        RightSquare,
        LeftSquare,
        RightCurly,
        LeftCurly,

        // Punctuation
        Comma,
        Dot,
        Colon,
        SemiColon,
        SmallArrow,

        // Keywords
        VarKeyword,
        ConstKeyword,
        IfKeyword,
        ElseKeyword,
        FuncKeyword,
    }
}