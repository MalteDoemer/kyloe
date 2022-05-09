using Mono.Cecil;
using Kyloe.Semantics;
using System;
using Kyloe.Diagnostics;
using Kyloe.Utility;
using Kyloe.Syntax;
using Kyloe.Symbols;
using Kyloe.Lowering;
using System.IO;
using Kyloe.Codegen;

namespace Kyloe
{
    public struct CompilationOptions
    {
        public bool RequireMain { get; set; }
    }

    public class Compilation
    {
        private readonly DiagnosticResult diagnostics;
        private readonly Symbols.TypeSystem typeSystem;
        private readonly SyntaxToken syntaxTree;
        private readonly LoweredCompilationUnit? loweredTree;

        private Compilation(Symbols.TypeSystem typeSystem, DiagnosticResult diagnostics, SyntaxToken syntaxTree, LoweredCompilationUnit? compilationUnit)
        {
            this.diagnostics = diagnostics;
            this.syntaxTree = syntaxTree;
            this.loweredTree = compilationUnit;
            this.typeSystem = typeSystem;
        }

        public void WriteLoweredTree(TextWriter writer)
        {
            if (loweredTree is not null)
            {
                var treeWriter = new LoweredTreeWriter(writer);
                treeWriter.WriteNode(loweredTree);
                return;
            }

            writer.WriteLine("(null)");
        }

        public void WriteSyntaxTree(TextWriter writer)
        {
            var treeWriter = new Syntax.TreeWriter(writer);
            treeWriter.Write(syntaxTree);
        }

        public void CreateProgram(string programName, string programPath)
        {
            var generator = new CodeGenerator(programName, typeSystem);
            if (loweredTree is not null)
                generator.GenerateCompiationUnit(loweredTree);

            generator.WriteTo(programPath);
        }

        public LoweredNode? GetLoweredTree() => loweredTree;

        public SyntaxToken GetSyntaxTree() => syntaxTree;

        public DiagnosticResult GetDiagnostics() => diagnostics;

        public static Compilation Compile(string text, CompilationOptions opts = default(CompilationOptions)) => Compile(SourceText.FromText(text), opts);

        public static Compilation Compile(SourceText text, CompilationOptions opts = default(CompilationOptions))
        {
            var typeSystem = Symbols.TypeSystem.Create();

            var diagnostics = new DiagnosticCollector(text);
            var parser = new Parser(text.GetAllText(), diagnostics);
            var rootNode = parser.Parse();
            var binder = new Binder(typeSystem, diagnostics);
            var boundCompilationUnit = binder.BindCompilationUnit(rootNode);

            if (opts.RequireMain && boundCompilationUnit.MainFunction is null)
                diagnostics.MissingMainFunction();


            if (!diagnostics.HasErrors())
            {
                var lowerer = new Lowerer(typeSystem);
                var loweredCompilationUnit = lowerer.LowerCompilationUnit(boundCompilationUnit);

                var simplifier = new LoweredTreeSimplifier(typeSystem);
                var simplifiedCompilationUnit = simplifier.RewriteCompilationUnit(loweredCompilationUnit);

                var flattener = new LoweredTreeFlattener(typeSystem);
                var flattenedCompilationUnit = flattener.RewriteCompilationUnit(simplifiedCompilationUnit);

                return new Compilation(typeSystem, diagnostics.ToResult(), rootNode, flattenedCompilationUnit);
            }

            return new Compilation(typeSystem, diagnostics.ToResult(), rootNode, null);
        }

    }
}