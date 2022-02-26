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
                default:
                    return null;
            }
        }

        public static TypeReference? GetBinaryOperationResult(TypeReference lhs, BinaryOperation op, TypeReference rhs)
        {
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
                                return lhs;
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
                                return lhs;
                            default:
                                return null;
                        }

                    case BinaryOperation.LogicalAnd:
                    case BinaryOperation.LogicalOr:
                        switch (lhs.MetadataType)
                        {
                            case MetadataType.Boolean:
                                return lhs;
                            default:
                                return null;
                        }

                    case BinaryOperation.LessThan:
                    case BinaryOperation.GreaterThan:
                    case BinaryOperation.LessThanOrEqual:
                    case BinaryOperation.GreaterThanOrEqual:
                    case BinaryOperation.Equal:
                    case BinaryOperation.NotEqual:
                        return lhs.Module.TypeSystem.Boolean;

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
                    return method.ReturnType;
            }
        }


        public static TypeReference? GetUnaryOperationResult(UnaryOperation op, TypeReference type)
        {
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
                                return type;
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
                                return type;
                            default:
                                return null;
                        }

                    case UnaryOperation.LogicalNot:
                        switch (type.MetadataType)
                        {
                            case MetadataType.Boolean:
                                return type;
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
                    return method.ReturnType;
            }
        }
    }
}