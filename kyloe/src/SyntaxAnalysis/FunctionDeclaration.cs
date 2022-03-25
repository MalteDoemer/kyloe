using System.Collections.Generic;
using Kyloe.Utility;

namespace Kyloe.Syntax
{
    internal sealed class FunctionDeclaration : SyntaxNode
    {
        public FunctionDeclaration(SyntaxToken funcToken, SyntaxToken nameToken, SyntaxToken leftParen, ParameterList parameterList, SyntaxToken rightParen, TrailingTypeClause? trailingTypeClause)
        {
            FuncToken = funcToken;
            NameToken = nameToken;
            LeftParen = leftParen;
            ParameterList = parameterList;
            RightParen = rightParen;
            TrailingTypeClause = trailingTypeClause;
        }

        public SyntaxToken FuncToken { get; }
        public SyntaxToken NameToken { get; }
        public SyntaxToken LeftParen { get; }
        public ParameterList ParameterList { get; }
        public SyntaxToken RightParen { get; }

        public TrailingTypeClause? TrailingTypeClause { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.FunctionDecleration;

        public override SourceLocation Location => throw new System.NotImplementedException();

        public override IEnumerable<SyntaxNodeChild> GetChildren()
        {
            yield return new SyntaxNodeChild(FuncToken);
            yield return new SyntaxNodeChild(NameToken);
            yield return new SyntaxNodeChild(LeftParen);

            foreach (var child in ParameterList.GetChildren())
                yield return child;

            yield return new SyntaxNodeChild(RightParen);

            if (TrailingTypeClause is not null)
                foreach (var child in TrailingTypeClause.GetChildren())
                    yield return child;
        }
    }
}