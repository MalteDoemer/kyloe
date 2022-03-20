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
        BoundLocalVariableExpression,
        BoundAssignmentExpression,
        BoundTypeNameExpression,
        BoundInvalidMemberAccessExpression,
        BoundTypeNameMemberAccessExpression,
        BoundNamespaceMemberAccessExpression,
        BoundNamespaceExpression,
        BoundFieldMemberAccessExpression,
        BoundPropertyMemberAccessExpression
    }
}