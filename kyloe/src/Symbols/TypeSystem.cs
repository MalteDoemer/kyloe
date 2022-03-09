using System;
using Mono.Cecil;
using Kyloe.Semantics;

namespace Kyloe.Symbols
{
    public partial class TypeSystem
    {
        public INamespaceSymbol RootNamespace { get; }

        public IErrorTypeSymbol Error { get; }

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

            AddNegation(i8Type);
            AddBasicArithmeticOperations(i8Type);
            AddBitwiseOperations(i8Type);
            AddComparisonOperations(i8Type, boolType);
            AddEqualityOperations(i8Type, boolType);

            AddNegation(i16Type);
            AddBasicArithmeticOperations(i16Type);
            AddBitwiseOperations(i16Type);
            AddComparisonOperations(i16Type, boolType);
            AddEqualityOperations(i16Type, boolType);

            AddNegation(i32Type);
            AddBasicArithmeticOperations(i32Type);
            AddBitwiseOperations(i32Type);
            AddComparisonOperations(i32Type, boolType);
            AddEqualityOperations(i32Type, boolType);

            AddNegation(i64Type);
            AddBasicArithmeticOperations(i64Type);
            AddBitwiseOperations(i64Type);
            AddComparisonOperations(i64Type, boolType);
            AddEqualityOperations(i64Type, boolType);

            AddBasicArithmeticOperations(u8Type);
            AddBitwiseOperations(u8Type);
            AddComparisonOperations(u8Type, boolType);
            AddEqualityOperations(u8Type, boolType);

            AddBasicArithmeticOperations(u16Type);
            AddBitwiseOperations(u16Type);
            AddComparisonOperations(u16Type, boolType);
            AddEqualityOperations(u16Type, boolType);

            AddBasicArithmeticOperations(u32Type);
            AddBitwiseOperations(u32Type);
            AddComparisonOperations(u32Type, boolType);
            AddEqualityOperations(u32Type, boolType);

            AddBasicArithmeticOperations(u64Type);
            AddBitwiseOperations(u64Type);
            AddComparisonOperations(u64Type, boolType);
            AddEqualityOperations(u64Type, boolType);

            AddNegation(floatType);
            AddBasicArithmeticOperations(floatType);
            AddComparisonOperations(floatType, boolType);
            AddEqualityOperations(floatType, boolType);

            AddNegation(doubleType);
            AddBasicArithmeticOperations(doubleType);
            AddComparisonOperations(doubleType, boolType);
            AddEqualityOperations(doubleType, boolType);

            AddEqualityOperations(boolType, boolType);
            AddLogicalNot(boolType);

            // TODO: char and string

            RootNamespace = rootNamespace;
            Error = new ErrorTypeSymbol();
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

        private static MethodSymbol CreateBuiltinBinaryOperator(BinaryOperation op, TypeSymbol ret, TypeSymbol left, TypeSymbol right)
        {
            var name = SemanticInfo.GetBinaryOperationMethodName(op);

            if (name is null)
                throw new ArgumentException(nameof(op), "op has no corresponding method");

            var method = new MethodSymbol(name)
                         .SetReturnType(ret)
                         .AddParameter(new ParameterSymbol("left").SetType(left))
                         .AddParameter(new ParameterSymbol("right").SetType(right))
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
                         .SetBuiltinOperator(true);

            return method;
        }

        private static void AddBasicArithmeticOperations(TypeSymbol type)
        {
            type.AddMethod(CreateBuiltinBinaryOperator(BinaryOperation.Addition, type, type, type))
                .AddMethod(CreateBuiltinBinaryOperator(BinaryOperation.Subtraction, type, type, type))
                .AddMethod(CreateBuiltinBinaryOperator(BinaryOperation.Multiplication, type, type, type))
                .AddMethod(CreateBuiltinBinaryOperator(BinaryOperation.Division, type, type, type))
                .AddMethod(CreateBuiltinBinaryOperator(BinaryOperation.Modulo, type, type, type));
        }

        private static void AddBitwiseOperations(TypeSymbol type)
        {
            type.AddMethod(CreateBuiltinBinaryOperator(BinaryOperation.BitwiseAnd, type, type, type))
                .AddMethod(CreateBuiltinBinaryOperator(BinaryOperation.BitwiseOr, type, type, type))
                .AddMethod(CreateBuiltinBinaryOperator(BinaryOperation.BitwiseXor, type, type, type))
                .AddMethod(CreateBuiltinUnaryOperator(UnaryOperation.BitwiseNot, type, type));
        }

        private static void AddComparisonOperations(TypeSymbol type, TypeSymbol booleanType)
        {
            type.AddMethod(CreateBuiltinBinaryOperator(BinaryOperation.LessThan, booleanType, type, type))
                .AddMethod(CreateBuiltinBinaryOperator(BinaryOperation.LessThanOrEqual, booleanType, type, type))
                .AddMethod(CreateBuiltinBinaryOperator(BinaryOperation.GreaterThan, booleanType, type, type))
                .AddMethod(CreateBuiltinBinaryOperator(BinaryOperation.GreaterThanOrEqual, booleanType, type, type));
        }

        private static void AddEqualityOperations(TypeSymbol type, TypeSymbol booleanType)
        {
            type.AddMethod(CreateBuiltinBinaryOperator(BinaryOperation.NotEqual, booleanType, type, type))
                .AddMethod(CreateBuiltinBinaryOperator(BinaryOperation.Equal, booleanType, type, type));
        }

        private static void AddNegation(TypeSymbol type)
        {
            type.AddMethod(CreateBuiltinUnaryOperator(UnaryOperation.Negation, type, type));
        }

        private static void AddLogicalNot(TypeSymbol type)
        {
            type.AddMethod(CreateBuiltinUnaryOperator(UnaryOperation.LogicalNot, type, type));
        }

        public static TypeSystem Create(AssemblyDefinition mainAssembly)
        {
            return new TypeSystem(mainAssembly);
        }
    }
}