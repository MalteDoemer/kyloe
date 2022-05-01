using Kyloe.Symbols;
using Kyloe.Syntax;

namespace Kyloe.Semantics
{
    internal sealed class BoundInvalidExpression : BoundExpression
    {

        public BoundInvalidExpression(TypeSystem typeSystem, SyntaxToken syntax)
        {
            ResultType = typeSystem.Error;
            Syntax = syntax;
        }

        public override TypeSpecifier ResultType { get; }

        public override SyntaxToken Syntax { get; }
        
        public override BoundNodeKind Kind => BoundNodeKind.BoundInvalidExpression;

        public override ValueCategory ValueCategory => ValueCategory.None;

    }
}