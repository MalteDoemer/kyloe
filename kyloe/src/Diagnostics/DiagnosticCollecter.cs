using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Kyloe.Semantics;
using Kyloe.Symbols;
using Kyloe.Syntax;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    public sealed class DiagnosticCollector : ICollection<Diagnostic>
    {
        private readonly ImmutableArray<Diagnostic>.Builder diagnostics;

        public DiagnosticCollector()
        {
            diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();
        }

        public bool HasErrors() => diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error);
        
        public bool HasWarnings() => diagnostics.Any(d => d.Severity == DiagnosticSeverity.Warn);

        public int Count => diagnostics.Count;

        public bool IsReadOnly => ((ICollection<Diagnostic>)diagnostics).IsReadOnly;

        public void Add(Diagnostic item)
        {
            diagnostics.Add(item);
        }

        public void AddError(DiagnosticKind kind, string message, SourceLocation? location)
        {
            Add(new Diagnostic(DiagnosticSeverity.Error, kind, message, location));
        }

        internal void InvalidLiteralError(SourceLocation location)
        {
            var msg = "invalid literal";
            AddError(DiagnosticKind.InvalidLiteralError, msg, location);
        }

        internal void ExpectedModifiableValueError(SourceLocation location)
        {
            var msg = $"expected a modifiable value";
            AddError(DiagnosticKind.ExpectedModifiableValueError, msg, location);
        }

        internal void ExpectedValueError(SourceLocation location)
        {
            var msg = $"expected a value";
            AddError(DiagnosticKind.ExpectedValueError, msg, location);
        }

        internal void ExpectedTypeNameError(SourceLocation location)
        {
            var msg = $"expected a type name";
            AddError(DiagnosticKind.ExpectedTypeNameError, msg, location);
        }

        internal void InvalidCharacterError(SourceLocation location, char character)
        {
            var msg = string.Format("invalid character: \\u{0:x4}", (int)(character));
            AddError(DiagnosticKind.InvalidCharacterError, msg, location);
        }

        internal void UnexpectedTokenError(SyntaxToken actual, SyntaxTokenKind[] expected)
        {
            string msg;

            if (expected.Length == 1)
                msg = $"expected {expected[0].GetSymbolOrName()}, got {actual.Kind.GetSymbolOrName()}";
            else
                msg = $"expected one of ({string.Join(", ", expected.Select(t => t.GetSymbolOrName()))}), got {actual.Kind.GetSymbolOrName()}";

            AddError(DiagnosticKind.UnexpectedTokenError, msg, actual.Location);
        }

        internal void UnsupportedBinaryOperation(SourceLocation location, BoundOperation operation, TypeInfo left, TypeInfo right)
        {
            var msg = $"binary operator '{operation.GetSymbolOrName()}' cannot be used with types '{left.FullName()}' and '{right.FullName()}'";
            AddError(DiagnosticKind.UnsupportedBinaryOperation, msg, location);
        }

        internal void UnsupportedUnaryOperation(SourceLocation location, BoundOperation operation, TypeInfo type)
        {
            var msg = $"unary operator '{operation.GetSymbolOrName()}' cannot be used with type '{type.FullName()}'";
            AddError(DiagnosticKind.UnsupportedUnaryOperation, msg, location);
        }

        internal void UnsupportedAssignmentOperation(SourceLocation location, AssignmentOperation operation, TypeInfo left, TypeInfo right)
        {
            var msg = $"assignment operator '{operation.GetSymbolOrName()}' cannot be used with types '{left.FullName()}' and '{right.FullName()}'";
            AddError(DiagnosticKind.UnsupportedAssignmentOperation, msg, location);
        }

        internal void MissmatchedTypeError(SourceLocation location, TypeInfo expected, TypeInfo provided)
        {
            var msg = $"missmatched types, expected '{expected.FullName()}', got '{provided.FullName()}'";
            AddError(DiagnosticKind.MissmatchedTypeError, msg, location);
        }

        internal void NonExistantNameError(SourceLocation location, string name)
        {
            var msg = $"the name '{name}' does not exist in the current scope";
            AddError(DiagnosticKind.NonExistantNameError, msg, location);
        }

        internal void NameAlreadyExistsError(SourceLocation location, string name)
        {
            var msg = $"the name '{name}' already exists";
            AddError(DiagnosticKind.NameAlreadyExistsError, msg, location);
        }

        internal void RedefinedParameterError(SourceLocation location, string name)
        {
            var msg = $"the parameter '{name}' already exists";
            AddError(DiagnosticKind.RedefinedParameterError, msg, location);
        }

        internal void NotCallableError(SourceLocation location)
        {
            var msg = $"expression is not callable";
            AddError(DiagnosticKind.NotCallableError, msg, location);
        }

        internal void NoMatchingOverloadError(SourceLocation location, string name, BoundArguments arguments)
        {
            string msg;
            if (arguments.Arguments.Length == 0)
                msg = $"cannot call '{name}' without arguments";
            else
                msg = $"cannot call '{name}' with arguments ({string.Join(", ", arguments.ArgumentTypes.Select(a  => a.FullName()))})";

            AddError(DiagnosticKind.NoMatchingOverloadError, msg, location);
        }

        internal void NoExplicitConversionExists(SourceLocation location, TypeInfo from, TypeInfo to) 
        {
            var msg = $"cannot explicitly convert '{from.FullName()}' to '{to.FullName()}'";
            AddError(DiagnosticKind.NoExplicitConversionExists, msg, location);
        }

        internal void OverloadWithSameParametersExistsError(SourceLocation location, string name)
        {
            var msg = $"the function '{name}' already exists with the same parameters";
            AddError(DiagnosticKind.OverloadWithSameParametersExistsError, msg, location);
        }

        internal void IllegalElifStatement(SourceLocation location)
        {
            var msg = "unexpected elif statement";
            AddError(DiagnosticKind.IllegalElifStatement, msg, location);
        }

        internal void IllegalElseStatement(SourceLocation location)
        {
            var msg = "unexpected else statement";
            AddError(DiagnosticKind.IllegalElseStatement, msg, location);
        }

        internal void IllegalReturnStatement(SourceLocation location)
        {
            var msg = "illeagal return statement outside a function";
            AddError(DiagnosticKind.IllegalReturnStatement, msg, location);
        }

        internal void IllegalBreakStatement(SourceLocation location)
        {
            var msg = "illegal break statement outside a loop";
            AddError(DiagnosticKind.IllegalBreakStatement, msg, location);
        }

        internal void IllegalContinueStatement(SourceLocation location)
        {
            var msg = "illegal continue statement outside a loop";
            AddError(DiagnosticKind.IllegalContinueStatement, msg, location);
        }

        internal void MissingMainFunction()
        {
            var msg = "the program does not contain a suitable 'main' function";
            AddError(DiagnosticKind.MissingMainFunction, msg, null);
        }

        internal void UnableToReadLibrary(string name) 
        {
            var msg = $"unable to read library file'{name}'";
            AddError(DiagnosticKind.UnableToReadLibrary, msg, null);
        }

        internal void UnresolvedImport(string name, SourceLocation? location) 
        {
            var msg = $"unable to resolve import '{name}'";
            AddError(DiagnosticKind.UnresolvedImport, msg, location);
        }

        internal void ImportedNameAlreadyExists(string name, SourceLocation? location) 
        {
            var msg = $"unable to import '{name}', the same name already exists";
            AddError(DiagnosticKind.ImportedNameAlreadyExists, msg, location);
        }

        public void Clear()
        {
            diagnostics.Clear();
        }

        public bool Contains(Diagnostic item)
        {
            return diagnostics.Contains(item);
        }

        public void CopyTo(Diagnostic[] array, int arrayIndex)
        {
            diagnostics.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Diagnostic> GetEnumerator()
        {
            return ((IEnumerable<Diagnostic>)diagnostics).GetEnumerator();
        }

        public bool Remove(Diagnostic item)
        {
            return diagnostics.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)diagnostics).GetEnumerator();
        }

        public DiagnosticResult ToResult()
        {
            return new DiagnosticResult(diagnostics.ToImmutable());
        }
    }
}