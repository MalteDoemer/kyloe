using Mono.Cecil;
using Kyloe.Semantics;
using System;
using Kyloe.Diagnostics;
using Kyloe.Utility;
using Kyloe.Syntax;

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

        public static Compilation Compile(string text) => Compile(SourceText.FromText(text));

        public static Compilation Compile(SourceText text)
        {
            var assemblyName = new AssemblyNameDefinition("test", new Version(0, 1));
            var assembly = AssemblyDefinition.CreateAssembly(assemblyName, "<test>", ModuleKind.Dll);

            var collector = new DiagnosticCollector(text);
            var lexer = new Lexer(text.GetReader(), collector);
            var parser = new Parser(lexer, collector);
            var rootNode = parser.Parse();
            var binder = new Binder(assembly.MainModule.TypeSystem, collector);
            var result = binder.Bind(rootNode);

            return new Compilation(assembly, collector.ToResult(), result);
        }

    }
}