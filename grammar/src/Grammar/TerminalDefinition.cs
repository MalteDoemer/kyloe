namespace Kyloe.Grammar
{
    public sealed class TerminalDefinition
    {
        public TerminalDefinition(string name, TokenKind kind, string text, bool isRegex)
        {
            Name = name;
            Kind = kind;
            Text = text;
            IsRegex = isRegex;
        }

        public string Name { get; }
        public TokenKind Kind { get; }
        public string Text { get; }
        public bool IsRegex { get; }

        public override string ToString() => $"{Name} = '{Text}'";
    }
}