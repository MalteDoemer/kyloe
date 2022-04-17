namespace Kyloe.Syntax
{
    public enum SyntaxNodeKind
    {
        // Expressions
        MalformedExpression,
        LiteralExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression,
        IdentifierExpression,
        MemberAccessExpression,
        SubscriptExpression,
        CallExpression,
        AssignmentExpression,

        // Statements
        ExpressionStatement,
        DeclarationStatement,
        IfStatement,
        EmptyStatement,
        BlockStatement,

        // Other
        FunctionDefinition,
        ParameterDeclaration,
        CompilationUnitSyntax,
    }

}