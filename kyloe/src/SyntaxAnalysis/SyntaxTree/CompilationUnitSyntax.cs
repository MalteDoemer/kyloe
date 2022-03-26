using System.Collections.Immutable;
using System.Collections.Generic;
using Kyloe.Utility;
using System.Linq;

namespace Kyloe.Syntax
{
    internal class CompilationUnitSyntax : SyntaxNode
    {
        public CompilationUnitSyntax(ImmutableArray<DeclarationStatement> globalDeclarations, ImmutableArray<FunctionDeclaration> functionDeclarations)
        {
            GlobalDeclarations = globalDeclarations;
            FunctionDeclarations = functionDeclarations;
        }

        public ImmutableArray<DeclarationStatement> GlobalDeclarations { get; }
        public ImmutableArray<FunctionDeclaration> FunctionDeclarations { get; }

        public override SyntaxNodeType Type => SyntaxNodeType.CompilationUnitSyntax;

        public override SourceLocation Location
        {
            get
            {
                if (GlobalDeclarations.IsEmpty && FunctionDeclarations.IsEmpty)
                    return SourceLocation.FromLength(0, 0);
                else if (GlobalDeclarations.IsEmpty)
                    return SourceLocation.CreateAround(FunctionDeclarations.First().Location, FunctionDeclarations.Last().Location);
                else if (FunctionDeclarations.IsEmpty)
                    return SourceLocation.CreateAround(GlobalDeclarations.First().Location, GlobalDeclarations.Last().Location);


                var first = GlobalDeclarations.First().Location.Start <= FunctionDeclarations.First().Location.Start
                            ? GlobalDeclarations.First().Location
                            : FunctionDeclarations.First().Location;

                var last = GlobalDeclarations.Last().Location.End >= FunctionDeclarations.Last().Location.End
                            ? GlobalDeclarations.Last().Location
                            : FunctionDeclarations.Last().Location;

                return SourceLocation.CreateAround(first, last);
            }
        }

        public override IEnumerable<SyntaxNodeChild> GetChildren()
        {
            foreach (var global in GlobalDeclarations)
                yield return new SyntaxNodeChild(global);

            foreach (var func in FunctionDeclarations)
                yield return new SyntaxNodeChild(func);
        }
    }
}