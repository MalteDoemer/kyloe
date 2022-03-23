using Mono.Cecil;
using Kyloe.Semantics;
using System;
using Kyloe.Diagnostics;
using Kyloe.Utility;
using Kyloe.Syntax;
using System.Collections.Generic;
using System.Linq;

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

        public static Compilation Compile(string text) => Compile(SourceText.FromText(text), Array.Empty<AssemblyDefinition>());

        public static Compilation Compile(SourceText text, IEnumerable<AssemblyDefinition> refrenceAssemblies)
        {
            var assemblyName = new AssemblyNameDefinition("test", new Version(0, 1));
            var assembly = AssemblyDefinition.CreateAssembly(assemblyName, "<test>", ModuleKind.Dll);

            var assemblies = refrenceAssemblies.ToArray();

            if (assemblies.Length == 0)
                assemblies = new[] { assembly.MainModule.TypeSystem.Boolean.Resolve().Module.Assembly };

            var typeSystem = Symbols.TypeSystem.Create(assembly, assemblies);

            var collector = new DiagnosticCollector(text);
            var lexer = new Lexer(text.GetReader(), collector);
            var parser = new Parser(lexer, collector);
            var rootNode = parser.Parse();
            var binder = new Binder(typeSystem, collector);
            var result = binder.Bind(rootNode);

            return new Compilation(assembly, collector.ToResult(), result);
        }

    }
}