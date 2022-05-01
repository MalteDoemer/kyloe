namespace Kyloe.Semantics
{
    public enum BoundNodeKind
    {
        BoundCompilationUnit,
        BoundFunctionDeclaration,
        BoundFunctionDefinition,
        BoundParameters,
        BoundTypeClause,
        BoundArguments,

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
        BoundReturnStatement,

    }
}