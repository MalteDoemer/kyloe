using System.Collections.Immutable;
using System.Text;

namespace Kyloe.Grammar
{
    public sealed class ProductionRule
    {
        public ProductionRule(string name, TokenKind kind, Production production)
        {
            Name = name;
            Kind = kind;
            FirstNonLeftRecursiveProduction = -1;
            Productions = ImmutableArray.Create(production);
        }

        public ProductionRule(string name, TokenKind kind, int firstProductionNonLeftRecursiveProduction, ImmutableArray<Production> productions)
        {
            Name = name;
            Kind = kind;
            FirstNonLeftRecursiveProduction = firstProductionNonLeftRecursiveProduction;
            Productions = productions;
        }

        public string Name { get; }
        public TokenKind Kind { get; }
        public int FirstNonLeftRecursiveProduction { get; }
        public ImmutableArray<Production> Productions { get; }

        public bool IsLeftRecursive => FirstNonLeftRecursiveProduction > 0;

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