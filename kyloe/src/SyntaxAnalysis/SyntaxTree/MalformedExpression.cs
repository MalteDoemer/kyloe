using System.Collections.Generic;
using Kyloe.Utility;

namespace Kyloe.Syntax
{
    internal class MalformedExpression: SyntaxExpression
    {
        public MalformedExpression(SyntaxToken token)
        {
            Token = token;
        }

        public SyntaxToken Token { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.MalformedExpression;

        public override SourceLocation Location => Token.Location;

        public override IEnumerable<SyntaxNodeChild> GetChildren()
        {
            yield return new SyntaxNodeChild(Token);
        }
    }
}