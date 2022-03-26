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
            int i = 0;
            for (i = 0; i < Commas.Length; i++)
            {
                yield return new SyntaxNodeChild(Parameters[i]);
                yield return new SyntaxNodeChild(Commas[i]);
            }

            for (; i < Parameters.Length; i++)
                yield return new SyntaxNodeChild(Parameters[i]);
        }

        public static ParameterList Empty = new ParameterList(ImmutableArray<ParameterDeclaration>.Empty, ImmutableArray<SyntaxToken>.Empty);
    }
}