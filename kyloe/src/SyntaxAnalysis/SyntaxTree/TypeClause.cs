using System.Collections.Generic;
using Kyloe.Utility;

namespace Kyloe.Syntax
{
    internal sealed class TypeClause
    {
        public TypeClause(SyntaxToken token, SyntaxExpression nameExpression)
        {
            Token = token;
            NameExpression = nameExpression;
        }

        public SyntaxToken Token { get; }
        public SyntaxExpression NameExpression { get; }

        public SourceLocation Location => SourceLocation.CreateAround(Token.Location, NameExpression.Location);

        public IEnumerable<SyntaxNodeChild> GetChildren()
        {
            yield return new SyntaxNodeChild(Token);
            yield return new SyntaxNodeChild(NameExpression);
        }
    }
}