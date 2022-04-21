using System;
using System.Collections.Generic;
using System.Linq;

namespace Kyloe.Grammar
{
    internal abstract class Production
    {
        public abstract IEnumerable<TokenKind> Children();
    }


    internal sealed class EmptyProduction : Production
    {
        public override IEnumerable<TokenKind> Children()
        {
            yield return TokenKind.Epsilon;
        }

        public override string ToString() => "ε";
    }

    internal sealed class NameProduction : Production
    {
        public NameProduction(string name, TokenKind kind)
        {
            Name = name;
            Kind = kind;
        }

        public string Name { get; }
        public TokenKind Kind { get; }

        public bool IsTerminal => Kind.IsTerminal;
        public bool IsNonTerminal => !Kind.IsTerminal;

        public override IEnumerable<TokenKind> Children()
        {
            yield return Kind;
        }

        public override string ToString() => Name;
    }

    internal sealed class ConcatProduction : Production
    {
        public ConcatProduction(Production left, Production right)
        {
            Left = left;
            Right = right;
        }

        public Production Left { get; }
        public Production Right { get; }

        public override IEnumerable<TokenKind> Children()
        {
            return Left.Children().Concat(Right.Children());
        }

        public override string ToString() => $"({Left}, {Right})";
    }

    internal sealed class OrProduction : Production
    {
        public OrProduction(Production left, Production right)
        {
            Left = left;
            Right = right;
        }

        public Production Left { get; }
        public Production Right { get; }

        public override IEnumerable<TokenKind> Children()
        {
            throw new NotSupportedException();
        }

        public override string ToString() => $"({Left} | {Right})";
    }
}