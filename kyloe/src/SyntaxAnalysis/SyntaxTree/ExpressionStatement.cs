using System.Collections.Generic;
using Kyloe.Utility;

namespace Kyloe.Syntax
{
    internal sealed class ExpressionStatement : SyntaxStatement
    {
        public ExpressionStatement(SyntaxExpression expression, SyntaxToken semicolon)
        {
            Expression = expression;
            Semicolon = semicolon;
        }

        public SyntaxExpression Expression { get; }
        public SyntaxToken Semicolon { get; }

        public override SyntaxNodeKind Kind => SyntaxNodeKind.ExpressionStatement;

        public override SourceLocation Location => SourceLocation.CreateAround(Expression.Location, Semicolon.Location);

        public override IEnumerable<SyntaxNodeChild> GetChildren()
        {
            yield return new SyntaxNodeChild(Expression);
            yield return new SyntaxNodeChild(Semicolon);
        }
    }
}