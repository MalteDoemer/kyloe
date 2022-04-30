namespace Kyloe.Diagnostics
{
    public enum DiagnosticKind
    {
        // Syntax Errors
        UnexpectedTokenError,
        InvalidCharacterError,

        // Semantic Errors
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
        InvalidLiteralError,            // TODO: add a test
        IllegalElseStatement,
        IllegalElifStatement,
        IllegalReturnStatement,
    }
}