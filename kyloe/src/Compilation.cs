using Kyloe.Semantics;
using System;
using Kyloe.Diagnostics;
using Kyloe.Utility;
using Kyloe.Syntax;
using Kyloe.Lowering;
using System.IO;
using System.Collections.Generic;
using Kyloe.Backend;

namespace Kyloe
{
    public struct CompilationOptions
    {
        public bool RequireMain { get; set; }
    }

    public class Compilation
    {
        private readonly DiagnosticResult diagnostics;
        private readonly Backend.Backend backend;
        private readonly SyntaxToken syntaxTree;
        private readonly LoweredCompilationUnit? loweredTree;

        private Compilation(Backend.Backend backend, DiagnosticResult diagnostics, SyntaxToken syntaxTree, LoweredCompilationUnit? compilationUnit)
        {
            this.diagnostics = diagnostics;
            this.syntaxTree = syntaxTree;
            this.loweredTree = compilationUnit;
            this.backend = backend;
        }

        public void WriteLoweredTree(TextWriter writer)
        {
            if (loweredTree is null)
            {
                writer.WriteLine("(null)");
                return;
            }

            var treeWriter = new LoweredTreeWriter(writer);
            treeWriter.WriteNode(loweredTree);
        }

        public void WriteSyntaxTree(TextWriter writer)
        {
            var treeWriter = new Syntax.TreeWriter(writer);
            treeWriter.Write(syntaxTree);
        }

        public LoweredNode? GetLoweredTree() => loweredTree;

        public SyntaxToken GetSyntaxTree() => syntaxTree;

        public DiagnosticResult GetDiagnostics() => diagnostics;

        public static Compilation Compile(string text, CompilationOptions opts = default(CompilationOptions))
        {
            throw new NotImplementedException();
            // return Compile(SourceText.FromText(text), Enumerable.Empty<string>(), opts);
        }

        public static Compilation Compile(string programName, string programPath, BackendKind backendKind, SourceText source, IEnumerable<string> libraries, CompilationOptions opts)
        {
            var diagnostics = new DiagnosticCollector(source);

            var typeSystem = Symbols.TypeSystem.Create();
            var backend = Backend.Backend.Create(programName, backendKind, typeSystem, libraries, diagnostics);

            var parser = new Parser(source.GetAllText(), diagnostics);
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

                backend.CreateProgram(programPath, flattenedCompilationUnit);
                return new Compilation(backend, diagnostics.ToResult(), rootNode, flattenedCompilationUnit);
            }

            return new Compilation(backend, diagnostics.ToResult(), rootNode, null);
        }
    }
}