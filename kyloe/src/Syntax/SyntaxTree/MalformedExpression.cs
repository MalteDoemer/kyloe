using Kyloe.Utility;

namespace Kyloe.Syntax
{
    class MalformedExpression : SyntaxNode
    {
        public MalformedExpression(SyntaxToken token)
        {
            Token = token;
        }

        public SyntaxToken Token { get; }

        public override SourceLocation Location => Token.Location;
    }
}