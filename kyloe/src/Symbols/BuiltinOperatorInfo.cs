using Kyloe.Semantics;
using System.Collections.Immutable;
using System.Linq;

namespace Kyloe.Symbols
{
    internal partial class TypeSystem
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
            public static readonly ImmutableArray<BoundOperation> ArithmeticOperations = ImmutableArray.Create<BoundOperation>
            (
                BoundOperation.Addition,
                BoundOperation.Subtraction,
                BoundOperation.Multiplication,
                BoundOperation.Division,
                BoundOperation.Modulo
            );


            public static readonly ImmutableArray<BoundOperation> BitwiseOperations = ImmutableArray.Create<BoundOperation>
            (
                BoundOperation.BitwiseAnd,
                BoundOperation.BitwiseOr,
                BoundOperation.BitwiseXor
            );

            public static readonly ImmutableArray<BoundOperation> EqualityOperations = ImmutableArray.Create<BoundOperation>
            (
                BoundOperation.Equal,
                BoundOperation.NotEqual
            );

            public static readonly ImmutableArray<BoundOperation> ComparisonOperations = ImmutableArray.Create<BoundOperation>
            (
                BoundOperation.LessThan,
                BoundOperation.GreaterThan,
                BoundOperation.LessThanOrEqual,
                BoundOperation.GreaterThanOrEqual
            );

            public static readonly ImmutableArray<BoundOperation> Negation = ImmutableArray.Create<BoundOperation>
            (
                BoundOperation.Negation
            );

            public static readonly ImmutableArray<BoundOperation> BitwiseNot = ImmutableArray.Create<BoundOperation>
            (
                BoundOperation.BitwiseNot
            );

            public static readonly ImmutableArray<BoundOperation> LogicalNot = ImmutableArray.Create<BoundOperation>
            (
                BoundOperation.LogicalNot
            );

            public static readonly ImmutableArray<BoundOperation> NegationAndBitwiseNot = Negation.Concat(BitwiseNot).ToImmutableArray();

            public static readonly ImmutableArray<BoundOperation> EqualityAndComparisonOperations = EqualityOperations.Concat(ComparisonOperations).ToImmutableArray();

            public static readonly ImmutableArray<BoundOperation> ArithmeticAndBitwiseOperations = ArithmeticOperations.Concat(BitwiseOperations).ToImmutableArray();

            public static readonly ImmutableArray<(ImmutableArray<BoundOperation> ops, BuiltinType ret, BuiltinType lhs, BuiltinType rhs)> BinaryOperations = ImmutableArray.Create<(ImmutableArray<BoundOperation>, BuiltinType, BuiltinType, BuiltinType)>
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

            public static readonly ImmutableArray<(ImmutableArray<BoundOperation> ops, BuiltinType ret, BuiltinType arg)> UnaryOperations = ImmutableArray.Create<(ImmutableArray<BoundOperation> ops, BuiltinType ret, BuiltinType arg)>
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