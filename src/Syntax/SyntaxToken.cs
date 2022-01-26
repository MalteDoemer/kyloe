namespace Kyloe.Syntax
{
    class SyntaxToken
    {
        public SyntaxToken(SyntaxTokenType type, object? value = null)
        {
            Type = type;
            Value = value;
        }

        public SyntaxTokenType Type { get; }
        public object? Value { get; }

        public override string ToString()
        {
            if (Value is null)
            {
                return Type.ToString();
            }
            else
            {
                return $"{Type}: {Value}";
            }
        }
    }
}