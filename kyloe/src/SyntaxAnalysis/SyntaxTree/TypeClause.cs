using System.Collections.Generic;
using Kyloe.Utility;

namespace Kyloe.Syntax
{
    internal sealed class TypeClause
    {
        public TypeClause(SyntaxToken colonToken, SyntaxExpression nameExpression)
        {
            ColonToken = colonToken;
            NameExpression = nameExpression;
        }

        public SyntaxToken ColonToken { get; }
        public SyntaxExpression NameExpression { get; }

        public SourceLocation Location => SourceLocation.CreateAround(ColonToken.Location, NameExpression.Location);

        public IEnumerable<SyntaxNodeChild> GetChildren()
        {
            yield return new SyntaxNodeChild(ColonToken);
            yield return new SyntaxNodeChild(NameExpression);
        }
    }
}