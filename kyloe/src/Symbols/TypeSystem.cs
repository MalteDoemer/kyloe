
using System;
using System.Collections.Generic;
using System.Linq;
using Kyloe.Semantics;

namespace Kyloe.Symbols
{
    internal partial class TypeSystem
    {
        public SymbolScope GlobalScope { get; }

        public ErrorType Error { get; }
        public BuiltinType Void { get; }
        public BuiltinType Char { get; }
        public BuiltinType I8 { get; }
        public BuiltinType I16 { get; }
        public BuiltinType I32 { get; }
        public BuiltinType I64 { get; }
        public BuiltinType U8 { get; }
        public BuiltinType U16 { get; }
        public BuiltinType U32 { get; }
        public BuiltinType U64 { get; }
        public BuiltinType Float { get; }
        public BuiltinType Double { get; }
        public BuiltinType Bool { get; }
        public BuiltinType String { get; }

        public static TypeSystem Create()
        {
            return new TypeSystem();
        }

        private TypeSystem()
        {
            GlobalScope = new SymbolScope();

            Error = new ErrorType();
            Void = new BuiltinType("void");
            Char = new BuiltinType("char");
            I8 = new BuiltinType("i8");
            I16 = new BuiltinType("i8");
            I32 = new BuiltinType("i32");
            I64 = new BuiltinType("i64");
            U8 = new BuiltinType("u8");
            U16 = new BuiltinType("u16");
            U32 = new BuiltinType("u32");
            U64 = new BuiltinType("u64");
            Float = new BuiltinType("float");
            Double = new BuiltinType("double");
            Bool = new BuiltinType("bool");
            String = new BuiltinType("string");


            var testFunctionGroup = new FunctionGroupType("test", I64);
            var testFunction = new FunctionType("test", I64, false, Void);
            testFunction.ParameterTypes.Add(Double);
            testFunctionGroup.Functions.Add(testFunction);
            I64.Scope.DeclareSymbol(new FunctionGroupSymbol(testFunctionGroup));


            foreach (var builtin in Enum.GetValues<BuiltinTypeKind>())
                GlobalScope.DeclareSymbol(new TypeNameSymbol(GetBuiltinType(builtin)));

            foreach (var binary in BuiltinOperatorInfo.BinaryOperations)
            {
                var left = GetBuiltinType(binary.lhs);
                var right = GetBuiltinType(binary.rhs);
                var ret = GetBuiltinType(binary.ret);

                foreach (var op in binary.ops)
                    left.Scope.DeclareSymbol(CreateBuiltinBinaryOperation(op, ret, left, right));
            }

            foreach (var unary in BuiltinOperatorInfo.UnaryOperations)
            {
                var arg = GetBuiltinType(unary.arg);
                var ret = GetBuiltinType(unary.ret);

                foreach (var op in unary.ops)
                    arg.Scope.DeclareSymbol(CreateBuiltinUnaryOperation(op, ret, arg));
            }

        }

        private BuiltinType GetBuiltinType(BuiltinTypeKind type)
        {
            switch (type)
            {
                case BuiltinTypeKind.Char: return Char;
                case BuiltinTypeKind.I8: return I8;
                case BuiltinTypeKind.I16: return I16;
                case BuiltinTypeKind.I32: return I32;
                case BuiltinTypeKind.I64: return I64;
                case BuiltinTypeKind.U8: return U8;
                case BuiltinTypeKind.U16: return U16;
                case BuiltinTypeKind.U32: return U32;
                case BuiltinTypeKind.U64: return U64;
                case BuiltinTypeKind.Float: return Float;
                case BuiltinTypeKind.Double: return Double;
                case BuiltinTypeKind.Bool: return Bool;
                case BuiltinTypeKind.String: return String;
                default: throw new Exception($"unexpected builtin type: {type}");
            }
        }

        private static OperationSymbol CreateBuiltinBinaryOperation(BoundOperation op, TypeSpecifier ret, TypeSpecifier left, TypeSpecifier right)
        {
            var name = SemanticInfo.GetFunctionNameFromOperation(op);
            var func = new FunctionType(name, left, true, ret);
            func.ParameterTypes.Add(left);
            func.ParameterTypes.Add(right);

            var group = new FunctionGroupType(name, left);
            group.Functions.Add(func);

            return new OperationSymbol(op, group);
        }

        private static OperationSymbol CreateBuiltinUnaryOperation(BoundOperation op, TypeSpecifier ret, TypeSpecifier arg)
        {
            var name = SemanticInfo.GetFunctionNameFromOperation(op);
            var func = new FunctionType(name, arg, true, ret);
            func.ParameterTypes.Add(arg);

            var group = new FunctionGroupType(name, arg);
            group.Functions.Add(func);

            return new OperationSymbol(op, group);
        }
    }
}