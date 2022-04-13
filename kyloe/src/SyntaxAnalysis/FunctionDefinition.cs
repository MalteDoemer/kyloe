using System.Collections.Generic;
using Kyloe.Utility;

namespace Kyloe.Syntax
{
    internal sealed class FunctionDefinition : SyntaxNode
    {
        public FunctionDefinition(SyntaxToken funcToken, SyntaxToken nameToken, SyntaxToken leftParen, ParameterList parameterList, SyntaxToken rightParen, TypeClause? typeClause, BlockStatement body)
        {
            FuncToken = funcToken;
            NameToken = nameToken;
            LeftParen = leftParen;
            ParameterList = parameterList;
            RightParen = rightParen;
            TypeClause = typeClause;
            Body = body;
        }

        public SyntaxToken FuncToken { get; }
        public SyntaxToken NameToken { get; }
        public SyntaxToken LeftParen { get; }
        public ParameterList ParameterList { get; }
        public SyntaxToken RightParen { get; }
        public TypeClause? TypeClause { get; }
        public BlockStatement Body { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.FunctionDefinition;

        public override SourceLocation Location => throw new System.NotImplementedException();

        public override IEnumerable<SyntaxNodeChild> GetChildren()
        {
            yield return new SyntaxNodeChild(FuncToken);
            yield return new SyntaxNodeChild(NameToken);
            yield return new SyntaxNodeChild(LeftParen);

            foreach (var child in ParameterList.GetChildren())
                yield return child;

            yield return new SyntaxNodeChild(RightParen);

            if (TypeClause is not null)
                foreach (var child in TypeClause.GetChildren())
                    yield return child;

            yield return new SyntaxNodeChild(Body);
        }
    }
}