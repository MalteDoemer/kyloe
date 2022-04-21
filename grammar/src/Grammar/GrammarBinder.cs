
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Kyloe.Grammar
{
    internal sealed class GrammarBinder
    {
        private readonly ParsedGrammar grammar;

        private readonly Dictionary<string, TokenKind> names;

        public GrammarBinder(ParsedGrammar grammar)
        {
            this.grammar = grammar;
            this.names = new Dictionary<string, TokenKind>();
        }

        private void DeclareNames()
        {
            foreach (var stmt in grammar.Statements)
            {
                if (IsTerminalDefinitionStatement(stmt))
                {
                    var kind = TokenKind.CreateTerminal();
                    names.Add(stmt.NameToken.Text, kind);
                }
                else
                {
                    var kind = TokenKind.CreateNonTerminal();
                    names.Add(stmt.NameToken.Text, kind);
                }
            }
        }

        public BoundGrammar Bind()
        {
            DeclareNames();

            var terminals = ImmutableDictionary.CreateBuilder<TokenKind, TerminalDefinition>();
            var rules = ImmutableDictionary.CreateBuilder<TokenKind, ProductionRule>();

            foreach (var stmt in grammar.Statements)
            {
                var name = stmt.NameToken.Text;
                var kind = names[name];

                if (kind.IsTerminal)
                {
                    var literalNode = (LiteralGrammarNode)stmt.Node;
                    var def = new TerminalDefinition(name, kind, literalNode.LiteralToken.Text);
                    terminals.Add(kind, def);
                }
                else
                {
                    var prod = BindNode(stmt.Node);
                    var rule = new ProductionRule(name, kind, ImmutableArray.Create(prod));
                    rules.Add(kind, rule);
                }
            }

            return new BoundGrammar(terminals.ToImmutable(), rules.ToImmutable());
        }

        private Production BindNode(GrammarNode node)
        {
            switch (node)
            {
                case LiteralGrammarNode literalGrammarNode:
                    throw new GrammarException("cannot have a literal directly in a production rule, try introducing a new rule for the literal", literalGrammarNode.Location);
                case NameGrammarNode nameGrammarNode:
                    return BindNameGrammarNode(nameGrammarNode);
                case OptionalGrammarNode optionalGrammarNode:
                    return BindOptionalGrammarNode(optionalGrammarNode);
                case OrGrammarNode orGrammarNode:
                    return BindOrGrammarNode(orGrammarNode);
                case ConcatGrammarNode concatGrammarNode:
                    return BindConcatGrammarNode(concatGrammarNode);
                default:
                    throw new Exception($"unexpected GrammarNode: {node.GetType()}");
            }
        }

        private Production BindConcatGrammarNode(ConcatGrammarNode concatGrammarNode)
        {
            var left = BindNode(concatGrammarNode.Left);
            var right = BindNode(concatGrammarNode.Right);
            return new ConcatProduction(left, right);
        }

        private Production BindOrGrammarNode(OrGrammarNode orGrammarNode)
        {
            var left = BindNode(orGrammarNode.Left);
            var right = BindNode(orGrammarNode.Right);
            return new OrProduction(left, right);
        }

        private Production BindOptionalGrammarNode(OptionalGrammarNode optionalGrammarNode)
        {
            return new EmptyProduction();
        }

        private Production BindNameGrammarNode(NameGrammarNode nameGrammarNode)
        {
            var name = nameGrammarNode.NameToken.Text;
            if (names.TryGetValue(name, out var kind))
                return new NameProduction(name, kind);
            else
                throw new GrammarException($"the name '{name}' does not exist", nameGrammarNode.Location);
        }

        private bool IsTerminalDefinitionStatement(GrammarStatement statement)
        {
            return statement.Node is LiteralGrammarNode;
        }
    }
}