using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace Kyloe.Grammar
{
    internal sealed class GrammarFlattener
    {
        private readonly BoundGrammar boundGrammar;

        public GrammarFlattener(BoundGrammar boundGrammar)
        {
            this.boundGrammar = boundGrammar;
        }

        public BoundGrammar Flatten()
        {
            var builder = ImmutableDictionary.CreateBuilder<TokenKind, ProductionRule>();

            foreach (var rule in boundGrammar.Rules.Values)
            {
                builder.Add(rule.Kind, Flatten(rule));
            }

            return new BoundGrammar(boundGrammar.Terminals, builder.ToImmutable());
        }

        /// <summary>
        /// This function goes through all productions and removes the or productions just like when multiplying out in maths.
        /// Also it reorders the production, so that all productions that are left recursive come first.
        /// </summary>
        private ProductionRule Flatten(ProductionRule rule)
        {
            // The binder always creates a rule with one production.
            Debug.Assert(rule.Productions.Length == 1);

            var production = rule.Productions.First();

            var withRecursion = ImmutableArray.CreateBuilder<Production>();
            var noRecursion = ImmutableArray.CreateBuilder<Production>();

            foreach (var prod in FlattenProduction(production))
                if (prod.Children().First() == rule.Kind)
                    withRecursion.Add(prod);
                else
                    noRecursion.Add(prod);

            var index = withRecursion.Count;
            withRecursion.AddRange(noRecursion);

            return new ProductionRule(rule.Name, rule.Kind, index, withRecursion.ToImmutable());
        }


        private IEnumerable<Production> FlattenProduction(Production production)
        {
            switch (production)
            {
                case EmptyProduction emptyProduction:
                    return FlattenEmptyProduction(emptyProduction);
                case NameProduction nameProduction:
                    return FlattenNameProduction(nameProduction);
                case ConcatProduction concatProduction:
                    return FlattenConcatProduction(concatProduction);
                case OrProduction orProduction:
                    return FlattenOrProduction(orProduction);
                default:
                    throw new Exception($"unexpected production: {production.GetType()}");
            }
        }

        private IEnumerable<Production> FlattenOrProduction(OrProduction orProduction)
        {
            var left = FlattenProduction(orProduction.Left);
            var right = FlattenProduction(orProduction.Right);
            return left.Concat(right);
        }

        private IEnumerable<Production> FlattenConcatProduction(ConcatProduction concatProduction)
        {
            var left = FlattenProduction(concatProduction.Left);
            var right = FlattenProduction(concatProduction.Right);

            foreach (var leftChild in left)
                foreach (var rightChild in right)
                    yield return new ConcatProduction(leftChild, rightChild);
        }

        private IEnumerable<Production> FlattenNameProduction(NameProduction nameProduction)
        {
            yield return nameProduction;
        }

        private IEnumerable<Production> FlattenEmptyProduction(EmptyProduction emptyProduction)
        {
            yield return emptyProduction;
        }
    }
}