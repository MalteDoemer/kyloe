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

        /// <summary>
        /// Import a function (static method) from an external assembly
        /// into the global namespace of the kyloe program. This function 
        /// should import all overloads.
        /// </summary>
        /// <param name="name">The full path of the function to import (ex: 'Kyloe.println')</param>
        public abstract void ImportFunction(string name);

        public abstract void CreateProgram(CompilationOptions opts, LoweredCompilationUnit compilationUnit);

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