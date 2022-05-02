using Mono.Cecil;
using Kyloe.Semantics;
using System;
using Kyloe.Diagnostics;
using Kyloe.Utility;
using Kyloe.Syntax;
using Kyloe.Symbols;
using Kyloe.Lowering;

namespace Kyloe
{
    public struct CompilationOptions
    {
        public bool RequireMain { get; set; }
    }

    public class Compilation
    {
        private readonly AssemblyDefinition assembly;
        private readonly DiagnosticResult diagnostics;
        private readonly LoweredCompilationUnit compilationUnit;

        private Compilation(AssemblyDefinition assembly, DiagnosticResult diagnostics, LoweredCompilationUnit compilationUnit)
        {
            this.assembly = assembly;
            this.diagnostics = diagnostics;
            this.compilationUnit = compilationUnit;
        }

        internal LoweredCompilationUnit GetCompilationUnit() => compilationUnit;

        public DiagnosticResult GetDiagnostics() => diagnostics;

        public static Compilation Compile(string text, CompilationOptions opts = default(CompilationOptions)) => Compile(SourceText.FromText(text), opts);

        public static Compilation Compile(SourceText text, CompilationOptions opts = default(CompilationOptions))
        {
            var assemblyName = new AssemblyNameDefinition("test", new Version(0, 1));
            var assembly = AssemblyDefinition.CreateAssembly(assemblyName, "<test>", ModuleKind.Dll);

            var typeSystem = Symbols.TypeSystem.Create();

            var diagnostics = new DiagnosticCollector(text);
            var parser = new Parser(text.GetAllText(), diagnostics);
            var rootNode = parser.Parse();
            var binder = new Binder(typeSystem, diagnostics);
            var boundCompilationUnit = binder.BindCompilationUnit(rootNode);

            if (opts.RequireMain && boundCompilationUnit.MainFunction is null)
                diagnostics.MissingMainFunction();

            var lowerer = new Lowerer(typeSystem);
            var loweredCompilationUnit = lowerer.LowerCompilationUnit(boundCompilationUnit);

            return new Compilation(assembly, diagnostics.ToResult(), loweredCompilationUnit);
        }

    }
}