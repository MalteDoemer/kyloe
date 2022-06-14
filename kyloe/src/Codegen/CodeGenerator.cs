using System;
using System.IO;
using Kyloe.Lowering;
using Mono.Cecil;


namespace Kyloe.Codegen
{
    internal sealed class CodeGenerator
    {
        public CodeGenerator(AssemblyDefinition assembly, TypeResolver resolver)
        {
            Assembly = assembly;
            Resolver = resolver;
        }

        public AssemblyDefinition Assembly;
        public TypeResolver Resolver { get; }

        public static CodeGenerator Create(string programName, Symbols.TypeSystem typeSystem)
        {
            var assemblyName = new AssemblyNameDefinition(programName, new Version(0, 1));
            var assembly = AssemblyDefinition.CreateAssembly(assemblyName, programName, ModuleKind.Dll);
            var resolver = new TypeResolver(typeSystem, assembly);
            return new CodeGenerator(assembly, resolver);
        }

        public void GenerateCompiationUnit(LoweredCompilationUnit unit)
        {
            var baseType = Resolver.ResolveType(Resolver.TypeSystem.Object);
            var mainClass = new TypeDefinition("", Assembly.Name.Name, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Abstract | TypeAttributes.Sealed, baseType);
            Assembly.MainModule.Types.Add(mainClass);

            foreach (var func in unit.LoweredFunctions)
            {
                var method = Resolver.CreateFunctionType(func.Type);
                Resolver.AddCallable(func.Type, method);
                mainClass.Methods.Add(method);

                if (func.Equals(unit.MainFunction))
                    Assembly.EntryPoint = method;
            }


            var attrs = MethodAttributes.SpecialName
                        | MethodAttributes.RTSpecialName
                        | MethodAttributes.Static
                        | MethodAttributes.Private
                        | MethodAttributes.HideBySig;

            var staticCtor = new MethodDefinition(".cctor", attrs, Resolver.ResolveType(Resolver.TypeSystem.Void));
            var ctorGenerator = new MethodGenerator(staticCtor, Resolver);

            mainClass.Methods.Add(staticCtor);
            
            foreach (var stmt in unit.GlobalStatement)
                ctorGenerator.GenerateStatement(stmt);



            foreach (var func in unit.LoweredFunctions)
            {
                var method = Resolver.ResolveCallable(func.Type).Resolve();
                var generator = new MethodGenerator(method, Resolver);
                generator.GenerateFunctionBody(func);
            }
        }

        public void WriteTo(string path)
        {
            using var file = new FileStream(path, FileMode.Create);
            WriteTo(file);
        }

        public void WriteTo(Stream stream)
        {
            Assembly.Write(stream);
        }
    }

}