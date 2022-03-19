using System;
using Mono.Cecil;
using Kyloe.Semantics;
using System.Diagnostics;

namespace Kyloe.Symbols
{
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

            var charType = GetOrDeclareType(rootNamespace, ts.Char.Resolve());
            var i8Type = GetOrDeclareType(rootNamespace, ts.SByte.Resolve());
            var i16Type = GetOrDeclareType(rootNamespace, ts.Int16.Resolve());
            var i32Type = GetOrDeclareType(rootNamespace, ts.Int32.Resolve());
            var i64Type = GetOrDeclareType(rootNamespace, ts.Int64.Resolve());
            var u8Type = GetOrDeclareType(rootNamespace, ts.Byte.Resolve());
            var u16Type = GetOrDeclareType(rootNamespace, ts.UInt16.Resolve());
            var u32Type = GetOrDeclareType(rootNamespace, ts.UInt32.Resolve());
            var u64Type = GetOrDeclareType(rootNamespace, ts.UInt64.Resolve());
            var floatType = GetOrDeclareType(rootNamespace, ts.Single.Resolve());
            var doubleType = GetOrDeclareType(rootNamespace, ts.Double.Resolve());
            var boolType = GetOrDeclareType(rootNamespace, ts.Boolean.Resolve());
            var stringType = GetOrDeclareType(rootNamespace, ts.String.Resolve());
            var emtpyType = GetOrDeclareType(rootNamespace, ts.Void.Resolve());

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
            if (reference.IsArray)
            {
                var arrayRef = (ArrayType)reference;
                var elementType = GetOrDeclareType(rootNamespace, arrayRef.ElementType);
                return new ArrayTypeSymbol(elementType);
            }
            else if (reference.IsByReference)
            {

                throw new System.NotImplementedException();
            }
            else if (reference.IsFunctionPointer)
            {
                throw new System.NotImplementedException();
            }
            else if (reference.IsPointer)
            {
                throw new System.NotImplementedException();
            }
            else if (reference.IsNested)
            {
                var declaringType = GetOrDeclareType(rootNamespace, reference.DeclaringType);
                return declaringType.GetOrAddNestedType(reference.Name);
            }
            else
            {
                var current = rootNamespace;

                foreach (var ns in reference.Namespace.Split("."))
                    current = current.GetOrAddNamespace(ns);

                return current.GetOrAddTypeSymbol(reference.Name);
            }
        }

        private static TypeSymbol DefineTypeRecursive(NamespaceSymbol rootNamespace, TypeDefinition typeDefinition)
        {
            var symbol = GetOrDeclareType(rootNamespace, typeDefinition);
            return DefineTypeSymbol(rootNamespace, typeDefinition, symbol);

            // switch (symbol.Kind)
            // {
            //     case SymbolKind.TypeSymbol:
            //         return DefineTypeSymbol(rootNamespace, typeDefinition, (TypeSymbol)symbol);
            //     default:
            //         throw new Exception($"unexpected symbol kind: {symbol.Kind}");
            // }
        }

        private static TypeSymbol DefineTypeSymbol(NamespaceSymbol rootNamespace, TypeDefinition typeDefinition, TypeSymbol symbol)
        {
            symbol.SetAccessModifiers(GetAccessModifiersForType(typeDefinition));

            foreach (var nestedType in typeDefinition.NestedTypes)
            {
                if (nestedType.HasGenericParameters)
                    continue;
                var type = DefineTypeRecursive(rootNamespace, nestedType);
            }

            foreach (var methodDefinition in typeDefinition.Methods)
            {
                if (methodDefinition.HasGenericParameters)
                    continue;

                symbol.AddMethod(CreateMethod(rootNamespace, methodDefinition));
            }

            return symbol;
        }

        private static bool IsOperator(MethodDefinition methodDefinition)
        {
            return methodDefinition.IsSpecialName
                && (SemanticInfo.IsBinaryOperationMethodName(methodDefinition.Name)
                || SemanticInfo.IsUnaryOperationMethodName(methodDefinition.Name));
        }

        private static AccessModifiers GetAccessModifiersForType(TypeDefinition typeDefinition)
        {
            if (typeDefinition.IsPublic)
                return AccessModifiers.Public;
            else if (typeDefinition.IsNotPublic)
                return AccessModifiers.Internal;
            else if (typeDefinition.IsNestedPublic)
                return AccessModifiers.Public;
            else if (typeDefinition.IsNestedPrivate)
                return AccessModifiers.Private;
            else if (typeDefinition.IsNestedAssembly)
                return AccessModifiers.Internal;
            else if (typeDefinition.IsNestedFamily)
                return AccessModifiers.Protected;
            else if (typeDefinition.IsNestedFamilyOrAssembly)
                return AccessModifiers.InternalOrProtected;
            else if (typeDefinition.IsNestedFamilyAndAssembly)
                return AccessModifiers.InternalAndProtected;

            throw new ArgumentException(nameof(typeDefinition), $"Invalid type: {typeDefinition}");
        }

        private static MethodSymbol CreateMethod(NamespaceSymbol rootNamespace, MethodDefinition methodDefinition)
        {
            var method = new MethodSymbol(methodDefinition.Name);

            method.SetReturnType(GetOrDeclareType(rootNamespace, methodDefinition.ReturnType));
            method.SetStatic(methodDefinition.IsStatic);
            method.SetOperator(IsOperator(methodDefinition));

            foreach (var paramDefinition in methodDefinition.Parameters)
            {
                var parameter = new ParameterSymbol(paramDefinition.Name);
                parameter.SetType(GetOrDeclareType(rootNamespace, paramDefinition.ParameterType));
            }

            return method;
        }

        private static MethodSymbol CreateBuiltinBinaryOperator(BinaryOperation op, TypeSymbol ret, TypeSymbol left, TypeSymbol right)
        {
            var name = SemanticInfo.GetBinaryOperationMethodName(op);

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

        private static MethodSymbol CreateBuiltinUnaryOperator(UnaryOperation op, TypeSymbol ret, TypeSymbol arg)
        {
            var name = SemanticInfo.GetUnaryOperationMethodName(op);

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


    }
}