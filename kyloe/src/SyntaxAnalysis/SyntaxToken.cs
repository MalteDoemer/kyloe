using Kyloe.Utility;

namespace Kyloe.Syntax
{
    public sealed class SyntaxToken
    {
        public SyntaxToken(SyntaxTokenType type, SourceLocation location, object? value = null)
        {
            Type = type;
            Location = location;
            Value = value;
        }

        public SyntaxTokenType Type { get; }
        public SourceLocation Location { get; }
        public object? Value { get; }

        public override string ToString()
        {
            if (Value is null)
                return $"{Type}";
            else
                return $"{Type}: {Value}";
        }
    }
}