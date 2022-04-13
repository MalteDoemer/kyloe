using System.Collections.Generic;
using System.Collections.Immutable;
using Kyloe.Utility;

namespace Kyloe.Syntax
{
    internal sealed class BlockSyntax : SyntaxNode
    {
        public BlockSyntax(SyntaxToken leftCurly, ImmutableArray<SyntaxNode> statements, SyntaxToken rightCurly)
        {
            LeftCurly = leftCurly;
            Statements = statements;
            RightCurly = rightCurly;
        }

        public SyntaxToken LeftCurly { get; }
        public ImmutableArray<SyntaxNode> Statements { get; }
        public SyntaxToken RightCurly { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.BlockSyntax;

        public override SourceLocation Location => SourceLocation.CreateAround(LeftCurly.Location, RightCurly.Location);

        public override IEnumerable<SyntaxNodeChild> GetChildren()
        {
            yield return new SyntaxNodeChild(LeftCurly);

            foreach (var stmt in Statements)
                yield return new SyntaxNodeChild(stmt);

            yield return new SyntaxNodeChild(RightCurly);
        }
    }
}