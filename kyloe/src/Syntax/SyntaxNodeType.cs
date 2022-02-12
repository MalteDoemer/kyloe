namespace Kyloe.Syntax
{

    // TODO: make SyntaxNodeType internal
    public enum SyntaxNodeType
    {
        MalformedExpression,
        LiteralExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression,
        NameExpression,
        MemberAccessExpression,
        SubscriptExpression,
        CallExpression,
        ArgumentExpression,
    }

}