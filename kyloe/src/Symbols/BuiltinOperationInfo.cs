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

            public static readonly ImmutableArray<BoundOperation> Identity = ImmutableArray.Create<BoundOperation>
            (
                BoundOperation.Identity
            );

            public static readonly ImmutableArray<BoundOperation> BitwiseNot = ImmutableArray.Create<BoundOperation>
            (
                BoundOperation.BitwiseNot
            );

            public static readonly ImmutableArray<BoundOperation> LogicalNot = ImmutableArray.Create<BoundOperation>
            (
                BoundOperation.LogicalNot
            );

            public static readonly ImmutableArray<BoundOperation> NegationAndIdenity = Negation.Concat(Identity).ToImmutableArray();

            public static readonly ImmutableArray<BoundOperation> NegationIdentityAndBitwiseNot = Negation.Concat(Identity).Concat(BitwiseNot).ToImmutableArray();

            public static readonly ImmutableArray<BoundOperation> IdentityAndBitwiseNot = Identity.Concat(BitwiseNot).ToImmutableArray();

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
                (NegationIdentityAndBitwiseNot, BuiltinTypeKind.I8, BuiltinTypeKind.I8),
                (NegationIdentityAndBitwiseNot, BuiltinTypeKind.I16, BuiltinTypeKind.I16),
                (NegationIdentityAndBitwiseNot, BuiltinTypeKind.I32, BuiltinTypeKind.I32),
                (NegationIdentityAndBitwiseNot, BuiltinTypeKind.I64, BuiltinTypeKind.I64),

                (IdentityAndBitwiseNot, BuiltinTypeKind.U8, BuiltinTypeKind.U8),
                (IdentityAndBitwiseNot, BuiltinTypeKind.U16, BuiltinTypeKind.U16),
                (IdentityAndBitwiseNot, BuiltinTypeKind.U32, BuiltinTypeKind.U32),
                (IdentityAndBitwiseNot, BuiltinTypeKind.U64, BuiltinTypeKind.U64),

                (NegationAndIdenity, BuiltinTypeKind.Float, BuiltinTypeKind.Float),
                (NegationAndIdenity, BuiltinTypeKind.Double, BuiltinTypeKind.Double),

                (LogicalNot, BuiltinTypeKind.Bool, BuiltinTypeKind.Bool)
            );

            public static readonly ImmutableArray<(BuiltinTypeKind from, BuiltinTypeKind to)> ImplicitConversions = ImmutableArray.Create<(BuiltinTypeKind from, BuiltinTypeKind to)>(
                (BuiltinTypeKind.I64, BuiltinTypeKind.I32)
            );
        }
    }


}