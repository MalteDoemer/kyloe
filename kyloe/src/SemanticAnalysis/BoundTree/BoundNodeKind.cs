namespace Kyloe.Semantics
{
    public enum BoundNodeKind
    {
        BoundCompilationUnit,
        BoundFunctionDefinition,

        BoundBlockStatement,
        BoundEmptyStatement,
        BoundIfStatement,
        BoundDeclarationStatement,
        BoundExpressionStatement,
        BoundInvalidStatement,
        BoundWhileStatement,

        BoundLiteralExpression,
        BoundBinaryExpression,
        BoundInvalidExpression,
        BoundUnaryExpression,
        BoundAssignmentExpression,
        BoundSymbolExpression,
        BoundFunctionCallExpression,
    }
}