using System.Collections.Generic;
using Kyloe.Utility;

namespace Kyloe.Syntax
{
    // func hi(a: i32, b: i32) -> void {  }

    internal sealed class ParameterDeclaration : SyntaxNode
    {
        public ParameterDeclaration(SyntaxToken nameToken, TypeClause typeClause)
        {
            NameToken = nameToken;
            TypeClause = typeClause;
        }

        public SyntaxToken NameToken { get; }
        public TypeClause TypeClause { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.ParameterDeclaration;

        public override SourceLocation Location => SourceLocation.CreateAround(NameToken.Location, TypeClause.Location);

        public override IEnumerable<SyntaxNodeChild> GetChildren()
        {
            yield return new SyntaxNodeChild(NameToken);
            foreach (var child in TypeClause.GetChildren())
                yield return child;
        }
    }
}