using System;
using System.Collections.Generic;
using Kyloe.Lowering;
using Kyloe.Symbols;
using Mono.Cecil;

namespace Kyloe.Codegen
{
    internal sealed class CodeGenerator
    {
        private readonly Symbols.TypeSystem typeSystem;
        private readonly AssemblyDefinition assembly;


        public CodeGenerator(string programName, Symbols.TypeSystem typeSystem)
        {
            this.typeSystem = typeSystem;

            var assemblyName = new AssemblyNameDefinition(programName, new Version(0, 1));
            assembly = AssemblyDefinition.CreateAssembly(assemblyName, programName, ModuleKind.Dll);
        }
    }
}