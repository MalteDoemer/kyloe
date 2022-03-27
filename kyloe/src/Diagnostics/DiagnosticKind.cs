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
        NonExistantNameError,
        ExpectedModifiableValueError,
        UnsupportedAssignmentOperation,
        ExpectedTypeNameError,          // TODO: add a test
        NotCallableError,               // TODO: add a test
        NoMatchingOverloadError,
        NameAlreadyExistsError, 
        OverloadWithSameParametersExistsError,
        RedefinedParameterError,
    }
}