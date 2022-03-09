using System.Collections.Generic;
using Kyloe.Utility;

namespace Kyloe.Syntax
{
    internal sealed class DeclarationStatement : SyntaxStatement
    {
        public DeclarationStatement(SyntaxToken declerationToken, SyntaxToken nameToken, SyntaxToken equalsToken, SyntaxExpression assignmentExpression, SyntaxToken semicolon)
        {
            DeclerationToken = declerationToken;
            NameToken = nameToken;
            EqualsToken = equalsToken;
            AssignmentExpression = assignmentExpression;
            Semicolon = semicolon;
        }

        public SyntaxToken DeclerationToken { get; }
        public SyntaxToken NameToken { get; }
        public SyntaxToken EqualsToken { get; }
        public SyntaxExpression AssignmentExpression { get; }
        public SyntaxToken Semicolon { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.DeclarationStatement;

        public override SourceLocation Location => SourceLocation.CreateAround(DeclerationToken.Location, AssignmentExpression.Location);

        public override IEnumerable<SyntaxNodeChild> GetChildren()
        {
            yield return new SyntaxNodeChild(DeclerationToken);
            yield return new SyntaxNodeChild(NameToken);
            yield return new SyntaxNodeChild(EqualsToken);
            yield return new SyntaxNodeChild(AssignmentExpression);
            yield return new SyntaxNodeChild(Semicolon);
        }
    }
}