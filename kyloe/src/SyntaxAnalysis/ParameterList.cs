using System.Collections.Generic;
using System.Collections.Immutable;

namespace Kyloe.Syntax
{
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
}