using System.Collections.Immutable;
using System.Collections.Generic;
using Kyloe.Utility;
using System.Linq;

namespace Kyloe.Syntax
{
    internal class CompilationUnitSyntax : SyntaxNode
    {
        public CompilationUnitSyntax(ImmutableArray<DeclarationStatement> globalDeclarations, ImmutableArray<FunctionDefinition> functionDefinitions)
        {
            GlobalDeclarations = globalDeclarations;
            FunctionDefinitions = functionDefinitions;
        }

        public ImmutableArray<DeclarationStatement> GlobalDeclarations { get; }
        public ImmutableArray<FunctionDefinition> FunctionDefinitions { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.CompilationUnitSyntax;

        public override SourceLocation Location
        {
            get
            {
                if (GlobalDeclarations.IsEmpty && FunctionDefinitions.IsEmpty)
                    return SourceLocation.FromLength(0, 0);
                else if (GlobalDeclarations.IsEmpty)
                    return SourceLocation.CreateAround(FunctionDefinitions.First().Location, FunctionDefinitions.Last().Location);
                else if (FunctionDefinitions.IsEmpty)
                    return SourceLocation.CreateAround(GlobalDeclarations.First().Location, GlobalDeclarations.Last().Location);


                var first = GlobalDeclarations.First().Location.Start <= FunctionDefinitions.First().Location.Start
                            ? GlobalDeclarations.First().Location
                            : FunctionDefinitions.First().Location;

                var last = GlobalDeclarations.Last().Location.End >= FunctionDefinitions.Last().Location.End
                            ? GlobalDeclarations.Last().Location
                            : FunctionDefinitions.Last().Location;

                return SourceLocation.CreateAround(first, last);
            }
        }

        public override IEnumerable<SyntaxNodeChild> GetChildren()
        {
            foreach (var global in GlobalDeclarations)
                yield return new SyntaxNodeChild(global);

            foreach (var func in FunctionDefinitions)
                yield return new SyntaxNodeChild(func);
        }
    }
}