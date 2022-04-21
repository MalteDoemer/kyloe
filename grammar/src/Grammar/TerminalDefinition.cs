namespace Kyloe.Grammar
{
    internal sealed class TerminalDefinition
    {
        public TerminalDefinition(string name, TokenKind kind, string text)
        {
            Name = name;
            Kind = kind;
            Text = text;
        }

        public string Name { get; }
        public TokenKind Kind { get; }
        public string Text { get; }
    }
}