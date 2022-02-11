

using Kyloe.Utility;

namespace Kyloe.Syntax
{

    class MemberAccessNode : SyntaxNode
    {
        public MemberAccessNode(SyntaxToken nameToken, SyntaxToken dotToken, SyntaxNode child)
        {
            NameToken = nameToken;
            DotToken = dotToken;
            Child = child;
        }

        public SyntaxToken NameToken { get; }
        public SyntaxToken DotToken { get; }
        public SyntaxNode Child { get; }

        public override SourceLocation Location => SourceLocation.CreateAround(NameToken.Location, Child.Location);
    }

}