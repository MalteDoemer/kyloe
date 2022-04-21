using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Kyloe.Grammar
{
    public sealed class FinalGrammar
    {
        private readonly BoundGrammar boundGrammar;

        private readonly Dictionary<TokenKind, HashSet<TokenKind>> firstSetCache;
        private readonly Dictionary<TokenKind, HashSet<TokenKind>> followSetCache;

        private FinalGrammar(BoundGrammar boundGrammar)
        {
            this.boundGrammar = boundGrammar;
            StartRule = LookupRule("start", StringComparison.OrdinalIgnoreCase);
            DiscardRule = LookupRule("discard", StringComparison.OrdinalIgnoreCase);

            firstSetCache = new Dictionary<TokenKind, HashSet<TokenKind>>();
            followSetCache = new Dictionary<TokenKind, HashSet<TokenKind>>();
        }

        public ProductionRule? StartRule { get; }
        public ProductionRule? DiscardRule { get; }

        public ImmutableDictionary<TokenKind, TerminalDefinition> Terminals => boundGrammar.Terminals;
        public ImmutableDictionary<TokenKind, ProductionRule> Rules => boundGrammar.Rules;

        public string GetName(TokenKind kind)
        {
            if (kind == TokenKind.End)
                return "End";
            else if (kind == TokenKind.Epsilon)
                return "Epsilon";
            else if (kind == TokenKind.Error)
                return "Error";
            else if (kind.IsTerminal)
                return boundGrammar.Terminals[kind].Name;
            else
                return boundGrammar.Rules[kind].Name;
        }

        public ProductionRule? LookupRule(string name, StringComparison comparison = StringComparison.Ordinal)
        {
            return boundGrammar.Rules.Values.Where(r => string.Equals(r.Name, name, comparison)).FirstOrDefault();
        }

        /// <summary>
        /// Find the FIRST set of a given token.
        /// </summary>
        /// <remarks>This function does not work with indirect left recursion</remarks>
        public IReadOnlySet<TokenKind> FirstSet(TokenKind kind)
        {
            if (firstSetCache.TryGetValue(kind, out var cached))
                return cached;

            var firstSet = new HashSet<TokenKind>();

            if (kind.IsTerminal)
            {
                firstSet.Add(kind);
            }
            else
            {
                var rule = Rules[kind];

                foreach (var prod in rule.Productions)
                    firstSet.UnionWith(FirstSet(kind, prod));
            }

            firstSetCache.Add(kind, firstSet);
            return firstSet;
        }

        /// <summary>
        /// Find the FIRST set of a given production in a rule.
        /// </summary>
        /// <param name="rule">The rule that corresponds to the production</param>
        /// <param name="production">The production from which the FIRST set should be computed</param>
        /// <remarks>This function does not work with indirect left recursion</remarks>
        public IReadOnlySet<TokenKind> FirstSet(TokenKind rule, Production production)
        {
            // Algorithm to compute FIRST(A, α)

            // - For every child Yi in α
            // -    If Yi == A
            // -        If Yi is last child, Add ε to FIRST(A, α)
            // -        break
            // -    Else
            // -        Add FIRST(Yi) to FIRST(A, α)
            // -        If ε ∉ FIRST(Yi), break
            // -
            // - If ε ∈ FIRST(Yi) for every child Yi in α
            // -    Add ε to FIRST(A, α)

            var firstSet = new HashSet<TokenKind>();

            var children = production.Children();
            var last = production.Children().Last();

            foreach (var child in children)
            {
                if (child == rule)
                {
                    if (child == last)
                        firstSet.Add(TokenKind.Epsilon);
                    break;
                }

                var childFirst = FirstSet(child);

                firstSet.UnionWith(childFirst);
                firstSet.Remove(TokenKind.Epsilon); // remove does nothing if the element wasn't present

                if (!childFirst.Contains(TokenKind.Epsilon))
                    break;

                if (child == last)
                    firstSet.Add(TokenKind.Epsilon);
            }

            return firstSet;
        }

        /// <summary>
        /// Find the FOLLOW set of a given token.
        /// </summary>
        /// <remarks>This function does not work with indirect right recursion</remarks>
        public IReadOnlySet<TokenKind> FollowSet(TokenKind kind)
        {
            // The algorithm to compute FOLLOW(B):
            //
            // - If B is the start rule, add $ to FOLLOW(B)
            // - For every production A -> α
            // -    If this is a production A -> αB
            // -        If A == B, add FIRST(A) to FOLLOW(B)
            // -        Else add FOLLOW(A) to FOLLOW(B)
            // -    If this is a production A -> αBβ
            // -        If ε ∉ FIRST(β), add FIRST(β) to FOLLOW(B)
            // -        IF ε ∈ FIRST(β), then add { FIRST(β) – ε } ∪ FOLLOW(β) to FOLLOW(B)

            if (followSetCache.TryGetValue(kind, out var cached))
                return cached;

            var followSet = new HashSet<TokenKind>();

            if (StartRule is not null && StartRule.Kind == kind)
                followSet.Add(TokenKind.End);

            foreach (var rule in Rules.Values)
                foreach (var prod in rule.Productions)
                {
                    var children = prod.Children();
                    var last = children.Last();

                    foreach (var (i, child) in children.EnumerateIndex())
                    {
                        if (child != kind)
                            continue;

                        if (child == last)
                        {
                            if (rule.Kind == kind)
                                followSet.UnionWith(FirstSet(rule.Kind));
                            else
                                followSet.UnionWith(FollowSet(rule.Kind));
                        }
                        else
                        {
                            var next = children.ElementAt(i + 1);
                            var first = FirstSet(next);

                            if (first.Contains(TokenKind.Epsilon))
                            {
                                followSet.UnionWith(first);
                                followSet.Remove(TokenKind.Epsilon);
                                followSet.UnionWith(FollowSet(next));
                            }
                            else
                            {
                                followSet.UnionWith(first);
                            }
                        }
                    }
                }

            followSetCache.Add(kind, followSet);
            return followSet;
        }

        public static FinalGrammar CreateFromText(string text)
        {
            var parser = new GrammarParser(text);
            var binder = new GrammarBinder(parser.Parse());
            var flattener = new GrammarFlattener(binder.Bind());
            var grammar = flattener.Flatten();
            return new FinalGrammar(grammar);
        }
    }
}