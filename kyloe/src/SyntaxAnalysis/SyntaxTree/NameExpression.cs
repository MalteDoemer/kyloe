using System.Collections.Generic;
using Kyloe.Utility;

namespace Kyloe.Syntax
{

    internal class NameExpression : SyntaxExpression
    {
        public NameExpression(SyntaxToken nameToken)
        {
            NameToken = nameToken;
        }

        public SyntaxToken NameToken { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.NameExpression;

        public override SourceLocation Location => NameToken.Location;

        public override IEnumerable<SyntaxNodeChild> GetChildren()
        {
            yield return new SyntaxNodeChild(NameToken);
        }
    }

}