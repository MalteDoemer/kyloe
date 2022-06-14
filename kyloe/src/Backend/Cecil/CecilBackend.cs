using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Kyloe.Diagnostics;
using Kyloe.Lowering;
using Kyloe.Symbols;
using Mono.Cecil;

namespace Kyloe.Backend.Cecil
{
    internal class CecilBackend : Backend
    {
        private readonly Dictionary<Symbols.TypeInfo, TypeReference> types;
        private readonly Dictionary<Symbols.TypeInfo, MethodReference> callables;

        private readonly ImmutableArray<AssemblyDefinition> assemblies;
        private readonly DiagnosticCollector diagnostics;

        private readonly AssemblyDefinition assembly;

        public CecilBackend(string programName, Symbols.TypeSystem typeSystem, IEnumerable<string> libraries, DiagnosticCollector diagnostics) : base(typeSystem)
        {
            var assemblyName = new AssemblyNameDefinition(programName, new Version(0, 1));
            this.assembly = AssemblyDefinition.CreateAssembly(assemblyName, programName, ModuleKind.Dll);

            var assemblyBuilder = ImmutableArray.CreateBuilder<AssemblyDefinition>();

            foreach (var path in libraries)
            {
                try
                {
                    var asm = AssemblyDefinition.ReadAssembly(path);
                    assemblyBuilder.Add(asm);
                }
                catch (BadImageFormatException)
                {
                    diagnostics.UnableToReadLibrary(path);
                }
                catch (IOException)
                {
                    diagnostics.UnableToReadLibrary(path);
                }
            }

            this.assemblies = assemblyBuilder.ToImmutable();
            this.diagnostics = diagnostics;

            types = new Dictionary<Symbols.TypeInfo, TypeReference>();
            callables = new Dictionary<Symbols.TypeInfo, MethodReference>();

            ResolveBuiltinTypes();
        }

        public override BackendKind Kind => BackendKind.Cecil;

        public TypeReference ResolveType(TypeInfo kyloeType)
        {
            if (types.TryGetValue(kyloeType, out var reference))
                return reference;

            throw new NotImplementedException();
        }

        public MethodReference ResolveCallable(TypeInfo kyloeType)
        {
            if (callables.TryGetValue(kyloeType, out var callable))
                return callable;

            throw new NotImplementedException();
        }

        public override void CreateProgram(string programPath, LoweredCompilationUnit unit)
        {
            var mainAttrs = TypeAttributes.Public
                        | TypeAttributes.Class
                        | TypeAttributes.Abstract
                        | TypeAttributes.Sealed;

            var mainClass = new TypeDefinition("", assembly.Name.Name, mainAttrs, ResolveType(TypeSystem.Object));
            assembly.MainModule.Types.Add(mainClass);

            foreach (var func in unit.LoweredFunctions)
            {
                var method = CreateFunctionType(func.Type);
                AddCallable(func.Type, method);
                mainClass.Methods.Add(method);

                if (func.Equals(unit.MainFunction))
                    assembly.EntryPoint = method;
            }

            var staticCtor = CreateStaticConstructor();
            mainClass.Methods.Add(staticCtor);

            var ctorGenerator = new MethodGenerator(staticCtor, this);
            foreach (var stmt in unit.GlobalStatement)
                ctorGenerator.GenerateStatement(stmt);

            foreach (var func in unit.LoweredFunctions)
            {
                var method = ResolveCallable(func.Type).Resolve();
                var generator = new MethodGenerator(method, this);
                generator.GenerateFunctionBody(func);
            }

            if (diagnostics.HasErrors())
                return;

            using var file = new FileStream(programPath, FileMode.Create);
            assembly.Write(file);
        }

        private void ResolveBuiltinTypes()
        {
            var map = new Dictionary<BuiltinType, string>
            {
                { TypeSystem.Object, "System.Object" },
                { TypeSystem.Void, "System.Void" },
                { TypeSystem.Char, "System.Char" },
                { TypeSystem.I8, "System.SByte" },
                { TypeSystem.I16, "System.Int16" },
                { TypeSystem.I32, "System.Int32" },
                { TypeSystem.I64, "System.Int64" },
                { TypeSystem.U8, "System.Byte" },
                { TypeSystem.U16, "System.UInt16" },
                { TypeSystem.U32, "System.UInt32" },
                { TypeSystem.U64, "System.UInt64" },
                { TypeSystem.Float, "System.Single" },
                { TypeSystem.Double, "System.Double" },
                { TypeSystem.Bool, "System.Boolean" },
                { TypeSystem.String, "System.String" },
            };

            foreach (var (type, name) in map)
            {
                var reference = ResolveExternalType(name);

                if (reference is not null)
                    AddType(type, reference);
            }
        }

        private void AddType(Symbols.TypeInfo kyloeType, TypeReference cecilType)
        {
            types.Add(kyloeType, cecilType);
        }

        private void AddCallable(Symbols.TypeInfo kyloeType, MethodReference cecilType)
        {
            callables.Add(kyloeType, cecilType);
        }

        private TypeReference? ResolveExternalType(string metadataName)
        {
            var types = assemblies.SelectMany(asm => asm.Modules)
                                  .SelectMany(mod => mod.Types)
                                  .Where(type => type.FullName == metadataName)
                                  .ToArray();

            if (types.Length != 1)
            {
                System.Console.WriteLine($"failed to resolve type: {metadataName}");
                diagnostics.UnresolvedImport(metadataName, null);
                return null;
            }

            return assembly.MainModule.ImportReference(types.First());
        }

        private MethodDefinition CreateFunctionType(Symbols.FunctionType function)
        {
            var method = new MethodDefinition(function.Name, MethodAttributes.Static | MethodAttributes.Private, ResolveType(function.ReturnType));

            foreach (var param in function.Parameters)
                method.Parameters.Add(new ParameterDefinition(param.Name, ParameterAttributes.None, ResolveType(param.Type)));

            return method;
        }

        private MethodDefinition CreateStaticConstructor()
        {
            var attrs = MethodAttributes.SpecialName
                        | MethodAttributes.RTSpecialName
                        | MethodAttributes.Static
                        | MethodAttributes.Private
                        | MethodAttributes.HideBySig;

            return new MethodDefinition(".cctor", attrs, ResolveType(TypeSystem.Void));
        }
    }
}