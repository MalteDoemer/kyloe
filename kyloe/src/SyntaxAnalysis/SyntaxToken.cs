using Kyloe.Utility;

namespace Kyloe.Syntax
{
    public sealed class SyntaxToken
    {
        public SyntaxToken(SyntaxTokenKind kind, SourceLocation location, object? value = null)
        {
            Kind = kind;
            Location = location;
            Value = value;
        }

        public SyntaxTokenKind Kind { get; }
        public SourceLocation Location { get; }
        public object? Value { get; }

        public override string ToString()
        {
            if (Value is null)
                return $"{Kind}";
            else
                return $"{Kind}: {Value}";
        }
    }
}