namespace Kyloe.Syntax
{

    // TODO: make SyntaxNodeType internal
    public enum SyntaxNodeType
    {
        // Expressions
        MalformedExpression,
        LiteralExpression,
        UnaryExpression,
        BinarySyntax,
        ParenthesizedExpression,
        IdentifierExpression,
        MemberAccessExpression,
        SubscriptExpression,
        CallSyntax,
        AssignmentSyntax,

        // Statements
        ExpressionStatement,
        DeclarationStatement,
        IfStatement,
        EmptyStatement,
        BlockSyntax,

        // Other
        FunctionDefinition,
        ParameterDeclaration,
        CompilationUnitSyntax,
    }

}