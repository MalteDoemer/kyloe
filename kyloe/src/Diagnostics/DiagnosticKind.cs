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
        InvalidExpressionStatement,

        // TODO: tests
        IllegalReturnStatement,
        IllegalContinueStatement,
        IllegalBreakStatement,
        MissingMainFunction,
        UnableToReadLibrary,
        UnresolvedImport,
        ImportedNameAlreadyExists,
    }
}