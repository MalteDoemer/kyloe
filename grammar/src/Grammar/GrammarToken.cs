
namespace Kyloe.Grammar
{
    public struct GrammarToken
    {
        public GrammarToken(GrammarTokenKind kind, string text, GrammarLocation location)
        {
            Kind = kind;
            Text = text;
            Location = location;
        }

        public GrammarTokenKind Kind { get; }
        public string Text { get; }
        public GrammarLocation Location { get; }

        public override string ToString() => $"{Kind}({Location})";
    }
}