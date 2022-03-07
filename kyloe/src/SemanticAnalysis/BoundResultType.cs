using System.Diagnostics;
using Kyloe.Symbols;

namespace Kyloe.Semantics
{
    internal class BoundResultType
    {
        enum Kind
        {
            Value,
            Name,
            Error,
        }

        public static readonly BoundResultType ErrorResult = new BoundResultType(Kind.Error);

        private readonly ISymbol? symbol;
        private readonly Kind kind;

        private BoundResultType(Kind kind)
        {
            this.kind = kind;
        }

        public BoundResultType(ISymbol symbol, bool isValue) : this(isValue ? Kind.Value : Kind.Name)
        {
            this.symbol = symbol;
        }

        public bool IsName => kind == Kind.Name;
        public bool IsValue => kind == Kind.Value;
        public bool IsError => kind == Kind.Error;

        public ISymbol Symbol
        {
            get
            {
                Debug.Assert(!IsError); 
                return symbol!;
            }
        }


        public override string? ToString()
        {
            switch (kind)
            {
                case Kind.Name:
                case Kind.Value:
                    return Symbol.Name;
                case Kind.Error:
                    return "<error-type>";
                default:
                    throw new System.Exception($"Unexpected result kind: {kind}");
            }
        }
    }
}