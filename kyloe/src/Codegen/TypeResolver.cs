using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;


namespace Kyloe.Codegen
{
    internal sealed class TypeResolver
    {
        private readonly Dictionary<Symbols.TypeInfo, TypeReference> types;
        private readonly Dictionary<Symbols.TypeInfo, MethodReference> callables;

        public TypeResolver(Symbols.TypeSystem typeSystem, AssemblyDefinition assembly)
        {
            types = new Dictionary<Symbols.TypeInfo, TypeReference>();
            callables = new Dictionary<Symbols.TypeInfo, MethodReference>();
            TypeSystem = typeSystem;
            Assembly = assembly;
            InitializeBuiltinTypes();
        }

        public Symbols.TypeSystem TypeSystem { get; }
        public AssemblyDefinition Assembly { get; }

        public void AddType(Symbols.TypeInfo kyloeType, TypeReference cecilType)
        {
            types.Add(kyloeType, cecilType);
        }

        public void AddCallable(Symbols.TypeInfo kyloeType, MethodReference cecilType)
        {
            callables.Add(kyloeType, cecilType);
        }

        public TypeReference ResolveType(Symbols.TypeInfo kyloeType)
        {
            return types[kyloeType];
        }

        public MethodReference ResolveCallable(Symbols.TypeInfo kyloeType)
        {
            return callables[kyloeType];
        }


        public MethodDefinition CreateFunctionType(Symbols.FunctionType function)
        {
            var method = new MethodDefinition(function.Name, MethodAttributes.Static | MethodAttributes.Private, ResolveType(function.ReturnType));

            foreach (var param in function.Parameters)
                method.Parameters.Add(new ParameterDefinition(param.Name, ParameterAttributes.None, ResolveType(param.Type)));

            return method;
        }

        public TypeReference ResolveExternalType(string metadataName)
        {
            var types = TypeSystem.ReferenceAssemblies.SelectMany(asm => asm.Modules).SelectMany(mod => mod.Types).Where(type => type.FullName == metadataName).ToArray();

            // TODO: report missing external refrences nicely
            if (types.Length == 0)
            {
                throw new System.NotImplementedException();
            }
            else if (types.Length > 1)
            {
                throw new System.NotImplementedException();
            }

            return Assembly.MainModule.ImportReference(types.First());
        }

        private void InitializeBuiltinTypes()
        {
            AddType(TypeSystem.Object, ResolveExternalType("System.Object"));
            AddType(TypeSystem.Void, ResolveExternalType("System.Void"));
            AddType(TypeSystem.Char, ResolveExternalType("System.Char"));
            AddType(TypeSystem.I8, ResolveExternalType("System.SByte"));
            AddType(TypeSystem.I16, ResolveExternalType("System.Int16"));
            AddType(TypeSystem.I32, ResolveExternalType("System.Int32"));
            AddType(TypeSystem.I64, ResolveExternalType("System.Int64"));
            AddType(TypeSystem.U8, ResolveExternalType("System.Byte"));
            AddType(TypeSystem.U16, ResolveExternalType("System.UInt16"));
            AddType(TypeSystem.U32, ResolveExternalType("System.UInt32"));
            AddType(TypeSystem.U64, ResolveExternalType("System.UInt64"));
            AddType(TypeSystem.Float, ResolveExternalType("System.Single"));
            AddType(TypeSystem.Double, ResolveExternalType("System.Double"));
            AddType(TypeSystem.Bool, ResolveExternalType("System.Boolean"));
            AddType(TypeSystem.String, ResolveExternalType("System.String"));
        }
    }
}