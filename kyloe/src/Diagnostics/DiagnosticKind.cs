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
        ExpectedTypeNameError,
        NotCallableError,
        NoMatchingOverloadError,
        NameAlreadyExistsError, 
        OverloadWithSameParametersExistsError,
        RedefinedParameterError,
        InvalidLiteralError,
        IllegalElseStatement,
        IllegalElifStatement,
        NoExplicitConversionExists,

        // TODO: tests
        InvalidExpressionStatement,
        IllegalReturnStatement,
        IllegalContinueStatement,
        IllegalBreakStatement,
        MissingMainFunction,
        UnableToReadLibrary,
        UnresolvedImport,
        ImportedNameAlreadyExists,
    }
}