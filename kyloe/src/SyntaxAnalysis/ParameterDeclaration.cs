using System.Collections.Generic;
using Kyloe.Utility;

namespace Kyloe.Syntax
{
    internal sealed class ParameterDeclaration : SyntaxNode
    {
        public ParameterDeclaration(SyntaxToken nameToken, TypeClause typeClause)
        {
            NameToken = nameToken;
            TypeClause = typeClause;
        }

        public SyntaxToken NameToken { get; }
        public TypeClause TypeClause { get; }

        public override SyntaxNodeKind Kind => SyntaxNodeKind.ParameterDeclaration;

        public override SourceLocation Location => SourceLocation.CreateAround(NameToken.Location, TypeClause.Location);

        public override IEnumerable<SyntaxNodeChild> GetChildren()
        {
            yield return new SyntaxNodeChild(NameToken);
            foreach (var child in TypeClause.GetChildren())
                yield return child;
        }
    }
}