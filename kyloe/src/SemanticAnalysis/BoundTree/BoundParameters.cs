using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Kyloe.Symbols;
using Kyloe.Syntax;

namespace Kyloe.Semantics
{
    internal sealed class BoundParameters : BoundNode
    {
        public BoundParameters(ImmutableArray<BoundParameterDeclaration> parameters, SyntaxToken syntax) 
        {
            Parameters = parameters;
            Syntax = syntax;
        }

        public ImmutableArray<BoundParameterDeclaration> Parameters { get; }
        
        public override SyntaxToken Syntax { get; }

        public bool AllParametersValid => Parameters.All(p => p.Type is not ErrorType);

        public IEnumerable<TypeSpecifier> ParameterTypes => Parameters.Select(p => p.Type);

        public override BoundNodeKind Kind => BoundNodeKind.BoundParameters;
    }
}