using System;
using Mono.Cecil;
using Kyloe.Semantics;
using System.Collections.Immutable;
using System.Linq;

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

        private enum BuiltinType
        {
            Char,
            I8,
            I16,
            I32,
            I64,
            U8,
            U16,
            U32,
            U64,
            Float,
            Double,
            Bool,
            String,
        }

        private static class BuiltinOperatorInfo
        {
            public static readonly ImmutableArray<BinaryOperation> ArithmeticOperations = ImmutableArray.Create<BinaryOperation>
            (
                BinaryOperation.Addition,
                BinaryOperation.Subtraction,
                BinaryOperation.Multiplication,
                BinaryOperation.Division,
                BinaryOperation.Modulo
            );


            public static readonly ImmutableArray<BinaryOperation> BitwiseOperations = ImmutableArray.Create<BinaryOperation>
            (
                BinaryOperation.BitwiseAnd,
                BinaryOperation.BitwiseOr,
                BinaryOperation.BitwiseXor
            );

            public static readonly ImmutableArray<BinaryOperation> EqualityOperations = ImmutableArray.Create<BinaryOperation>
            (
                BinaryOperation.Equal,
                BinaryOperation.NotEqual
            );

            public static readonly ImmutableArray<BinaryOperation> ComparisonOperations = ImmutableArray.Create<BinaryOperation>
            (
                BinaryOperation.LessThan,
                BinaryOperation.GreaterThan,
                BinaryOperation.LessThanOrEqual,
                BinaryOperation.GreaterThanOrEqual
            );

            public static readonly ImmutableArray<UnaryOperation> Negation = ImmutableArray.Create<UnaryOperation>
            (
                UnaryOperation.Negation
            );

            public static readonly ImmutableArray<UnaryOperation> BitwiseNot = ImmutableArray.Create<UnaryOperation>
            (
                UnaryOperation.BitwiseNot
            );

            public static readonly ImmutableArray<UnaryOperation> LogicalNot = ImmutableArray.Create<UnaryOperation>
            (
                UnaryOperation.LogicalNot
            );

            public static readonly ImmutableArray<UnaryOperation> NegationAndBitwiseNot = Negation.Concat(BitwiseNot).ToImmutableArray();

            public static readonly ImmutableArray<BinaryOperation> EqualityAndComparisonOperations = EqualityOperations.Concat(ComparisonOperations).ToImmutableArray();

            public static readonly ImmutableArray<BinaryOperation> ArithmeticAndBitwiseOperations = ArithmeticOperations.Concat(BitwiseOperations).ToImmutableArray();

            public static readonly ImmutableArray<(ImmutableArray<BinaryOperation> ops, BuiltinType ret, BuiltinType lhs, BuiltinType rhs)> BinaryOperations = ImmutableArray.Create<(ImmutableArray<BinaryOperation>, BuiltinType, BuiltinType, BuiltinType)>
            (
                (ArithmeticAndBitwiseOperations, BuiltinType.I8, BuiltinType.I8, BuiltinType.I8),
                (ArithmeticAndBitwiseOperations, BuiltinType.I16, BuiltinType.I16, BuiltinType.I16),
                (ArithmeticAndBitwiseOperations, BuiltinType.I32, BuiltinType.I32, BuiltinType.I32),
                (ArithmeticAndBitwiseOperations, BuiltinType.I64, BuiltinType.I64, BuiltinType.I64),

                (EqualityAndComparisonOperations, BuiltinType.Bool, BuiltinType.I8, BuiltinType.I8),
                (EqualityAndComparisonOperations, BuiltinType.Bool, BuiltinType.I16, BuiltinType.I16),
                (EqualityAndComparisonOperations, BuiltinType.Bool, BuiltinType.I32, BuiltinType.I32),
                (EqualityAndComparisonOperations, BuiltinType.Bool, BuiltinType.I64, BuiltinType.I64),


                (ArithmeticAndBitwiseOperations, BuiltinType.U8, BuiltinType.U8, BuiltinType.U8),
                (ArithmeticAndBitwiseOperations, BuiltinType.U16, BuiltinType.U16, BuiltinType.U16),
                (ArithmeticAndBitwiseOperations, BuiltinType.U32, BuiltinType.U32, BuiltinType.U32),
                (ArithmeticAndBitwiseOperations, BuiltinType.U64, BuiltinType.U64, BuiltinType.U64),

                (EqualityAndComparisonOperations, BuiltinType.Bool, BuiltinType.U8, BuiltinType.U8),
                (EqualityAndComparisonOperations, BuiltinType.Bool, BuiltinType.U16, BuiltinType.U16),
                (EqualityAndComparisonOperations, BuiltinType.Bool, BuiltinType.U32, BuiltinType.U32),
                (EqualityAndComparisonOperations, BuiltinType.Bool, BuiltinType.U64, BuiltinType.U64),

                (ArithmeticOperations, BuiltinType.Float, BuiltinType.Float, BuiltinType.Float),
                (ArithmeticOperations, BuiltinType.Double, BuiltinType.Double, BuiltinType.Double),

                (EqualityAndComparisonOperations, BuiltinType.Bool, BuiltinType.Float, BuiltinType.Float),
                (EqualityAndComparisonOperations, BuiltinType.Bool, BuiltinType.Double, BuiltinType.Double),

                (EqualityOperations, BuiltinType.Bool, BuiltinType.Bool, BuiltinType.Bool),
                (EqualityOperations, BuiltinType.Bool, BuiltinType.Char, BuiltinType.Char),
                (EqualityOperations, BuiltinType.Bool, BuiltinType.String, BuiltinType.String)


            );

            public static readonly ImmutableArray<(ImmutableArray<UnaryOperation> ops, BuiltinType ret, BuiltinType arg)> UnaryOperations = ImmutableArray.Create<(ImmutableArray<UnaryOperation> ops, BuiltinType ret, BuiltinType arg)>
            (
                (NegationAndBitwiseNot, BuiltinType.I8, BuiltinType.I8),
                (NegationAndBitwiseNot, BuiltinType.I16, BuiltinType.I16),
                (NegationAndBitwiseNot, BuiltinType.I32, BuiltinType.I32),
                (NegationAndBitwiseNot, BuiltinType.I64, BuiltinType.I64),

                (BitwiseNot, BuiltinType.U8, BuiltinType.U8),
                (BitwiseNot, BuiltinType.U16, BuiltinType.U16),
                (BitwiseNot, BuiltinType.U32, BuiltinType.U32),
                (BitwiseNot, BuiltinType.U64, BuiltinType.U64),

                (Negation, BuiltinType.Float, BuiltinType.Float),
                (Negation, BuiltinType.Double, BuiltinType.Double),

                (LogicalNot, BuiltinType.Bool, BuiltinType.Bool)
            );
        }
    }
}