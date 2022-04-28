using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
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

        public int Count => diagnostics.Count;

        public bool IsReadOnly => ((ICollection<Diagnostic>)diagnostics).IsReadOnly;

        public void Add(Diagnostic item)
        {
            diagnostics.Add(item);
        }

        public void AddDiagnostic(DiagnosticKind kind, string message, SourceLocation? location)
        {
            Add(new Diagnostic(kind, message, location));
        }

        public void InvalidLiteralError(SourceLocation location) 
        {
            var msg = "invalid literal";
            AddDiagnostic(DiagnosticKind.InvalidLiteralError, msg, location);
        }

        public void ExpectedModifiableValueError(SourceLocation location)
        {
            var msg = $"expected a modifiable value";
            AddDiagnostic(DiagnosticKind.ExpectedModifiableValueError, msg, location);
        }

        public void ExpectedValueError(SourceLocation location)
        {
            var msg = $"expected a value";
            AddDiagnostic(DiagnosticKind.ExpectedValueError, msg, location);
        }

        public void ExpectedTypeNameError(SourceLocation location)
        {
            var msg = $"expected a type name";
            AddDiagnostic(DiagnosticKind.ExpectedTypeNameError, msg, location);
        }

        public void UnsupportedBinaryOperation(SourceLocation location, BoundOperation operation, TypeSpecifier left, TypeSpecifier right)
        {
            var msg = $"operator '{operation}' cannot be used with types '{left.FullName()}' and '{right.FullName()}'";
            AddDiagnostic(DiagnosticKind.UnsupportedBinaryOperation, msg, location);
        }

        public void UnsupportedUnaryOperation(SourceLocation location, BoundOperation operation, TypeSpecifier type)
        {
            var msg = $"operator '{operation}' cannot be used with type '{type.FullName()}'";
            AddDiagnostic(DiagnosticKind.UnsupportedUnaryOperation, msg, location);
        }

        public void UnsupportedAssignmentOperation(SourceLocation location, BoundOperation operation, TypeSpecifier left, TypeSpecifier right)
        {
            var msg = $"operator '{operation}' cannot be used with types '{left.FullName()}' and '{right.FullName()}'";
        }

        public void MissmatchedTypeError(SourceLocation location, TypeSpecifier expected, TypeSpecifier provided)
        {
            var msg = $"missmatched types, expected '{expected}', got '{provided}'";
            AddDiagnostic(DiagnosticKind.MissmatchedTypeError, msg, location);
        }

        public void NonExistantNameError(SourceLocation location, string name)
        {
            var msg = $"the name '{name}' does not exist in the current scope";
            AddDiagnostic(DiagnosticKind.NonExistantNameError, msg, location);
        }

        public void NameAlreadyExistsError(SourceLocation location, string name)
        {
            var msg = $"the name '{name}' already exists";
            AddDiagnostic(DiagnosticKind.NameAlreadyExistsError, msg, location);
        }

        public void RedefinedParameterError(SourceLocation location, string name)
        {
            var msg = $"the parameter '{name}' already exists";
            AddDiagnostic(DiagnosticKind.RedefinedParameterError, msg, location);
        }

        public void NotCallableError(SourceLocation location)
        {
            var msg = $"expression is not callable";
            AddDiagnostic(DiagnosticKind.NotCallableError, msg, location);
        }

        public void NoMatchingOverloadError(SourceLocation location, string name, BoundArguments arguments)
        {
            string msg;
            if (arguments.Arguments.Length == 0)
                msg = $"cannot call {name} without arguments";
            else
                msg = $"cannot call {name} with arguments ({arguments.JoinArgumentTypes()})";

            AddDiagnostic(DiagnosticKind.NoMatchingOverloadError, msg, location);
        }

        public void OverloadWithSameParametersExistsError(SourceLocation location, string name)
        {
            var msg = $"the function '{name}' already exists with the same parameters";
            AddDiagnostic(DiagnosticKind.OverloadWithSameParametersExistsError, msg, location);
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