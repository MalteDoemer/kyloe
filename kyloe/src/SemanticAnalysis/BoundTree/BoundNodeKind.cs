namespace Kyloe.Semantics
{
    public enum BoundNodeKind
    {
        BoundCompilationUnit,
        BoundFunctionDeclaration,
        BoundFunctionDefinition,
        BoundTypeClause,
        BoundParameters,
        BoundParameterDeclaration,
        BoundArguments,

        BoundBlockStatement,
        BoundEmptyStatement,
        BoundIfStatement,
        BoundDeclarationStatement,
        BoundExpressionStatement,
        BoundInvalidStatement,
        BoundWhileStatement,
        BoundReturnStatement,
        BoundContinueStatement,
        BoundBreakStatement,

        BoundLiteralExpression,
        BoundBinaryExpression,
        BoundParenthesizedExpression,
        BoundInvalidExpression,
        BoundUnaryExpression,
        BoundAssignmentExpression,
        BoundSymbolExpression,
        BoundCallExpression,
        BoundConversionExpression,
        BoundForStatement,
    }
}