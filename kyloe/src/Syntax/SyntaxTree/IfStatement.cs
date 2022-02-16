using Kyloe.Utility;
using System.Collections.Generic;

namespace Kyloe.Syntax
{
    internal class IfStatement : SyntaxStatement
    {
        public IfStatement(SyntaxToken ifToken, SyntaxExpression condition, SyntaxStatement body, ElseClause? elseClause)
        {
            IfToken = ifToken;
            Condition = condition;
            Body = body;
            ElseClause = elseClause;
        }

        public SyntaxToken IfToken { get; }
        public SyntaxExpression Condition { get; }
        public SyntaxStatement Body { get; }
        public ElseClause? ElseClause { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.IfStatement;

        public override SourceLocation Location => ElseClause is null ? SourceLocation.CreateAround(IfToken.Location, Body.Location) : SourceLocation.CreateAround(IfToken.Location, ElseClause.Location);

        public override IEnumerable<SyntaxNodeChild> GetChildren()
        {
            yield return new SyntaxNodeChild(IfToken);
            yield return new SyntaxNodeChild(Condition);
            yield return new SyntaxNodeChild(Body);

            if (ElseClause is not null)
                yield return new SyntaxNodeChild(ElseClause);
        }
    }
}