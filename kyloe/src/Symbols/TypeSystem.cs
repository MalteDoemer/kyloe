using System;
using Mono.Cecil;
using Kyloe.Semantics;
using System.Collections.Generic;

namespace Kyloe.Symbols
{
    public partial class TypeSystem
    {
        private Dictionary<ITypeSymbol, ArrayTypeSymbol> arrayTypes;
        private Dictionary<ITypeSymbol, ByRefTypeSymbol> byRefTypes;
        private Dictionary<ITypeSymbol, PointerTypeSymbol> pointerTypes;


        private NamespaceSymbol rootNamespace;
        private ErrorTypeSymbol error;

        private ClassTypeSymbol emptyType;
        private ClassTypeSymbol charType;
        private ClassTypeSymbol i8Type;
        private ClassTypeSymbol i16Type;
        private ClassTypeSymbol i32Type;
        private ClassTypeSymbol i64Type;
        private ClassTypeSymbol u8Type;
        private ClassTypeSymbol u16Type;
        private ClassTypeSymbol u32Type;
        private ClassTypeSymbol u64Type;
        private ClassTypeSymbol floatType;
        private ClassTypeSymbol doubleType;
        private ClassTypeSymbol boolType;
        private ClassTypeSymbol stringType;

        private TypeSystem(AssemblyDefinition mainAssembly)
        {
            var ts = mainAssembly.MainModule.TypeSystem;

            arrayTypes = new Dictionary<ITypeSymbol, ArrayTypeSymbol>();
            byRefTypes = new Dictionary<ITypeSymbol, ByRefTypeSymbol>();
            pointerTypes = new Dictionary<ITypeSymbol, PointerTypeSymbol>();

            rootNamespace = new NamespaceSymbol("");
            error = new ErrorTypeSymbol();

            emptyType = (ClassTypeSymbol)DefineType(ts.Void);
            charType = (ClassTypeSymbol)DefineType(ts.Char);
            i8Type = (ClassTypeSymbol)DefineType(ts.SByte);
            i16Type = (ClassTypeSymbol)DefineType(ts.Int16);
            i32Type = (ClassTypeSymbol)DefineType(ts.Int32);
            i64Type = (ClassTypeSymbol)DefineType(ts.Int64);
            u8Type = (ClassTypeSymbol)DefineType(ts.Byte);
            u16Type = (ClassTypeSymbol)DefineType(ts.UInt16);
            u32Type = (ClassTypeSymbol)DefineType(ts.UInt32);
            u64Type = (ClassTypeSymbol)DefineType(ts.UInt64);
            floatType = (ClassTypeSymbol)DefineType(ts.Single);
            doubleType = (ClassTypeSymbol)DefineType(ts.Double);
            boolType = (ClassTypeSymbol)DefineType(ts.Boolean);
            stringType = (ClassTypeSymbol)DefineType(ts.String);

            foreach (var binary in BuiltinOperatorInfo.BinaryOperations)
            {
                var left = GetBuiltinType(binary.lhs);
                var right = GetBuiltinType(binary.rhs);
                var ret = GetBuiltinType(binary.ret);

                foreach (var op in binary.ops)
                    left.AddOperation(CreateBuiltinBinaryOperation(op, ret, left, right));
            }

            foreach (var unary in BuiltinOperatorInfo.UnaryOperations)
            {
                var arg = GetBuiltinType(unary.arg);
                var ret = GetBuiltinType(unary.ret);

                foreach (var op in unary.ops)
                    arg.AddOperation(CreateBuiltinUnaryOperation(op, ret, arg));
            }
        }

        public INamespaceSymbol RootNamespace => rootNamespace;
        public IErrorTypeSymbol Error => error;
        public ITypeSymbol Empty => emptyType;
        public ITypeSymbol Char => charType;
        public ITypeSymbol I8 => i8Type;
        public ITypeSymbol I16 => i16Type;
        public ITypeSymbol I32 => i32Type;
        public ITypeSymbol I64 => i64Type;
        public ITypeSymbol U8 => u8Type;
        public ITypeSymbol U16 => u16Type;
        public ITypeSymbol U32 => u32Type;
        public ITypeSymbol U64 => u64Type;
        public ITypeSymbol Float => floatType;
        public ITypeSymbol Double => doubleType;
        public ITypeSymbol Bool => boolType;
        public ITypeSymbol String => stringType;

        private ClassTypeSymbol GetBuiltinType(BuiltinType type)
        {
            switch (type)
            {
                case BuiltinType.Char: return charType;
                case BuiltinType.I8: return i8Type;
                case BuiltinType.I16: return i16Type;
                case BuiltinType.I32: return i32Type;
                case BuiltinType.I64: return i64Type;
                case BuiltinType.U8: return u8Type;
                case BuiltinType.U16: return u16Type;
                case BuiltinType.U32: return u32Type;
                case BuiltinType.U64: return u64Type;
                case BuiltinType.Float: return floatType;
                case BuiltinType.Double: return doubleType;
                case BuiltinType.Bool: return boolType;
                case BuiltinType.String: return stringType;
                default: throw new Exception($"unexpected builtin type: {type}");
            }
        }

        private ITypeSymbol GetOrDeclareType(TypeReference reference)
        {
            if (reference.IsArray)
            {
                var arrayRef = (ArrayType)reference;

                if (!arrayRef.IsVector)
                    throw new System.NotImplementedException();

                return CreateArrayType(GetOrDeclareType(arrayRef.ElementType));
            }
            else if (reference.IsByReference)
            {
                var byRef = (ByReferenceType)reference;
                return CreateByRefType(GetOrDeclareType(byRef.ElementType));
            }
            else if (reference.IsPointer)
            {
                var pointer = (PointerType)reference;
                return CreatePointerType(GetOrDeclareType(pointer.ElementType));
            }
            else if (reference.IsFunctionPointer)
            {
                throw new System.NotImplementedException();
            }
            else if (reference.IsNested)
            {
                var outerType = GetOrDeclareType(reference.DeclaringType) as ClassTypeSymbol;

                if (outerType is null)
                    throw new Exception($"Declaring outer type '{reference.DeclaringType}' of the nested type '{reference}' didn't result in a ClassTypeSymbol.");

                return outerType.GetOrAddNestedClass(reference.Name);
            }
            else
            {
                var current = rootNamespace;

                foreach (var ns in reference.Namespace.Split("."))
                    current = current.GetOrAddNamespace(ns);

                return current.GetOrAddClassType(reference.Name);
            }
        }

        private ITypeSymbol DefineType(TypeReference reference)
        {
            var type = GetOrDeclareType(reference);

            if (type.Kind == SymbolKind.ClassTypeSymbol)
                return DefineClassTypeSymbol((ClassTypeSymbol)type, reference);

            return type;
        }

        private ITypeSymbol DefineClassTypeSymbol(ClassTypeSymbol type, TypeReference reference)
        {
            var definition = reference.Resolve();

            foreach (var nesetedType in definition.NestedTypes)
            {
                DefineType(nesetedType);
            }

            foreach (var field in definition.Fields)
            {
                if (field.IsCompilerControlled || field.IsSpecialName || field.IsRuntimeSpecialName || field.IsWindowsRuntimeProjection)
                {
                    Console.WriteLine($"Found special field: {field}, compiler={field.IsCompilerControlled}, special={field.IsSpecialName}, rt_special={field.IsRuntimeSpecialName}, windows={field.IsWindowsRuntimeProjection}");
                    continue;
                }

                var fieldSymbol = new FieldSymbol(field.Name);
                fieldSymbol.SetAccessModifiers(GetAccessModifiers(field))
                           .SetReadonly(field.IsInitOnly)
                           .SetStatic(field.IsStatic)
                           .SetType(GetOrDeclareType(field.FieldType));
            }

            foreach (var property in definition.Properties)
            {
                if (property.IsSpecialName || property.IsRuntimeSpecialName || property.IsWindowsRuntimeProjection)
                {
                    Console.WriteLine($"Found special property: {property}, special={property.IsSpecialName}, rt_special={property.IsRuntimeSpecialName}, windows={property.IsWindowsRuntimeProjection}");
                    continue;
                }

                var propertySymbol = new PropertySymbol(property.Name);

                if (property.GetMethod is null && property.SetMethod is null)
                    throw new Exception($"Property has neither get nor set method: {property}");

                if (property.GetMethod is not null)
                    propertySymbol.SetGetterMethod(CreateMethodSymbol(property.GetMethod));

                if (property.SetMethod is not null)
                    propertySymbol.SetSetterMethod(CreateMethodSymbol(property.SetMethod));


                propertySymbol.SetStatic(property.HasThis)
                              .SetType(GetOrDeclareType(property.PropertyType));
            }

            foreach (var method in definition.Methods)
            {
                // TODO
            }

            return type;
        }

        public IArrayTypeSymbol CreateArrayType(ITypeSymbol type)
        {
            // TODO: add System.Array methods
            // TODO: add indexing operation

            if (arrayTypes.TryGetValue(type, out var array))
                return array;

            var arrayType = new ArrayTypeSymbol(type);
            arrayTypes.Add(type, arrayType);
            return arrayType;
        }

        public IByRefTypeSymbol CreateByRefType(ITypeSymbol type)
        {
            if (byRefTypes.TryGetValue(type, out var byRef))
                return byRef;

            var byRefType = new ByRefTypeSymbol(type);
            byRefTypes.Add(type, byRefType);
            return byRefType;
        }

        public IPointerTypeSymbol CreatePointerType(ITypeSymbol type)
        {
            // TODO: add pointer operations
            if (pointerTypes.TryGetValue(type, out var pointer))
                return pointer;

            var pointerType = new PointerTypeSymbol(type);
            pointerTypes.Add(type, pointerType);
            return pointerType;
        }

        private MethodSymbol CreateMethodSymbol(MethodDefinition method)
        {
            var methodSymbol = new MethodSymbol(method.Name);
            methodSymbol
                .SetAccessModifiers(GetAccessModifiers(method))
                .SetReturnType(GetOrDeclareType(method.ReturnType))
                .SetStatic(method.IsStatic);

            foreach (var param in method.Parameters)
            {
                var paramSymbol = new ParameterSymbol(param.Name);
                paramSymbol.SetType(GetOrDeclareType(param.ParameterType));
            }

            return methodSymbol;
        }

        private AccessModifiers GetAccessModifiers(TypeDefinition property)
        {
            if (property.IsPublic)
                return AccessModifiers.Public;
            else if (property.IsNotPublic)
                return AccessModifiers.Internal;
            else if (property.IsNestedPublic)
                return AccessModifiers.Public;
            else if (property.IsNestedPrivate)
                return AccessModifiers.Private;
            else if (property.IsNestedFamily)
                return AccessModifiers.Protected;
            else if (property.IsNestedAssembly)
                return AccessModifiers.Internal;
            else if (property.IsNestedFamilyOrAssembly)
                return AccessModifiers.InternalOrProtected;
            else if (property.IsNestedFamilyAndAssembly)
                return AccessModifiers.InternalAndProtected;

            throw new Exception($"Invalid access modifiers for property: '{property}'");
        }

        private AccessModifiers GetAccessModifiers(FieldDefinition field)
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

        private AccessModifiers GetAccessModifiers(MethodDefinition method)
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

        private static OperationSymbol CreateBuiltinBinaryOperation(BoundOperation op, ITypeSymbol ret, ITypeSymbol left, ITypeSymbol right)
        {
            var name = SemanticInfo.GetMethodNameFromOperation(op);

            var method = new MethodSymbol(name)
                        .SetReturnType(ret)
                        .AddParameter(new ParameterSymbol("left").SetType(left))
                        .AddParameter(new ParameterSymbol("right").SetType(right))
                        .SetStatic(true);


            var operation = new OperationSymbol(op)
                        .SetUnerlyingMethod(method)
                        .SetBuiltin(true);

            return operation;
        }

        private static OperationSymbol CreateBuiltinUnaryOperation(BoundOperation op, ITypeSymbol ret, ITypeSymbol type)
        {
            var name = SemanticInfo.GetMethodNameFromOperation(op);

            var method = new MethodSymbol(name)
                        .SetReturnType(ret)
                        .AddParameter(new ParameterSymbol("arg").SetType(type))
                        .SetStatic(true);

            var operation = new OperationSymbol(op)
                        .SetUnerlyingMethod(method)
                        .SetBuiltin(true);

            return operation;
        }

        public static TypeSystem Create(AssemblyDefinition mainAssembly)
        {
            return new TypeSystem(mainAssembly);
        }
    }


    /*
    public partial class TypeSystem
    {
        public INamespaceSymbol RootNamespace { get; }

        public IErrorTypeSymbol Error { get; }
        public ITypeSymbol Empty { get; }
        public ITypeSymbol Char { get; }
        public ITypeSymbol I8 { get; }
        public ITypeSymbol I16 { get; }
        public ITypeSymbol I32 { get; }
        public ITypeSymbol I64 { get; }
        public ITypeSymbol U8 { get; }
        public ITypeSymbol U16 { get; }
        public ITypeSymbol U32 { get; }
        public ITypeSymbol U64 { get; }
        public ITypeSymbol Float { get; }
        public ITypeSymbol Double { get; }
        public ITypeSymbol Bool { get; }
        public ITypeSymbol String { get; }

        public static TypeSystem Create(AssemblyDefinition mainAssembly)
        {
            return new TypeSystem(mainAssembly);
        }

        private TypeSystem(AssemblyDefinition mainAssembly)
        {
            var ts = mainAssembly.MainModule.TypeSystem;

            var rootNamespace = new NamespaceSymbol("");

            var charType = GetOrDeclareType(rootNamespace, ts.Char);
            var i8Type = GetOrDeclareType(rootNamespace, ts.SByte);
            var i16Type = GetOrDeclareType(rootNamespace, ts.Int16);
            var i32Type = GetOrDeclareType(rootNamespace, ts.Int32);
            var i64Type = GetOrDeclareType(rootNamespace, ts.Int64);
            var u8Type = GetOrDeclareType(rootNamespace, ts.Byte);
            var u16Type = GetOrDeclareType(rootNamespace, ts.UInt16);
            var u32Type = GetOrDeclareType(rootNamespace, ts.UInt32);
            var u64Type = GetOrDeclareType(rootNamespace, ts.UInt64);
            var floatType = GetOrDeclareType(rootNamespace, ts.Single);
            var doubleType = GetOrDeclareType(rootNamespace, ts.Double);
            var boolType = GetOrDeclareType(rootNamespace, ts.Boolean);
            var stringType = GetOrDeclareType(rootNamespace, ts.String);
            var emtpyType = GetOrDeclareType(rootNamespace, ts.Void);

            TypeSymbol GetBuiltinType(BuiltinType type)
            {
                switch (type)
                {
                    case BuiltinType.Char: return charType!;
                    case BuiltinType.I8: return i8Type!;
                    case BuiltinType.I16: return i16Type!;
                    case BuiltinType.I32: return i32Type!;
                    case BuiltinType.I64: return i64Type!;
                    case BuiltinType.U8: return u8Type!;
                    case BuiltinType.U16: return u16Type!;
                    case BuiltinType.U32: return u32Type!;
                    case BuiltinType.U64: return u64Type!;
                    case BuiltinType.Float: return floatType!;
                    case BuiltinType.Double: return doubleType!;
                    case BuiltinType.Bool: return boolType!;
                    case BuiltinType.String: return stringType!;
                    default: throw new Exception($"unexpected builtin type: {type}");
                }
            }

            foreach (var binary in BuiltinOperatorInfo.BinaryOperations)
            {
                var left = GetBuiltinType(binary.lhs);
                var right = GetBuiltinType(binary.rhs);
                var ret = GetBuiltinType(binary.ret);

                foreach (var op in binary.ops)
                    left.AddMethod(CreateBuiltinBinaryOperator(op, ret, left, right));
            }

            foreach (var unary in BuiltinOperatorInfo.UnaryOperations)
            {
                var arg = GetBuiltinType(unary.arg);
                var ret = GetBuiltinType(unary.ret);

                foreach (var op in unary.ops)
                    arg.AddMethod(CreateBuiltinUnaryOperator(op, ret, arg));
            }

            RootNamespace = rootNamespace;
            Error = new ErrorTypeSymbol();
            Empty = emtpyType;
            Char = charType;
            I8 = i8Type;
            I16 = i16Type;
            I32 = i32Type;
            I64 = i64Type;
            U8 = u8Type;
            U16 = u16Type;
            U32 = u32Type;
            U64 = u64Type;
            Float = floatType;
            Double = doubleType;
            Bool = boolType;
            String = stringType;
        }

        private static TypeSymbol GetOrDeclareType(NamespaceSymbol rootNamespace, TypeReference reference)
        {
            var current = rootNamespace;

            foreach (var ns in reference.Namespace.Split("."))
                current = current.GetOrAddNamespace(ns);

            return current.GetOrAddTypeSymbol(reference.Name);
        }

        private static MethodSymbol CreateBuiltinBinaryOperator(BoundOperation op, TypeSymbol ret, TypeSymbol left, TypeSymbol right)
        {
            var name = SemanticInfo.GetMethodNameFromOperation(op);

            if (name is null)
                throw new ArgumentException(nameof(op), "op has no corresponding method");

            var method = new MethodSymbol(name)
                         .SetReturnType(ret)
                         .AddParameter(new ParameterSymbol("left").SetType(left))
                         .AddParameter(new ParameterSymbol("right").SetType(right))
                         .SetStatic(true)
                         .SetOperator(true)
                         .SetBuiltinOperator(true);

            return method;
        }

        private static MethodSymbol CreateBuiltinUnaryOperator(BoundOperation op, TypeSymbol ret, TypeSymbol arg)
        {
            var name = SemanticInfo.GetMethodNameFromOperation(op);

            if (name is null)
                throw new ArgumentException(nameof(op), "op has no corresponding method");

            var method = new MethodSymbol(name)
                         .SetReturnType(ret)
                         .AddParameter(new ParameterSymbol("arg").SetType(arg))
                         .SetOperator(true)
                         .SetStatic(true)
                         .SetBuiltinOperator(true);

            return method;
        }


    }*/
}