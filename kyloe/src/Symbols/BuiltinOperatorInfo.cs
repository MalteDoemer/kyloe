using Kyloe.Semantics;
using System.Collections.Immutable;
using System.Linq;

namespace Kyloe.Symbols
{
    public partial class TypeSystem
    {
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