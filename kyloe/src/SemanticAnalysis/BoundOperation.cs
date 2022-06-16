namespace Kyloe.Semantics
{
    public enum BoundOperation 
    {
        Addition,
        Subtraction,
        Multiplication,
        Division,
        Modulo,
        BitwiseAnd,
        BitwiseOr,
        BitwiseXor,
        LogicalAnd,
        LogicalOr,
        LessThan,
        GreaterThan,
        LessThanOrEqual,
        GreaterThanOrEqual,
        Equal,
        NotEqual,

        Identity,
        Negation,
        BitwiseNot,
        LogicalNot,

        ImplicitConversion,
        ExplicitConversion,
    }
}