using System.Linq;
using Kyloe.Syntax;
using Mono.Cecil;

namespace Kyloe.Semantics
{
    public static class SemanticInfo
    {
        public static TypeReference GetTypeFromLiteral(TypeSystem typeSystem, SyntaxTokenType tokenType)
        {
            switch (tokenType)
            {
                case SyntaxTokenType.IntLiteral: return typeSystem.Int64;
                case SyntaxTokenType.FloatLiteral: return typeSystem.Double;
                case SyntaxTokenType.BoolLiteral: return typeSystem.Boolean;
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

        internal static BoundResultType? GetBinaryOperationResult(BoundResultType leftResult, BinaryOperation op, BoundResultType rightResult)
        {
            if (!leftResult.IsTypeValue || !rightResult.IsTypeValue)
                return BoundResultType.ErrorResult;

            var lhs = leftResult.TypeValue;
            var rhs = rightResult.TypeValue;

            if (lhs.IsPrimitive)
            {
                if (lhs != rhs)
                    return null;

                switch (op)
                {
                    case BinaryOperation.Addition:
                    case BinaryOperation.Subtraction:
                    case BinaryOperation.Multiplication:
                    case BinaryOperation.Division:
                    case BinaryOperation.Modulo:
                        switch (lhs.MetadataType)
                        {
                            case MetadataType.SByte:
                            case MetadataType.Int16:
                            case MetadataType.Int32:
                            case MetadataType.Int64:
                            case MetadataType.Byte:
                            case MetadataType.UInt16:
                            case MetadataType.UInt32:
                            case MetadataType.UInt64:
                            case MetadataType.Single:
                            case MetadataType.Double:
                                return leftResult;
                            default:
                                return null;
                        }

                    case BinaryOperation.BitwiseAnd:
                    case BinaryOperation.BitwiseOr:
                    case BinaryOperation.BitwiseXor:
                        switch (lhs.MetadataType)
                        {
                            case MetadataType.SByte:
                            case MetadataType.Int16:
                            case MetadataType.Int32:
                            case MetadataType.Int64:
                            case MetadataType.Byte:
                            case MetadataType.UInt16:
                            case MetadataType.UInt32:
                            case MetadataType.UInt64:
                                return leftResult;
                            default:
                                return null;
                        }

                    case BinaryOperation.LogicalAnd:
                    case BinaryOperation.LogicalOr:
                        switch (lhs.MetadataType)
                        {
                            case MetadataType.Boolean:
                                return leftResult;
                            default:
                                return null;
                        }

                    case BinaryOperation.LessThan:
                    case BinaryOperation.GreaterThan:
                    case BinaryOperation.LessThanOrEqual:
                    case BinaryOperation.GreaterThanOrEqual:
                    case BinaryOperation.Equal:
                    case BinaryOperation.NotEqual:
                        return new BoundResultType(lhs.Module.TypeSystem.Boolean, isValue: true);

                    default:
                        throw new System.Exception($"Unexpected operation: {op}");
                }
            }
            else
            {
                var methodName = GetBinaryOperationMethodName(op);

                var methods = lhs.Resolve().Methods.Where(m =>
                                            m.IsSpecialName &&
                                            m.IsStatic &&
                                            m.Name == methodName &&
                                            m.Parameters.Count == 2 &&
                                            m.Parameters[0].ParameterType == lhs &&
                                            m.Parameters[1].ParameterType == rhs);

                var method = methods.FirstOrDefault();

                if (method is null)
                    return null;
                else
                    return new BoundResultType(method.ReturnType, isValue: true);
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

        internal static BoundResultType? GetUnaryOperationResult(UnaryOperation op, BoundResultType result)
        {
            if (!result.IsTypeValue)
                return null;

            var type = result.TypeValue;

            if (type.IsPrimitive)
            {
                switch (op)
                {
                    case UnaryOperation.Negation:
                        switch (type.MetadataType)
                        {
                            case MetadataType.SByte:
                            case MetadataType.Int16:
                            case MetadataType.Int32:
                            case MetadataType.Int64:
                            case MetadataType.Single:
                            case MetadataType.Double:
                                return result;
                            default:
                                return null;
                        }


                    case UnaryOperation.BitwiseNot:
                        switch (type.MetadataType)
                        {
                            case MetadataType.SByte:
                            case MetadataType.Int16:
                            case MetadataType.Int32:
                            case MetadataType.Int64:
                            case MetadataType.Byte:
                            case MetadataType.UInt16:
                            case MetadataType.UInt32:
                            case MetadataType.UInt64:
                                return result;
                            default:
                                return null;
                        }

                    case UnaryOperation.LogicalNot:
                        switch (type.MetadataType)
                        {
                            case MetadataType.Boolean:
                                return result;
                            default:
                                return null;
                        }

                    default:
                        return null;
                }
            }
            else
            {
                var methodName = GetUnaryOperationMethodName(op);

                var methods = type.Resolve().Methods.Where(m =>
                                            m.IsSpecialName &&
                                            m.IsStatic &&
                                            m.Name == methodName &&
                                            m.Parameters.Count == 1 &&
                                            m.Parameters[0].ParameterType == type);

                var method = methods.FirstOrDefault();

                if (method is null)
                    return null;
                else
                    return new BoundResultType(method.ReturnType, isValue: true);
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