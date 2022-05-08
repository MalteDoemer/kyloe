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

    internal sealed class TypeResolver
    {
        private readonly Symbols.TypeSystem kyloeTypeSystem;
        private readonly Mono.Cecil.TypeSystem cecilTypeSystem;
        private readonly IEnumerable<AssemblyDefinition> assemblies;

        private readonly Dictionary<Symbols.TypeSpecifier, TypeReference> typeCache;
        private readonly Dictionary<Symbols.FunctionType, MethodReference> functionCache;

        public TypeResolver(Symbols.TypeSystem kyloeTypeSystem, Mono.Cecil.TypeSystem cecilTypeSystem, IEnumerable<AssemblyDefinition> assemblies)
        {
            this.kyloeTypeSystem = kyloeTypeSystem;
            this.cecilTypeSystem = cecilTypeSystem;
            this.assemblies = assemblies;
            this.typeCache = new Dictionary<TypeSpecifier, TypeReference>();
            this.functionCache = new Dictionary<Symbols.FunctionType, MethodReference>();
        }

        public TypeReference ResolveType(Symbols.TypeSpecifier type)
        {
            if (typeCache.TryGetValue(type, out var cached))
                return cached;

            var resolved = ResolveTypeImpl(type);

            typeCache.Add(type, resolved);

            return resolved;
        }

        private TypeReference ResolveTypeImpl(Symbols.TypeSpecifier type)
        {
            switch (type.Kind)
            {
                case Symbols.TypeKind.BuiltinType:
                    return ResolveBuiltinType((Symbols.BuiltinType)type);
                case Symbols.TypeKind.ArrayType:
                    return ResolveArrayType((Symbols.ArrayType)type);
                default:
                    throw new Exception($"unexpected kind: {type.Kind}");
            }
        }

        private TypeReference ResolveArrayType(Symbols.ArrayType type)
        {
            throw new NotImplementedException();
        }

        private TypeReference ResolveBuiltinType(BuiltinType type)
        {
            switch (type.Name)
            {
                case "void":
                    return cecilTypeSystem.Void;
                case "char":
                    return cecilTypeSystem.Char;
                case "i8":
                    return cecilTypeSystem.SByte;
                case "i16":
                    return cecilTypeSystem.Int16;
                case "i32":
                    return cecilTypeSystem.Int32;
                case "i64":
                    return cecilTypeSystem.Int64;
                case "u8":
                    return cecilTypeSystem.Byte;
                case "u16":
                    return cecilTypeSystem.UInt16;
                case "u32":
                    return cecilTypeSystem.UInt32;
                case "u64":
                    return cecilTypeSystem.UInt64;
                case "float":
                    return cecilTypeSystem.Single;
                case "double":
                    return cecilTypeSystem.Double;
                case "bool":
                    return cecilTypeSystem.Boolean;
                case "string":
                    return cecilTypeSystem.String;
                default:
                    throw new Exception($"unexpected builtin type: {type.Name}");
            }
        }

        public MethodReference ResolveFunction(Symbols.FunctionType func)
        {
            if (functionCache.TryGetValue(func, out var cached))
                return cached;

            var resolved = ResolveFunctionImpl(func);

            functionCache.Add(func, resolved);

            return resolved;
        }

        private MethodReference ResolveFunctionImpl(Symbols.FunctionType func)
        {
            if (func.IsBuiltin) {
                return ResolveBuiltinFunction(func);
            } else {
                throw new System.NotImplementedException();
            }
        }

        private MethodReference ResolveBuiltinFunction(FunctionType func)
        {
            throw new NotImplementedException();
        }
    }
}