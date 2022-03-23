
using System;
using System.Collections.Generic;
using System.Linq;
using Kyloe.Semantics;

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
            var systemAssembly = ts.Boolean.Resolve().Module.Assembly;

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

            foreach (var type in systemAssembly.Modules.SelectMany(mod => mod.Types))
            {
                if (type.HasGenericParameters || type.IsNotPublic)
                    continue;

                DefineType(type);
            }

            foreach (var type in assemblyDefinitions.SelectMany(asm => asm.Modules).SelectMany(mod => mod.Types))
            {
                if (type.HasGenericParameters || type.IsNotPublic)
                    continue;

                DefineType(type);
            }

            foreach (var binary in BuiltinOperatorInfo.BinaryOperations)
            {
                var left = GetBuiltinType(binary.lhs);
                var right = GetBuiltinType(binary.rhs);
                var ret = GetBuiltinType(binary.ret);

                foreach (var op in binary.ops)
                    left.Scope.DeclareSymbol(CreateBuiltinBinaryOperator(op, ret, left, right));
            }

            foreach (var unary in BuiltinOperatorInfo.UnaryOperations)
            {
                var arg = GetBuiltinType(unary.arg);
                var ret = GetBuiltinType(unary.ret);

                foreach (var op in unary.ops)
                    arg.Scope.DeclareSymbol(CreateBuiltinUnaryOperator(op, ret, arg));
            }

        }

        private ClassType GetBuiltinType(BuiltinType type)
        {
            switch (type)
            {
                case BuiltinType.Char: return Char;
                case BuiltinType.I8: return I8;
                case BuiltinType.I16: return I16;
                case BuiltinType.I32: return I32;
                case BuiltinType.I64: return I64;
                case BuiltinType.U8: return U8;
                case BuiltinType.U16: return U16;
                case BuiltinType.U32: return U32;
                case BuiltinType.U64: return U64;
                case BuiltinType.Float: return Float;
                case BuiltinType.Double: return Double;
                case BuiltinType.Bool: return Bool;
                case BuiltinType.String: return String;
                default: throw new Exception($"unexpected builtin type: {type}");
            }
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
                    parent.Scope.DeclareSymbol(new TypeNameSymbol(classType));
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
                        currentNS.Scope.DeclareSymbol(new NamespaceSymbol(newNS));
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
                    currentNS.Scope.DeclareSymbol(new TypeNameSymbol(classType));
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
                    var fieldSymbol = new FieldSymbol(
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

                    var propertySymbol = new PropertySymbol(
                        name: property.Name,
                        type: GetOrDeclareType(property.PropertyType),
                        isStatic: !property.HasThis,
                        getMethod: getMethod,
                        setMethod: setMethod
                    );

                    classType.Scope.DeclareSymbol(propertySymbol);
                }

                var methodDictionary = new Dictionary<string, MethodGroupType>();
                var operationDictionary = new Dictionary<string, OperationSymbol>();

                foreach (var method in definition.Methods)
                {
                    if (method.HasGenericParameters)
                        continue;

                    var methodType = CreateMethodType(method);

                    if (method.IsSpecialName)
                    {
                        if (SemanticInfo.GetOperationFromMethodName(method.Name) is BoundOperation op)
                        {
                            if (operationDictionary.TryGetValue(method.Name, out var operationSymbol))
                            {
                                operationSymbol.MethodGroup.Methods.Add(methodType);
                            }
                            else
                            {
                                var operation = new OperationSymbol(op, new MethodGroupType(method.Name, classType));
                                operation.MethodGroup.Methods.Add(methodType);
                                operationDictionary.Add(method.Name, operation);
                            }

                            continue;
                        }
                    }


                    if (methodDictionary.TryGetValue(method.Name, out var methodGroup))
                    {
                        methodGroup.Methods.Add(methodType);
                    }
                    else
                    {
                        var group = new MethodGroupType(method.Name, classType);
                        group.Methods.Add(methodType);
                        methodDictionary.Add(method.Name, group);
                    }
                }

                foreach (var group in methodDictionary.Values)
                {
                    classType.Scope.DeclareSymbol(new MethodGroupSymbol(group));
                }

                foreach (var op in operationDictionary.Values)
                {
                    classType.Scope.DeclareSymbol(op);
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

        private static OperationSymbol CreateBuiltinBinaryOperator(BoundOperation op, TypeSpecifier ret, TypeSpecifier left, TypeSpecifier right)
        {
            var name = SemanticInfo.GetMethodNameFromOperation(op);
            var method = new MethodType(name, AccessModifiers.Public, left, true, ret);
            method.ParameterTypes.Add(left);
            method.ParameterTypes.Add(right);

            var group = new MethodGroupType(name, left);
            group.Methods.Add(method);

            return new OperationSymbol(op, group);
        }

        private static OperationSymbol CreateBuiltinUnaryOperator(BoundOperation op, TypeSpecifier ret, TypeSpecifier arg)
        {
            var name = SemanticInfo.GetMethodNameFromOperation(op);
            var method = new MethodType(name, AccessModifiers.Public, arg, true, ret);
            method.ParameterTypes.Add(arg);

            var group = new MethodGroupType(name, arg);
            group.Methods.Add(method);

            return new OperationSymbol(op, group);
        }


    }
}