using System.Collections.Generic;
using Kyloe.Utility;

namespace Kyloe.Syntax
{
    internal sealed class CallSyntax : SyntaxNode
    {
        public CallSyntax(SyntaxNode node, SyntaxNode arguments)
        {
            Expression = node;
            Arguments = arguments;
        }

        public SyntaxNode Expression { get; }
        public SyntaxNode Arguments { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.CallSyntax;

        public override SourceLocation Location => SourceLocation.CreateAround(Expression.Location, Arguments.Location);

        public override IEnumerable<SyntaxNodeChild> GetChildren()
        {
            yield return new SyntaxNodeChild(Expression);
            yield return new SyntaxNodeChild(Arguments);
        }
    }
}