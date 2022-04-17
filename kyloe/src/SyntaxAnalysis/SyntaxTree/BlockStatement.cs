using System.Collections.Generic;
using System.Collections.Immutable;
using Kyloe.Utility;

namespace Kyloe.Syntax
{
    internal sealed class BlockStatement : SyntaxStatement
    {
        public BlockStatement(SyntaxToken leftCurly, ImmutableArray<SyntaxStatement> statements, SyntaxToken rightCurly)
        {
            LeftCurly = leftCurly;
            Statements = statements;
            RightCurly = rightCurly;
        }

        public SyntaxToken LeftCurly { get; }
        public ImmutableArray<SyntaxStatement> Statements { get; }
        public SyntaxToken RightCurly { get; }

        public override SyntaxNodeKind Kind => SyntaxNodeKind.BlockStatement;

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