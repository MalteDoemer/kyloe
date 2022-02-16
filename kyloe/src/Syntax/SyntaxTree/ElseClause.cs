using Kyloe.Utility;
using System.Collections.Generic;

namespace Kyloe.Syntax
{
    internal class ElseClause : SyntaxStatement
    {
        public ElseClause(SyntaxToken elseToken, SyntaxStatement body)
        {
            ElseToken = elseToken;
            Body = body;
        }

        public SyntaxToken ElseToken { get; }
        public SyntaxStatement Body { get; }

        public override SourceLocation Location => SourceLocation.CreateAround(ElseToken.Location, Body.Location);

        public override SyntaxNodeType Type => SyntaxNodeType.ElseClause;

        public override IEnumerable<SyntaxNodeChild> GetChildren()
        {
            yield return new SyntaxNodeChild(ElseToken);
            yield return new SyntaxNodeChild(Body);
        }
    }
}