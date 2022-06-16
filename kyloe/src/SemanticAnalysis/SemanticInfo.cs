using System;
using System.Linq;
using Kyloe.Symbols;
using Kyloe.Syntax;

namespace Kyloe.Semantics
{
    public static class SemanticInfo
    {
        internal static TypeInfo GetTypeFromLiteral(TypeSystem typeSystem, SyntaxTokenKind tokenKind)
        {
            switch (tokenKind)
            {
                case SyntaxTokenKind.Int: return typeSystem.I64;
                case SyntaxTokenKind.Float: return typeSystem.Double;
                case SyntaxTokenKind.Bool: return typeSystem.Bool;
                case SyntaxTokenKind.String: return typeSystem.String;
                default:
                    throw new System.Exception($"Unexpected literal type: {tokenKind}");
            }
        }

        public static string GetFunctionNameFromOperation(BoundOperation operation)
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
                case BoundOperation.Identity: return "op_UnaryPlus";
                case BoundOperation.BitwiseNot: return "op_OnesComplement";
                case BoundOperation.LogicalNot: return "op_LogicalNot";

                case BoundOperation.LogicalAnd: return "op_LogicalAnd";
                case BoundOperation.LogicalOr: return "op_LogicalOr";

                case BoundOperation.ImplicitConversion: return "op_Implicit";
                case BoundOperation.ExplicitConversion: return "op_Explicit";

                default:
                    throw new Exception($"unexpected operation: {operation}");
            }
        }

        public static BoundOperation? GetOperationFromFunctionName(string name)
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
                case "op_UnaryPlus": return BoundOperation.Identity;
                case "op_OnesComplement": return BoundOperation.BitwiseNot;
                case "op_LogicalNot": return BoundOperation.LogicalNot;
                case "op_Implicit": return BoundOperation.ImplicitConversion;
                case "op_Explicit": return BoundOperation.ExplicitConversion;
                default:
                    return null;
            }
        }

        public static bool IsUnaryOperation(this BoundOperation operation)
        {
            switch (operation)
            {
                case BoundOperation.Negation:
                case BoundOperation.Identity:
                case BoundOperation.BitwiseNot:
                case BoundOperation.LogicalNot:

                case BoundOperation.ImplicitConversion:
                case BoundOperation.ExplicitConversion:
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

        public static bool IsUnaryOperationName(string name)
        {
            return GetOperationFromFunctionName(name) is BoundOperation op && op.IsUnaryOperation();
        }

        public static bool IsBinaryOperationName(string name)
        {
            return GetOperationFromFunctionName(name) is BoundOperation op && op.IsBinaryOperation();
        }

        internal static BoundOperation GetBinaryOperation(SyntaxTokenKind type)
        {
            switch (type)
            {
                case SyntaxTokenKind.Less: return BoundOperation.LessThan;
                case SyntaxTokenKind.Greater: return BoundOperation.GreaterThan;
                case SyntaxTokenKind.DoubleEqual: return BoundOperation.Equal;
                case SyntaxTokenKind.LessEqual: return BoundOperation.LessThanOrEqual;
                case SyntaxTokenKind.GreaterEqual: return BoundOperation.GreaterThanOrEqual;
                case SyntaxTokenKind.NotEqual: return BoundOperation.NotEqual;
                case SyntaxTokenKind.Plus: return BoundOperation.Addition;
                case SyntaxTokenKind.Minus: return BoundOperation.Subtraction;
                case SyntaxTokenKind.Star: return BoundOperation.Multiplication;
                case SyntaxTokenKind.Slash: return BoundOperation.Division;
                case SyntaxTokenKind.Percent: return BoundOperation.Modulo;
                case SyntaxTokenKind.Ampersand: return BoundOperation.BitwiseAnd;
                case SyntaxTokenKind.DoubleAmpersand: return BoundOperation.LogicalAnd;
                case SyntaxTokenKind.Pipe: return BoundOperation.BitwiseOr;
                case SyntaxTokenKind.DoublePipe: return BoundOperation.LogicalOr;
                case SyntaxTokenKind.Hat: return BoundOperation.BitwiseXor;
                default:
                    throw new System.Exception($"Unexpected binary operation type: {type}");
            }
        }

        internal static BoundOperation GetUnaryOperation(SyntaxTokenKind type)
        {
            switch (type)
            {
                case SyntaxTokenKind.Tilde: return BoundOperation.BitwiseNot;
                case SyntaxTokenKind.Bang: return BoundOperation.LogicalNot;
                case SyntaxTokenKind.Minus: return BoundOperation.Negation;
                case SyntaxTokenKind.Plus: return BoundOperation.Identity;
                default:
                    throw new System.Exception($"Unexpected unary operation type: {type}");
            }
        }

        internal static AssignmentOperation GetAssignmentOperation(SyntaxTokenKind type)
        {
            switch (type)
            {
                case SyntaxTokenKind.Equal: return AssignmentOperation.Assign;
                case SyntaxTokenKind.PlusEqual: return AssignmentOperation.AddAssign;
                case SyntaxTokenKind.MinusEqual: return AssignmentOperation.SubAssign;
                case SyntaxTokenKind.StarEqual: return AssignmentOperation.MulAssign;
                case SyntaxTokenKind.SlashEqual: return AssignmentOperation.DivAssign;
                case SyntaxTokenKind.PercentEqual: return AssignmentOperation.ModAssign;
                case SyntaxTokenKind.AmpersandEqual: return AssignmentOperation.AndAssign;
                case SyntaxTokenKind.PipeEqual: return AssignmentOperation.OrAssign;
                case SyntaxTokenKind.HatEqual: return AssignmentOperation.XorAssign;

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

        internal static string? GetSymbol(this BoundOperation op)
        {
            switch (op)
            {
                case BoundOperation.Addition:
                    return "+";
                case BoundOperation.Subtraction:
                    return "-";
                case BoundOperation.Multiplication:
                    return "*";
                case BoundOperation.Division:
                    return "/";
                case BoundOperation.Modulo:
                    return "%";
                case BoundOperation.BitwiseAnd:
                    return "&";
                case BoundOperation.BitwiseOr:
                    return "|";
                case BoundOperation.BitwiseXor:
                    return "^";
                case BoundOperation.LogicalAnd:
                    return "&&";
                case BoundOperation.LogicalOr:
                    return "||";
                case BoundOperation.LessThan:
                    return "<";
                case BoundOperation.GreaterThan:
                    return ">";
                case BoundOperation.LessThanOrEqual:
                    return "<=";
                case BoundOperation.GreaterThanOrEqual:
                    return ">=";
                case BoundOperation.Equal:
                    return "==";
                case BoundOperation.NotEqual:
                    return "!=";
                case BoundOperation.Identity:
                    return "+";
                case BoundOperation.Negation:
                    return "-";
                case BoundOperation.BitwiseNot:
                    return "~";
                case BoundOperation.LogicalNot:
                    return "!";
                default:
                    return null;
            }
        }

        internal static string? GetSymbol(this AssignmentOperation op)
        {
            switch (op)
            {
                case AssignmentOperation.Assign:
                    return "=";
                case AssignmentOperation.AddAssign:
                    return "+=";
                case AssignmentOperation.SubAssign:
                    return "-=";
                case AssignmentOperation.MulAssign:
                    return "*=";
                case AssignmentOperation.DivAssign:
                    return "/=";
                case AssignmentOperation.ModAssign:
                    return "%=";
                case AssignmentOperation.AndAssign:
                    return "&=";
                case AssignmentOperation.OrAssign:
                    return "|=";
                case AssignmentOperation.XorAssign:
                    return "^=";
                default:
                    return null;
            }
        }

        internal static string GetSymbolOrName(this BoundOperation op) 
        {
            var sym = op.GetSymbol();

            if (sym is not null)
                return sym;

            return op.ToString();
        }

        internal static string GetSymbolOrName(this AssignmentOperation op) 
        {
            var sym = op.GetSymbol();

            if (sym is not null)
                return sym;

            return op.ToString();
        }
    }
}