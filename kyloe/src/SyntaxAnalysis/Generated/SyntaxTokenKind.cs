
namespace Kyloe.Syntax
{
    public enum SyntaxTokenKind : int
    {
        End = -1,
        Error = -2,
        Epsilon = -3,
        Whitespace = -4,
        LineComment = -5,
        BlockComment = -6,
        Comma = -7,
        Dot = -8,
        Colon = -9,
        SemiColon = -10,
        SmallArrow = -11,
        LessEqual = -12,
        Less = -13,
        GreaterEqual = -14,
        Greater = -15,
        DoubleEqual = -16,
        NotEqual = -17,
        PlusEqual = -18,
        MinusEqual = -19,
        StarEqual = -20,
        SlashEqual = -21,
        PercentEqual = -22,
        AmpersandEqual = -23,
        PipeEqual = -24,
        HatEqual = -25,
        Equal = -26,
        Plus = -27,
        Minus = -28,
        Star = -29,
        Slash = -30,
        Percent = -31,
        DoubleAmpersand = -32,
        Ampersand = -33,
        DoublePipe = -34,
        Pipe = -35,
        Hat = -36,
        Tilde = -37,
        Bang = -38,
        LeftParen = -39,
        RightParen = -40,
        LeftSquare = -41,
        RightSquare = -42,
        LeftCurly = -43,
        RightCurly = -44,
        Float = -45,
        Int = -46,
        Bool = -47,
        String = -48,
        VarKeyword = -49,
        ConstKeyword = -50,
        FuncKeyword = -51,
        IfKeyword = -52,
        ElseKeyword = -53,
        ElifKeyword = -54,
        WhileKeyword = -55,
        ForKeyword = -56,
        BreakKeyword = -57,
        ContinueKeyword = -58,
        ReturnKeyword = -59,
        Identifier = -60,
        Discard = 1,
        Start = 2,
        Stop = 3,
        CompilationUnit = 4,
        TopLevelItem = 5,
        FunctionDefinition = 6,
        TrailingTypeClause = 7,
        OptionalParameters = 8,
        Parameters = 9,
        ParameterDeclaration = 10,
        TypeClause = 11,
        OptionalTypeClause = 12,
        Statement = 13,
        ExpressionStatement = 14,
        BlockStatement = 15,
        RepeatedStatement = 16,
        IfStatement = 17,
        ElifClause = 18,
        ElifStatement = 19,
        WhileStatement = 20,
        ForStatement = 21,
        BreakStatement = 22,
        ContinueStatement = 23,
        ReturnStatement = 24,
        DeclarationStatement = 25,
        Expression = 26,
        OptionalExpression = 27,
        AssignmentHelper = 28,
        Assignment = 29,
        LogicalOr = 30,
        LogicalAnd = 31,
        BitOr = 32,
        BitXor = 33,
        BitAnd = 34,
        Equality = 35,
        Comparison = 36,
        Sum = 37,
        Mult = 38,
        Prefix = 39,
        Postfix = 40,
        OptionalArguments = 41,
        Arguments = 42,
        Primary = 43,
        Parenthesized = 44,
    }
}
