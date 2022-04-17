using System.Collections.Generic;
using Kyloe.Utility;

namespace Kyloe.Syntax
{
    internal sealed class IdentifierExpression : SyntaxExpression
    {
        public IdentifierExpression(SyntaxToken nameToken)
        {
            NameToken = nameToken;
        }

        public SyntaxToken NameToken { get; }

        public override SyntaxNodeKind Kind => SyntaxNodeKind.IdentifierExpression;

        public override SourceLocation Location => NameToken.Location;

        public override IEnumerable<SyntaxNodeChild> GetChildren()
        {
            yield return new SyntaxNodeChild(NameToken);
        }
    }
}