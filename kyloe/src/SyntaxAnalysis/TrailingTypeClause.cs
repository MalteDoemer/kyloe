using System.Collections.Generic;
using Kyloe.Utility;

namespace Kyloe.Syntax
{
    internal sealed class TrailingTypeClause
    {
        public TrailingTypeClause(SyntaxToken arrowToken, SyntaxExpression nameExpression)
        {
            ArrowToken = arrowToken;
            NameExpression = nameExpression;
        }

        public SyntaxToken ArrowToken { get; }
        public SyntaxExpression NameExpression { get; }

        public SourceLocation Location => SourceLocation.CreateAround(ArrowToken.Location, NameExpression.Location);

        public IEnumerable<SyntaxNodeChild> GetChildren()
        {
            yield return new SyntaxNodeChild(ArrowToken);
            yield return new SyntaxNodeChild(NameExpression);
        }
    }
}