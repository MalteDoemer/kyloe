using System.Collections.Generic;
using Kyloe.Utility;

namespace Kyloe.Syntax
{
    internal class LiteralExpression : SyntaxExpression
    {
        public LiteralExpression(SyntaxToken literalToken)
        {
            LiteralToken = literalToken;
        }

        public SyntaxToken LiteralToken { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.LiteralExpression;

        public override SourceLocation Location => LiteralToken.Location;

        public override IEnumerable<SyntaxNodeChild> GetChildren()
        {
            yield return new SyntaxNodeChild(LiteralToken);
        }
    }
}