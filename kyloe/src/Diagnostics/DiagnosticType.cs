namespace Kyloe.Diagnostics
{
    public enum DiagnosticKind
    {
        NeverClosedBlockCommentError,
        NeverClosedStringLiteralError,
        ExpectedTokenError,
        UnknownCharacterError,
        InvalidIntLiteralError,
        InvalidFloatLiteralError,
        ExpectedExpressionError,
        ExpectedValueError,
        UnsupportedBinaryOperation,
        UnsupportedUnaryOperation,
        MissmatchedTypeError,
        RedefinedLocalVariableError,
        NonExistantNameError,
        ExpectedModifiableValueError,
        UnsupportedAssignmentOperation,
        ExpectedTypeNameError,
        MemberNotFoundError,
        MemberAccessError,
        MemberAccessNotAllowed
    }
}