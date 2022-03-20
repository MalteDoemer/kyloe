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

        public static string GetMethodNameFromOperation(BoundOperation operation)
        {
            switch (operation)
            {
                case BoundOperation.Addition: return "op_Addition";
                case BoundOperation.Subtraction: return "op_Subtraction";
                case BoundOperation.Multiplication: return "op_Multiply";
                case BoundOperation.Division: return "op_Division";
                case BoundOperation.Modulo: return "op_Modulus";
                case BoundOperation.BitwiseAnd: return "op_BitwiseAnd";
                case BoundOperation.BitwiseOr: return "op_BitwiseOr";
                case BoundOperation.BitwiseXor: return "op_ExclusiveOr";
                case BoundOperation.Equal: return "op_Equality";
                case BoundOperation.NotEqual: return "op_Inequality";
                case BoundOperation.LessThanOrEqual: return "op_LessThanOrEqual";
                case BoundOperation.GreaterThanOrEqual: return "op_GreaterThanOrEqual";
                case BoundOperation.LessThan: return "op_LessThan";
                case BoundOperation.GreaterThan: return "op_GreaterThan";
                case BoundOperation.Negation: return "op_UnaryNegation";
                case BoundOperation.BitwiseNot: return "op_OnesComplement";
                case BoundOperation.LogicalNot: return "op_LogicalNot";

                case BoundOperation.LogicalAnd: return "op_LogicalAnd";
                case BoundOperation.LogicalOr: return "op_LogicalOr";
                default:
                    throw new Exception($"unexpected operation: {operation}");
            }
        }

        public static BoundOperation? GetOperationFromMethodName(string name)
        {
            switch (name)
            {
                case "op_Addition": return BoundOperation.Addition;
                case "op_Subtraction": return BoundOperation.Subtraction;
                case "op_Multiply": return BoundOperation.Multiplication;
                case "op_Division": return BoundOperation.Division;
                case "op_Modulus": return BoundOperation.Modulo;
                case "op_BitwiseAnd": return BoundOperation.BitwiseAnd;
                case "op_BitwiseOr": return BoundOperation.BitwiseOr;
                case "op_ExclusiveOr": return BoundOperation.BitwiseXor;
                case "op_Equality": return BoundOperation.Equal;
                case "op_Inequality": return BoundOperation.NotEqual;
                case "op_LessThanOrEqual": return BoundOperation.LessThanOrEqual;
                case "op_GreaterThanOrEqual": return BoundOperation.GreaterThanOrEqual;
                case "op_LessThan": return BoundOperation.LessThan;
                case "op_GreaterThan": return BoundOperation.GreaterThan;
                case "op_UnaryNegation": return BoundOperation.Negation;
                case "op_OnesComplement": return BoundOperation.BitwiseNot;
                case "op_LogicalNot": return BoundOperation.LogicalNot;
                default:
                    return null;
            }
        }

        public static bool IsUnaryOperation(this BoundOperation operation)
        {
            switch (operation)
            {
                case BoundOperation.Negation:
                case BoundOperation.BitwiseNot:
                case BoundOperation.LogicalNot:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsBinaryOperation(this BoundOperation operation)
        {
            switch (operation)
            {
                case BoundOperation.Addition:
                case BoundOperation.Subtraction:
                case BoundOperation.Multiplication:
                case BoundOperation.Division:
                case BoundOperation.Modulo:
                case BoundOperation.BitwiseAnd:
                case BoundOperation.BitwiseOr:
                case BoundOperation.BitwiseXor:
                case BoundOperation.LogicalAnd:
                case BoundOperation.LogicalOr:
                case BoundOperation.LessThan:
                case BoundOperation.GreaterThan:
                case BoundOperation.LessThanOrEqual:
                case BoundOperation.GreaterThanOrEqual:
                case BoundOperation.Equal:
                case BoundOperation.NotEqual:
                    return true;
                default:
                    return false;
            }
        }


        public static bool IsUnaryOperationMethodName(string name)
        {
            return GetOperationFromMethodName(name) is BoundOperation op && op.IsUnaryOperation();
        }

        public static bool IsBinaryOperationMethodName(string name)
        {
            return GetOperationFromMethodName(name) is BoundOperation op && op.IsBinaryOperation();
        }

        internal static BoundOperation GetBinaryOperation(SyntaxTokenType type)
        {
            switch (type)
            {
                case SyntaxTokenType.Less: return BoundOperation.LessThan;
                case SyntaxTokenType.Greater: return BoundOperation.GreaterThan;
                case SyntaxTokenType.DoubleEqual: return BoundOperation.Equal;
                case SyntaxTokenType.LessEqual: return BoundOperation.LessThanOrEqual;
                case SyntaxTokenType.GreaterEqual: return BoundOperation.GreaterThanOrEqual;
                case SyntaxTokenType.NotEqual: return BoundOperation.NotEqual;
                case SyntaxTokenType.Plus: return BoundOperation.Addition;
                case SyntaxTokenType.Minus: return BoundOperation.Subtraction;
                case SyntaxTokenType.Star: return BoundOperation.Multiplication;
                case SyntaxTokenType.Slash: return BoundOperation.Division;
                case SyntaxTokenType.Percent: return BoundOperation.Modulo;
                case SyntaxTokenType.Ampersand: return BoundOperation.BitwiseAnd;
                case SyntaxTokenType.DoubleAmpersand: return BoundOperation.LogicalAnd;
                case SyntaxTokenType.Pipe: return BoundOperation.BitwiseOr;
                case SyntaxTokenType.DoublePipe: return BoundOperation.LogicalOr;
                case SyntaxTokenType.Hat: return BoundOperation.BitwiseXor;
                default:
                    throw new System.Exception($"Unexpected binary operation type: {type}");
            }
        }

        internal static BoundOperation GetUnaryOperation(SyntaxTokenType type)
        {
            switch (type)
            {
                case SyntaxTokenType.Tilde: return BoundOperation.BitwiseNot;
                case SyntaxTokenType.Bang: return BoundOperation.LogicalNot;
                case SyntaxTokenType.Minus: return BoundOperation.Negation;
                default:
                    throw new System.Exception($"Unexpected unary operation type: {type}");
            }
        }

        internal static AssignmentOperation GetAssignmentOperation(SyntaxTokenType type)
        {
            switch (type)
            {
                case SyntaxTokenType.Equals: return AssignmentOperation.Assign;
                case SyntaxTokenType.PlusEquals: return AssignmentOperation.AddAssign;
                case SyntaxTokenType.MinusEquals: return AssignmentOperation.SubAssign;
                case SyntaxTokenType.StarEquals: return AssignmentOperation.MulAssign;
                case SyntaxTokenType.SlashEquals: return AssignmentOperation.DivAssign;
                case SyntaxTokenType.PercentEquals: return AssignmentOperation.ModAssign;
                case SyntaxTokenType.AmpersandEquals: return AssignmentOperation.AndAssign;
                case SyntaxTokenType.PipeEquals: return AssignmentOperation.OrAssign;
                case SyntaxTokenType.HatEquals: return AssignmentOperation.XorAssign;

                default:
                    throw new System.Exception($"Unexpected assignment operation type: {type}");
            }
        }

        internal static BoundOperation GetOperationForAssignment(AssignmentOperation op)
        {
            switch (op)
            {
                case AssignmentOperation.AddAssign: return BoundOperation.Addition;
                case AssignmentOperation.SubAssign: return BoundOperation.Subtraction;
                case AssignmentOperation.MulAssign: return BoundOperation.Multiplication;
                case AssignmentOperation.DivAssign: return BoundOperation.Division;
                case AssignmentOperation.ModAssign: return BoundOperation.Modulo;
                case AssignmentOperation.AndAssign: return BoundOperation.BitwiseAnd;
                case AssignmentOperation.OrAssign: return BoundOperation.BitwiseOr;
                case AssignmentOperation.XorAssign: return BoundOperation.BitwiseXor;

                default:
                    throw new Exception($"no corresponding binary operation for assignment: {op}");
            }
        }
    }
}