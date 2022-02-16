using System.Collections.Generic;
using Kyloe.Utility;

namespace Kyloe.Syntax
{
    internal class EmptyStatement : SyntaxStatement
    {
        public EmptyStatement(SyntaxToken semicolon)
        {
            Semicolon = semicolon;
        }

        public SyntaxToken Semicolon { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.EmptyStatement;

        public override SourceLocation Location => Semicolon.Location;

        public override IEnumerable<SyntaxNodeChild> GetChildren()
        {
            yield return new SyntaxNodeChild(Semicolon);
        }
    }
}