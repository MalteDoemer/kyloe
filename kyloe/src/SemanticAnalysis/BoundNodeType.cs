namespace Kyloe.Semantics
{
    public enum BoundNodeType
    {
        BoundLiteralExpression,
        BoundBinaryExpression,
        BoundExpressionStatement,
        BoundInvalidExpression,
        BoundUnaryExpression,
        BoundEmptyStatement,
        BoundBlockStatement,
        BoundIfStatement,
        BoundDeclarationStatement,
        BoundAssignmentExpression,
        BoundMemberAccessExpression,
        BoundInvalidMemberAccessExpression,
        BoundSymbolExpression,
        BoundInvalidCallExpression,
        BoundCallExpression,
    }
}