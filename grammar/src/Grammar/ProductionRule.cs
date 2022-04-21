using System.Collections.Immutable;
using System.Text;

namespace Kyloe.Grammar
{
    internal sealed class ProductionRule
    {
        public ProductionRule(string name, TokenKind kind, ImmutableArray<Production> productions)
        {
            Name = name;
            Kind = kind;
            Productions = productions;
        }

        public string Name { get; }
        public TokenKind Kind { get; }
        public ImmutableArray<Production> Productions { get; }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append($"{Name} = ");

            foreach (var prod in Productions)
            {
                builder.Append(prod.ToString());
                builder.Append(" | ");
            }

            // Hack:
            // Remove the or symbol of the last node
            builder.Remove(builder.Length - 3, 3);

            return builder.ToString();
        }
    }
}