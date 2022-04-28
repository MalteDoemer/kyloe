namespace Kyloe.Semantics
{
    public enum BoundNodeKind
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
        BoundInvalidStatement,
    }
}