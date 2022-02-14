using System.Collections.Generic;
using Kyloe.Utility;

namespace Kyloe.Syntax
{

    internal class ExpressionStatement : SyntaxStatement
    {
        public ExpressionStatement(SyntaxExpression expression, SyntaxToken semicolon)
        {
            Expression = expression;
            Semicolon = semicolon;
        }

        public SyntaxExpression Expression { get; }
        public SyntaxToken Semicolon { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.ExpressionStatement;

        public override SourceLocation Location => SourceLocation.CreateAround(Expression.Location, Semicolon.Location);

        public override IEnumerable<SyntaxNodeChild> GetChildren()
        {
            yield return new SyntaxNodeChild(Expression);
            yield return new SyntaxNodeChild(Semicolon);
        }
    }

}