
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kyloe.Symbols
{
    internal partial class TypeSystem
    {
        public NamespaceType RootNamespace { get; }
        public ErrorType Error { get; }
        public ClassType Empty { get; }
        public ClassType Char { get; }
        public ClassType I8 { get; }
        public ClassType I16 { get; }
        public ClassType I32 { get; }
        public ClassType I64 { get; }
        public ClassType U8 { get; }
        public ClassType U16 { get; }
        public ClassType U32 { get; }
        public ClassType U64 { get; }
        public ClassType Float { get; }
        public ClassType Double { get; }
        public ClassType Bool { get; }
        public ClassType String { get; }

        public static TypeSystem Create(Mono.Cecil.AssemblyDefinition mainAssembly)
        {
            return new TypeSystem(mainAssembly, Array.Empty<Mono.Cecil.AssemblyDefinition>());
        }

        private TypeSystem(Mono.Cecil.AssemblyDefinition mainAssembly, Mono.Cecil.AssemblyDefinition[] assemblyDefinitions)
        {
            var ts = mainAssembly.MainModule.TypeSystem;

            RootNamespace = new NamespaceType("", null);
            Error = new ErrorType();

            Empty = (ClassType)GetOrDeclareType(ts.Void);
            Char = (ClassType)GetOrDeclareType(ts.Char);
            I8 = (ClassType)GetOrDeclareType(ts.SByte);
            I16 = (ClassType)GetOrDeclareType(ts.Int16);
            I32 = (ClassType)GetOrDeclareType(ts.Int32);
            I64 = (ClassType)GetOrDeclareType(ts.Int64);
            U8 = (ClassType)GetOrDeclareType(ts.Byte);
            U16 = (ClassType)GetOrDeclareType(ts.UInt16);
            U32 = (ClassType)GetOrDeclareType(ts.UInt32);
            U64 = (ClassType)GetOrDeclareType(ts.UInt64);
            Float = (ClassType)GetOrDeclareType(ts.Single);
            Double = (ClassType)GetOrDeclareType(ts.Double);
            Bool = (ClassType)GetOrDeclareType(ts.Boolean);
            String = (ClassType)GetOrDeclareType(ts.String);

        }

        private TypeSpecifier GetOrDeclareType(Mono.Cecil.TypeReference reference)
        {
            if (reference.IsArray)
            {
                var arrayRef = (Mono.Cecil.ArrayType)reference;

                if (!arrayRef.IsVector)
                    return Error;

                var elementType = GetOrDeclareType(arrayRef.ElementType);
                return new ArrayType(elementType);

            }
            else if (reference.IsByReference)
            {
                var byRef = (Mono.Cecil.ByReferenceType)reference;

                var elementType = GetOrDeclareType(byRef.ElementType);
                return new ByRefType(elementType);
            }
            else if (reference.IsPointer)
            {
                var pointer = (Mono.Cecil.PointerType)reference;
                var elementType = GetOrDeclareType(pointer.ElementType);
                return new PointerType(elementType);
            }
            else if (reference.IsFunctionPointer)
            {
                return Error;
            }
            else if (reference.IsNested)
            {
                var parent = (ClassType)GetOrDeclareType(reference.DeclaringType);

                var sym = parent.Scope.LookupSymbol(reference.Name);

                if (sym is null)
                {
                    var classType = new ClassType(reference.Name, GetAccessModifiers(reference.Resolve()), parent);
                    parent.Scope.DeclareSymbol(new Symbols.TypeNameSymbol(classType));
                    return classType;
                }

                return sym.Type;
            }
            else
            {
                var currentNS = RootNamespace;

                foreach (var ns in reference.Namespace.Split("."))
                {
                    var next = currentNS.Scope.LookupSymbol(ns);

                    if (next is null)
                    {
                        var newNS = new NamespaceType(ns, currentNS);
                        currentNS.Scope.DeclareSymbol(new Symbols.NamespaceSymbol(newNS));
                        currentNS = newNS;
                    }
                    else
                    {
                        currentNS = (NamespaceType)next.Type;
                    }
                }

                var sym = currentNS.Scope.LookupSymbol(reference.Name);

                if (sym is null)
                {
                    var typeDef = reference.Resolve();
                    var classType = new ClassType(reference.Name, GetAccessModifiers(typeDef), currentNS);
                    currentNS.Scope.DeclareSymbol(new Symbols.TypeNameSymbol(classType));
                    return classType;
                }

                return sym.Type;
            }
        }

        private void DefineType(Mono.Cecil.TypeReference reference)
        {
            if (GetOrDeclareType(reference) is ClassType classType)
            {
                var definition = reference.Resolve();

                foreach (var nested in definition.NestedTypes)
                {
                    if (nested.IsNotPublic || nested.HasGenericParameters)
                        continue;

                    DefineType(nested);
                }

                foreach (var field in definition.Fields)
                {
                    var fieldSymbol = new Symbols.FieldSymbol(
                        field.Name,
                        GetOrDeclareType(field.FieldType),
                        field.IsInitOnly,
                        field.IsStatic,
                        GetAccessModifiers(field)
                    );

                    classType.Scope.DeclareSymbol(fieldSymbol);
                }

                foreach (var property in definition.Properties)
                {
                    var getMethod = property.GetMethod is null ? null : CreateMethodType(property.GetMethod);
                    var setMethod = property.SetMethod is null ? null : CreateMethodType(property.SetMethod);

                    var propertySymbol = new Symbols.PropertySymbol(
                        name: property.Name,
                        type: GetOrDeclareType(property.PropertyType),
                        isStatic: !property.HasThis,
                        getMethod: getMethod,
                        setMethod: setMethod
                    );

                    classType.Scope.DeclareSymbol(propertySymbol);
                }

                var methodDictionary = new Dictionary<string, MethodGroupType>();

                foreach (var method in definition.Methods)
                {
                    if (method.HasGenericParameters)
                        continue;

                    var methodType = CreateMethodType(method);

                    if (methodDictionary.TryGetValue(method.Name, out var methodGroup))
                        methodGroup.Methods.Add(methodType);
                    else
                    {
                        var group = new MethodGroupType(method.Name, classType);
                        group.Methods.Add(methodType);
                        methodDictionary.Add(method.Name, group);
                    }
                }

                foreach (var group in methodDictionary.Values)
                {
                    classType.Scope.DeclareSymbol(new Symbols.MethodGroupSymbol(group));
                }
            }
        }

        private MethodType CreateMethodType(Mono.Cecil.MethodDefinition methodDef)
        {

            var method = new MethodType(
                name: methodDef.Name,
                accessModifiers: GetAccessModifiers(methodDef),
                parent: GetOrDeclareType(methodDef.DeclaringType),
                isStatic: methodDef.IsStatic,
                returnType: GetOrDeclareType(methodDef.ReturnType)
            );

            foreach (var param in methodDef.Parameters)
            {
                var type = GetOrDeclareType(param.ParameterType);
                method.ParameterTypes.Add(type);
            }

            return method;
        }

        private AccessModifiers GetAccessModifiers(Mono.Cecil.TypeDefinition type)
        {
            if (type.IsPublic)
                return AccessModifiers.Public;
            else if (type.IsNotPublic)
                return AccessModifiers.Internal;
            else if (type.IsNestedPublic)
                return AccessModifiers.Public;
            else if (type.IsNestedPrivate)
                return AccessModifiers.Private;
            else if (type.IsNestedFamily)
                return AccessModifiers.Protected;
            else if (type.IsNestedAssembly)
                return AccessModifiers.Internal;
            else if (type.IsNestedFamilyOrAssembly)
                return AccessModifiers.InternalOrProtected;
            else if (type.IsNestedFamilyAndAssembly)
                return AccessModifiers.InternalAndProtected;

            throw new Exception($"Invalid access modifiers for type: '{type}'");
        }

        private AccessModifiers GetAccessModifiers(Mono.Cecil.FieldDefinition field)
        {
            if (field.IsPublic)
                return AccessModifiers.Public;
            else if (field.IsPrivate)
                return AccessModifiers.Private;
            else if (field.IsFamily)
                return AccessModifiers.Protected;
            else if (field.IsAssembly)
                return AccessModifiers.Internal;
            else if (field.IsFamilyOrAssembly)
                return AccessModifiers.InternalOrProtected;
            else if (field.IsFamilyAndAssembly)
                return AccessModifiers.InternalAndProtected;

            throw new Exception($"Invalid access modifiers for field: '{field}'");
        }

        private AccessModifiers GetAccessModifiers(Mono.Cecil.MethodDefinition method)
        {
            if (method.IsPublic)
                return AccessModifiers.Public;
            else if (method.IsPrivate)
                return AccessModifiers.Private;
            else if (method.IsFamily)
                return AccessModifiers.Protected;
            else if (method.IsAssembly)
                return AccessModifiers.Internal;
            else if (method.IsFamilyOrAssembly)
                return AccessModifiers.InternalOrProtected;
            else if (method.IsFamilyAndAssembly)
                return AccessModifiers.InternalAndProtected;

            throw new Exception($"Invalid access modifiers for method: '{method}'");
        }



    }
}