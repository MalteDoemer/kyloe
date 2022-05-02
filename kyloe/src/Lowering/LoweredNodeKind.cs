namespace Kyloe.Lowering
{
    public enum LoweredNodeKind
    {
        LoweredCompilationUnit,
        LoweredFunctionDefinition,
        LoweredArguments,


        LoweredLiteralExpression,
        LoweredBinaryExpression,
        LoweredUnaryExpression,
        LoweredAssignment,
        LoweredVariableAccessExpression,
        LoweredFunctionAccessExpression,

        LoweredBlockStatement,
        LoweredContinueStatement,
        LoweredBreakStatement,
        LoweredReturnStatement,
        LoweredWhileStatement,
        LoweredExpressionStatement,
        LoweredDeclarationStatement,
        LoweredEmptyStatement,
        LoweredIfStatement,
        
        LoweredGotoStatement,
        LoweredConditionalGotoStatement,
        LoweredLabelStatement,
        LoweredCallExpression,
    }
}
