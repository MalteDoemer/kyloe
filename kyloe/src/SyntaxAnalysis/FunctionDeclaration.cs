using System.Collections.Generic;
using System.Collections.Immutable;
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

    internal sealed class ParameterList
    {
        public ParameterList(ImmutableArray<ParameterDeclaration> parameters, ImmutableArray<SyntaxToken> commas)
        {
            Parameters = parameters;
            Commas = commas;
        }

        public ImmutableArray<ParameterDeclaration> Parameters;
        public ImmutableArray<SyntaxToken> Commas;

        public IEnumerable<SyntaxNodeChild> GetChildren()
        {
            foreach (var param in Parameters)
                yield return new SyntaxNodeChild(param);
        }

        public static ParameterList Empty = new ParameterList(ImmutableArray<ParameterDeclaration>.Empty, ImmutableArray<SyntaxToken>.Empty);
    }

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