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
    internal sealed class DiagnosticCollector : ICollection<Diagnostic>
    {
        private readonly ImmutableArray<Diagnostic>.Builder diagnostics;
        private readonly SourceText sourceText;

        public DiagnosticCollector(SourceText sourceText)
        {
            diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();
            this.sourceText = sourceText;
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

        public void InvalidLiteralError(SourceLocation location)
        {
            var msg = "invalid literal";
            AddError(DiagnosticKind.InvalidLiteralError, msg, location);
        }

        public void ExpectedModifiableValueError(SourceLocation location)
        {
            var msg = $"expected a modifiable value";
            AddError(DiagnosticKind.ExpectedModifiableValueError, msg, location);
        }

        public void ExpectedValueError(SourceLocation location)
        {
            var msg = $"expected a value";
            AddError(DiagnosticKind.ExpectedValueError, msg, location);
        }

        public void ExpectedTypeNameError(SourceLocation location)
        {
            var msg = $"expected a type name";
            AddError(DiagnosticKind.ExpectedTypeNameError, msg, location);
        }

        public void InvalidCharacterError(SourceLocation location, char character)
        {
            var msg = string.Format("invalid character: \\u{0:x4}", (int)(character));
            AddError(DiagnosticKind.InvalidCharacterError, msg, location);
        }

        public void UnexpectedTokenError(SyntaxToken actual, SyntaxTokenKind[] expected)
        {
            string msg;

            if (expected.Length == 1)
                msg = $"expected {expected[0].GetSymbolOrName()}, got {actual.Kind.GetSymbolOrName()}";
            else
                msg = $"expected one of ({string.Join(", ", expected.Select(t => t.GetSymbolOrName()))}), got {actual.Kind.GetSymbolOrName()}";

            AddError(DiagnosticKind.UnexpectedTokenError, msg, actual.Location);
        }

        public void UnsupportedBinaryOperation(SourceLocation location, BoundOperation operation, TypeInfo left, TypeInfo right)
        {
            var msg = $"binary operator '{operation.GetSymbolOrName()}' cannot be used with types '{left.FullName()}' and '{right.FullName()}'";
            AddError(DiagnosticKind.UnsupportedBinaryOperation, msg, location);
        }

        public void UnsupportedUnaryOperation(SourceLocation location, BoundOperation operation, TypeInfo type)
        {
            var msg = $"unary operator '{operation.GetSymbolOrName()}' cannot be used with type '{type.FullName()}'";
            AddError(DiagnosticKind.UnsupportedUnaryOperation, msg, location);
        }

        public void UnsupportedAssignmentOperation(SourceLocation location, AssignmentOperation operation, TypeInfo left, TypeInfo right)
        {
            var msg = $"assignment operator '{operation.GetSymbolOrName()}' cannot be used with types '{left.FullName()}' and '{right.FullName()}'";
            AddError(DiagnosticKind.UnsupportedAssignmentOperation, msg, location);
        }

        public void MissmatchedTypeError(SourceLocation location, TypeInfo expected, TypeInfo provided)
        {
            var msg = $"missmatched types, expected '{expected.FullName()}', got '{provided.FullName()}'";
            AddError(DiagnosticKind.MissmatchedTypeError, msg, location);
        }

        public void NonExistantNameError(SourceLocation location, string name)
        {
            var msg = $"the name '{name}' does not exist in the current scope";
            AddError(DiagnosticKind.NonExistantNameError, msg, location);
        }

        public void NameAlreadyExistsError(SourceLocation location, string name)
        {
            var msg = $"the name '{name}' already exists";
            AddError(DiagnosticKind.NameAlreadyExistsError, msg, location);
        }

        public void RedefinedParameterError(SourceLocation location, string name)
        {
            var msg = $"the parameter '{name}' already exists";
            AddError(DiagnosticKind.RedefinedParameterError, msg, location);
        }

        public void NotCallableError(SourceLocation location)
        {
            var msg = $"expression is not callable";
            AddError(DiagnosticKind.NotCallableError, msg, location);
        }

        public void NoMatchingOverloadError(SourceLocation location, string name, BoundArguments arguments)
        {
            string msg;
            if (arguments.Arguments.Length == 0)
                msg = $"cannot call '{name}' without arguments";
            else
                msg = $"cannot call '{name}' with arguments ({string.Join(", ", arguments.ArgumentTypes.Select(a  => a.FullName()))})";

            AddError(DiagnosticKind.NoMatchingOverloadError, msg, location);
        }

        public void OverloadWithSameParametersExistsError(SourceLocation location, string name)
        {
            var msg = $"the function '{name}' already exists with the same parameters";
            AddError(DiagnosticKind.OverloadWithSameParametersExistsError, msg, location);
        }

        public void IllegalElifStatement(SourceLocation location)
        {
            var msg = "unexpected elif statement";
            AddError(DiagnosticKind.IllegalElifStatement, msg, location);
        }

        public void IllegalElseStatement(SourceLocation location)
        {
            var msg = "unexpected else statement";
            AddError(DiagnosticKind.IllegalElseStatement, msg, location);
        }

        public void IllegalReturnStatement(SourceLocation location)
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

        public void MissingMainFunction()
        {
            var msg = "the program does not contain a suitable 'main' function";
            AddError(DiagnosticKind.MissingMainFunction, msg, null);
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
            return new DiagnosticResult(sourceText, diagnostics.ToImmutable());
        }
    }
}