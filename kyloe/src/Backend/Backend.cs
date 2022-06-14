using System.Collections.Generic;
using System.Diagnostics;
using Kyloe.Diagnostics;
using Kyloe.Lowering;
using Kyloe.Symbols;

namespace Kyloe.Backend
{
    internal abstract class Backend
    {
        protected Backend(TypeSystem typeSystem)
        {
            TypeSystem = typeSystem;
        }

        public abstract BackendKind Kind { get; }

        public TypeSystem TypeSystem { get; }

        public abstract void CreateProgram(string programPath, LoweredCompilationUnit compilationUnit);

        public static Backend Create(string programName, BackendKind backendKind, TypeSystem typeSystem, IEnumerable<string> libraries, DiagnosticCollector diagnostics)
        {
            switch (backendKind)
            {
                case BackendKind.Cecil:
                    var backend = new Cecil.CecilBackend(programName, typeSystem, libraries, diagnostics);
                    return backend;
                default:
                    throw new System.Exception($"Unknown backend: {backendKind}");
            }
        }
    }
}