namespace Kyloe.Syntax
{

    // TODO: make SyntaxNodeType internal
    public enum SyntaxNodeType
    {
        // Expressions
        MalformedExpression,
        LiteralExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression,
        NameExpression,
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
    }

}