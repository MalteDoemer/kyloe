using System;
using System.Linq;
using Kyloe.Symbols;
using Kyloe.Syntax;

namespace Kyloe.Semantics
{
    public static class SemanticInfo
    {
        public static ITypeSymbol GetTypeFromLiteral(TypeSystem typeSystem, SyntaxTokenType tokenType)
        {
            switch (tokenType)
            {
                case SyntaxTokenType.IntLiteral: return typeSystem.I64;
                case SyntaxTokenType.FloatLiteral: return typeSystem.Double;
                case SyntaxTokenType.BoolLiteral: return typeSystem.Bool;
                case SyntaxTokenType.StringLiteral: return typeSystem.String;
                default:
                    throw new System.Exception($"Unexpected literal type: {tokenType}");
            }
        }

        public static string? GetUnaryOperationMethodName(UnaryOperation operation)
        {
            switch (operation)
            {
                case UnaryOperation.Negation: return "op_UnaryNegation";
                case UnaryOperation.BitwiseNot: return "op_OnesComplement";
                case UnaryOperation.LogicalNot: return "op_LogicalNot";
                default:
                    return null;
            }
        }

        public static string? GetBinaryOperationMethodName(BinaryOperation operation)
        {
            switch (operation)
            {
                case BinaryOperation.Addition: return "op_Addition";
                case BinaryOperation.Subtraction: return "op_Subtraction";
                case BinaryOperation.Multiplication: return "op_Multiply";
                case BinaryOperation.Division: return "op_Division";
                case BinaryOperation.Modulo: return "op_Modulus";
                case BinaryOperation.BitwiseAnd: return "op_BitwiseAnd";
                case BinaryOperation.BitwiseOr: return "op_BitwiseOr";
                case BinaryOperation.BitwiseXor: return "op_ExclusiveOr";
                case BinaryOperation.Equal: return "op_Equality";
                case BinaryOperation.NotEqual: return "op_Inequality";
                case BinaryOperation.LessThanOrEqual: return "op_LessThanOrEqual";
                case BinaryOperation.GreaterThanOrEqual: return "op_GreaterThanOrEqual";
                case BinaryOperation.LessThan: return "op_LessThan";
                case BinaryOperation.GreaterThan: return "op_GreaterThan";
                default:
                    return null;
            }
        }

        internal static BinaryOperation GetBinaryOperation(SyntaxTokenType type)
        {
            switch (type)
            {
                case SyntaxTokenType.Less: return BinaryOperation.LessThan;
                case SyntaxTokenType.Greater: return BinaryOperation.GreaterThan;
                case SyntaxTokenType.DoubleEqual: return BinaryOperation.Equal;
                case SyntaxTokenType.LessEqual: return BinaryOperation.LessThanOrEqual;
                case SyntaxTokenType.GreaterEqual: return BinaryOperation.GreaterThanOrEqual;
                case SyntaxTokenType.NotEqual: return BinaryOperation.NotEqual;
                case SyntaxTokenType.Plus: return BinaryOperation.Addition;
                case SyntaxTokenType.Minus: return BinaryOperation.Subtraction;
                case SyntaxTokenType.Star: return BinaryOperation.Multiplication;
                case SyntaxTokenType.Slash: return BinaryOperation.Division;
                case SyntaxTokenType.Percent: return BinaryOperation.Modulo;
                case SyntaxTokenType.Ampersand: return BinaryOperation.BitwiseAnd;
                case SyntaxTokenType.DoubleAmpersand: return BinaryOperation.LogicalAnd;
                case SyntaxTokenType.Pipe: return BinaryOperation.BitwiseOr;
                case SyntaxTokenType.DoublePipe: return BinaryOperation.LogicalOr;
                case SyntaxTokenType.Hat: return BinaryOperation.BitwiseXor;
                default:
                    throw new System.Exception($"Unexpected binary operation type: {type}");
            }
        }

        internal static UnaryOperation GetUnaryOperation(SyntaxTokenType type)
        {
            switch (type)
            {
                case SyntaxTokenType.Tilde: return UnaryOperation.BitwiseNot;
                case SyntaxTokenType.Bang: return UnaryOperation.LogicalNot;
                case SyntaxTokenType.Minus: return UnaryOperation.Negation;
                default:
                    throw new System.Exception($"Unexpected unary operation type: {type}");
            }
        }
    }
}