using Mono.Cecil;
using Kyloe.Semantics;
using System;
using Kyloe.Diagnostics;

namespace Kyloe
{
    public class Compilation
    {
        private readonly AssemblyDefinition assembly;
        private readonly DiagnosticResult diagnostics;
        private readonly BoundNode result;

        private Compilation(AssemblyDefinition assembly, DiagnosticResult diagnostics, BoundNode result)
        {
            this.assembly = assembly;
            this.diagnostics = diagnostics;
            this.result = result;
        }

        public DiagnosticResult GetDiagnostics() => diagnostics;

        public static Compilation Compile(SyntaxTree tree)
        {
            var assemblyName = new AssemblyNameDefinition("test", new Version(0, 1));
            var assembly = AssemblyDefinition.CreateAssembly(assemblyName, "<test>", ModuleKind.Dll);

            var collector = new DiagnosticCollector(tree.GetSourceText());
            collector.AddRange(tree.GetDiagnostics().GetAll());
            var binder = new Binder(assembly.MainModule.TypeSystem, collector);

            var result = binder.Bind(tree.GetRoot());

            return new Compilation(assembly, collector.ToResult(), result);
        }

    }
}