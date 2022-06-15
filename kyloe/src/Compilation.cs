using Kyloe.Semantics;
using Kyloe.Diagnostics;
using Kyloe.Utility;
using Kyloe.Syntax;
using Kyloe.Lowering;
using System.IO;
using System.Collections.Generic;
using Kyloe.Backend;
using System.Linq;

namespace Kyloe
{
    public class CompilationOptions
    {

        public CompilationOptions()
        {
            ProgramName = "program";
            ProgramPath = "";
            BackendKind = BackendKind.Cecil;
            RequireMain = false;
            GenerateOutput = false;
        }

        public string ProgramName { get; set; }
        public string ProgramPath { get; set; }
        public BackendKind BackendKind { get; set; }
        public bool GenerateOutput { get; set; }
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

        public static Compilation Compile(string text, CompilationOptions? opts = null)
        {
            if (opts is null)
                opts = new CompilationOptions();

            var refs = ReferenceAssmblyFinder.GetReferenceAssemblies();

            return Compile(new[] { SourceText.FromText(text) }, refs, opts);
        }

        public static Compilation Compile(IEnumerable<SourceText> sources, IEnumerable<string> libraries, CompilationOptions opts)
        {
            var diagnostics = new DiagnosticCollector();

            var typeSystem = Symbols.TypeSystem.Create();
            var backend = Backend.Backend.Create(opts.ProgramName, opts.BackendKind, typeSystem, libraries, diagnostics);

            var syntaxTree = SyntaxTree.Parse(sources, diagnostics);

            var binder = new Binder(typeSystem, diagnostics);
            var boundCompilationUnit = binder.BindCompilationUnit(syntaxTree.GetRoot());

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

                backend.CreateProgram(opts, flattenedCompilationUnit);
                return new Compilation(backend, diagnostics.ToResult(), syntaxTree.GetRoot(), flattenedCompilationUnit);
            }

            return new Compilation(backend, diagnostics.ToResult(), syntaxTree.GetRoot(), null);
        }
    }
}