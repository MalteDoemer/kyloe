using Kyloe.Semantics;
using System.Collections.Immutable;
using System.Linq;

namespace Kyloe.Symbols
{
    internal partial class TypeSystem
    {
        private static class BuiltinOperationInfo
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

            public static readonly ImmutableArray<(ImmutableArray<BoundOperation> ops, BuiltinTypeKind ret, BuiltinTypeKind lhs, BuiltinTypeKind rhs)> BinaryOperations = ImmutableArray.Create<(ImmutableArray<BoundOperation>, BuiltinTypeKind, BuiltinTypeKind, BuiltinTypeKind)>
            (
                (ArithmeticAndBitwiseOperations, BuiltinTypeKind.I8, BuiltinTypeKind.I8, BuiltinTypeKind.I8),
                (ArithmeticAndBitwiseOperations, BuiltinTypeKind.I16, BuiltinTypeKind.I16, BuiltinTypeKind.I16),
                (ArithmeticAndBitwiseOperations, BuiltinTypeKind.I32, BuiltinTypeKind.I32, BuiltinTypeKind.I32),
                (ArithmeticAndBitwiseOperations, BuiltinTypeKind.I64, BuiltinTypeKind.I64, BuiltinTypeKind.I64),

                (EqualityAndComparisonOperations, BuiltinTypeKind.Bool, BuiltinTypeKind.I8, BuiltinTypeKind.I8),
                (EqualityAndComparisonOperations, BuiltinTypeKind.Bool, BuiltinTypeKind.I16, BuiltinTypeKind.I16),
                (EqualityAndComparisonOperations, BuiltinTypeKind.Bool, BuiltinTypeKind.I32, BuiltinTypeKind.I32),
                (EqualityAndComparisonOperations, BuiltinTypeKind.Bool, BuiltinTypeKind.I64, BuiltinTypeKind.I64),


                (ArithmeticAndBitwiseOperations, BuiltinTypeKind.U8, BuiltinTypeKind.U8, BuiltinTypeKind.U8),
                (ArithmeticAndBitwiseOperations, BuiltinTypeKind.U16, BuiltinTypeKind.U16, BuiltinTypeKind.U16),
                (ArithmeticAndBitwiseOperations, BuiltinTypeKind.U32, BuiltinTypeKind.U32, BuiltinTypeKind.U32),
                (ArithmeticAndBitwiseOperations, BuiltinTypeKind.U64, BuiltinTypeKind.U64, BuiltinTypeKind.U64),

                (EqualityAndComparisonOperations, BuiltinTypeKind.Bool, BuiltinTypeKind.U8, BuiltinTypeKind.U8),
                (EqualityAndComparisonOperations, BuiltinTypeKind.Bool, BuiltinTypeKind.U16, BuiltinTypeKind.U16),
                (EqualityAndComparisonOperations, BuiltinTypeKind.Bool, BuiltinTypeKind.U32, BuiltinTypeKind.U32),
                (EqualityAndComparisonOperations, BuiltinTypeKind.Bool, BuiltinTypeKind.U64, BuiltinTypeKind.U64),

                (ArithmeticOperations, BuiltinTypeKind.Float, BuiltinTypeKind.Float, BuiltinTypeKind.Float),
                (ArithmeticOperations, BuiltinTypeKind.Double, BuiltinTypeKind.Double, BuiltinTypeKind.Double),

                (EqualityAndComparisonOperations, BuiltinTypeKind.Bool, BuiltinTypeKind.Float, BuiltinTypeKind.Float),
                (EqualityAndComparisonOperations, BuiltinTypeKind.Bool, BuiltinTypeKind.Double, BuiltinTypeKind.Double),

                (EqualityOperations, BuiltinTypeKind.Bool, BuiltinTypeKind.Bool, BuiltinTypeKind.Bool),
                (EqualityOperations, BuiltinTypeKind.Bool, BuiltinTypeKind.Char, BuiltinTypeKind.Char),
                (EqualityOperations, BuiltinTypeKind.Bool, BuiltinTypeKind.String, BuiltinTypeKind.String)


            );

            public static readonly ImmutableArray<(ImmutableArray<BoundOperation> ops, BuiltinTypeKind ret, BuiltinTypeKind arg)> UnaryOperations = ImmutableArray.Create<(ImmutableArray<BoundOperation> ops, BuiltinTypeKind ret, BuiltinTypeKind arg)>
            (
                (NegationAndBitwiseNot, BuiltinTypeKind.I8, BuiltinTypeKind.I8),
                (NegationAndBitwiseNot, BuiltinTypeKind.I16, BuiltinTypeKind.I16),
                (NegationAndBitwiseNot, BuiltinTypeKind.I32, BuiltinTypeKind.I32),
                (NegationAndBitwiseNot, BuiltinTypeKind.I64, BuiltinTypeKind.I64),

                (BitwiseNot, BuiltinTypeKind.U8, BuiltinTypeKind.U8),
                (BitwiseNot, BuiltinTypeKind.U16, BuiltinTypeKind.U16),
                (BitwiseNot, BuiltinTypeKind.U32, BuiltinTypeKind.U32),
                (BitwiseNot, BuiltinTypeKind.U64, BuiltinTypeKind.U64),

                (Negation, BuiltinTypeKind.Float, BuiltinTypeKind.Float),
                (Negation, BuiltinTypeKind.Double, BuiltinTypeKind.Double),

                (LogicalNot, BuiltinTypeKind.Bool, BuiltinTypeKind.Bool)
            );
        }
    }


}