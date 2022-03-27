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
        BoundSymbolExpression,
        BoundInvalidCallExpression,
        BoundCallExpression,
        BoundFunctionDefinition,
        BoundCompilationUnit,
    }
}