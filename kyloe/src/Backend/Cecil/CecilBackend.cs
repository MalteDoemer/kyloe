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
        private readonly Dictionary<TypeInfo, TypeReference> types;
        private readonly Dictionary<TypeInfo, MethodReference> callables;
        private readonly Dictionary<string, TypeInfo> reverseTypes;
        private readonly Dictionary<string, TypeInfo> reverseCallables;

        private readonly ImmutableArray<ModuleDefinition> modules;
        private readonly DiagnosticCollector diagnostics;

        private readonly AssemblyDefinition assembly;

        public CecilBackend(string programName, Symbols.TypeSystem typeSystem, IEnumerable<string> libraries, DiagnosticCollector diagnostics) : base(typeSystem)
        {
            var assemblyName = new AssemblyNameDefinition(programName, new Version(0, 1));
            var assemblyResolver = new DefaultAssemblyResolver();
            var moduleParams = new ModuleParameters();
            moduleParams.AssemblyResolver = assemblyResolver;
            this.assembly = AssemblyDefinition.CreateAssembly(assemblyName, programName, moduleParams);
            var moduleBuilder = ImmutableArray.CreateBuilder<ModuleDefinition>();

            foreach (var path in libraries)
            {
                try
                {
                    var asm = ModuleDefinition.ReadModule(path);
                    assemblyResolver.AddSearchDirectory(Path.GetDirectoryName(path));
                    moduleBuilder.Add(asm);
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

            this.modules = moduleBuilder.ToImmutable();
            this.diagnostics = diagnostics;

            types = new Dictionary<Symbols.TypeInfo, TypeReference>();
            callables = new Dictionary<Symbols.TypeInfo, MethodReference>();
            reverseTypes = new Dictionary<string, TypeInfo>();
            reverseCallables = new Dictionary<string, TypeInfo>();

            ResolveBuiltinTypes();
            ResolveBuiltinFunctions();

            ObjectEqualsMethod = ResolveCompilerFunction("System.Object.Equals", new[] { "System.Object", "System.Object" });
            StringConcatMethod = ResolveCompilerFunction("System.String.Concat", new[] { "System.String", "System.String" });
        }

        public MethodReference ObjectEqualsMethod { get; }
        public MethodReference StringConcatMethod { get; }

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

        public override void ImportFunction(string fullName)
        {
            var methods = ResolveExternalFunction(fullName);
            if (methods is null)
                return;

            var methodName = methods.First().Name;

            var callGroup = new CallableGroupType(methodName, null); // parent is null here because functions don't have parents only methods have.


            foreach (var method in methods)
            {
                var returnType = ReverseResolveType(method.ReturnType);
                var functionType = new FunctionType(callGroup, returnType);

                foreach (var (i, param) in method.Parameters.EnumerateIndex())
                {
                    var paramType = ReverseResolveType(param.ParameterType);
                    functionType.Parameters.Add(new ParameterSymbol(param.Name, i, paramType));
                }

                AddCallable(functionType, method);
                callGroup.Callables.Add(functionType);
            }


            if (!TypeSystem.GlobalScope.DeclareSymbol(new CallableGroupSymbol(callGroup)))
                diagnostics.ImportedNameAlreadyExists(methodName, null);
        }

        public override void CreateProgram(CompilationOptions opts, LoweredCompilationUnit unit)
        {
            var mainAttrs = TypeAttributes.Public
                        | TypeAttributes.Class
                        | TypeAttributes.Abstract
                        | TypeAttributes.Sealed;

            var mainClass = new TypeDefinition("", opts.ProgramName, mainAttrs, ResolveType(TypeSystem.Object));
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

            if (diagnostics.HasErrors() || !opts.GenerateOutput)
                return;

            using var file = new FileStream(opts.ProgramPath, FileMode.Create);
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

        private void ResolveBuiltinFunctions()
        {
            ImportFunction("Kyloe.Builtins.println");
        }

        private void AddType(Symbols.TypeInfo kyloeType, TypeReference cecilType)
        {
            types.Add(kyloeType, cecilType);
            reverseTypes.Add(cecilType.FullName, kyloeType);
        }

        private void AddCallable(Symbols.TypeInfo kyloeType, MethodReference cecilType)
        {
            callables.Add(kyloeType, cecilType);
            reverseCallables.Add(cecilType.FullName, kyloeType);
        }

        private TypeReference? ResolveExternalType(string metadataName)
        {
            var types = modules.SelectMany(mod => mod.Types)
                               .Where(type => type.FullName == metadataName)
                               .ToArray();

            if (types.Length != 1)
            {
                diagnostics.UnresolvedImport(metadataName, null);
                return null;
            }

            return assembly.MainModule.ImportReference(types.First());
        }

        private IEnumerable<MethodReference>? ResolveExternalFunction(string name)
        {
            var idx = name.LastIndexOf('.');

            var typeName = name[..idx];
            var methodName = name[(idx + 1)..];

            var type = ResolveExternalType(typeName);

            if (type is null)
                return null;

            var methods = type.Resolve().Methods.Where(method => method.IsPublic && method.IsStatic && method.Name == methodName);

            if (!methods.Any())
            {
                diagnostics.UnresolvedImport(name, null);
                return null;
            }

            return methods.Select(method => assembly.MainModule.ImportReference(method));
        }

        private MethodReference ResolveCompilerFunction(string name, IEnumerable<string> parameters)
        {
            var methods = ResolveExternalFunction(name);

            var method = ResolveExternalFunction(name)?
                        .Where(method => method.Parameters
                                .Select(param => param.ParameterType.FullName)
                                .SequenceEqual(parameters))
                        .FirstOrDefault();

            if (method is null)
                throw new Exception($"unable to resolve compiler function '{name}'");

            return method;
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

        private TypeInfo ReverseResolveType(TypeReference cecilType)
        {
            if (reverseTypes.TryGetValue(cecilType.FullName, out var reference))
                return reference;

            throw new NotImplementedException();
        }

        private TypeInfo ReverseResolveCallable(MethodReference cecilType)
        {
            if (reverseCallables.TryGetValue(cecilType.FullName, out var callable))
                return callable;

            throw new NotImplementedException();
        }
    }
}