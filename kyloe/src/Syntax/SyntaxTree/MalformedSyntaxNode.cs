using Kyloe.Utility;

namespace Kyloe.Syntax
{
    class MalformedSyntaxNode : SyntaxNode
    {
        public MalformedSyntaxNode(SyntaxToken token)
        {
            Token = token;
        }

        public SyntaxToken Token { get; }

        public override SourceLocation Location => Token.Location;
    }
}