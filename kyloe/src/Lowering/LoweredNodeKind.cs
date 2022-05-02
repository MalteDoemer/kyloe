namespace Kyloe.Lowering
{
    public enum LoweredNodeKind
    {
        LoweredCompilationUnit,
        LoweredFunctionDefinition,


        LoweredLiteralExpression,
        LoweredBinaryExpression,
        LoweredUnaryExpression,
        LoweredAssignment,
        LoweredVariableAccessExpression,

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
    }
}
