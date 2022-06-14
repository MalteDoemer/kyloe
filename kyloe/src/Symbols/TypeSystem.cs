
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Kyloe.Semantics;
using Mono.Cecil;

namespace Kyloe.Symbols
{
    internal partial class TypeSystem
    {
        public SymbolScope GlobalScope { get; }

        public ErrorType Error { get; }
        public BuiltinType Void { get; }
        public BuiltinType Object { get; }
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

        public AssemblyDefinition KyloeBuiltinsAssembly { get; }

        public static TypeSystem Create(AssemblyDefinition kyloeBuiltinsAssembly)
        {
            return new TypeSystem(kyloeBuiltinsAssembly);
        }

        private TypeSystem(AssemblyDefinition kyloeBuiltinsAssembly)
        {
            KyloeBuiltinsAssembly = kyloeBuiltinsAssembly;

            GlobalScope = new SymbolScope();

            Error = new ErrorType();
            Object = new BuiltinType("object");
            Void = new BuiltinType("void");
            Char = new BuiltinType("char");
            I8 = new BuiltinType("i8");
            I16 = new BuiltinType("i16");
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

            GlobalScope.DeclareSymbol(new ErrorSymbol(this));

            foreach (var builtin in Enum.GetValues<BuiltinTypeKind>())
                GlobalScope.DeclareSymbol(new TypeNameSymbol(GetBuiltinType(builtin)));

            InitializeBuiltinFunctions();

            InitializeBuiltinOperations();
        }

        private void InitializeBuiltinOperations()
        {
            foreach (var binary in BuiltinOperationInfo.BinaryOperations)
            {
                var left = GetBuiltinType(binary.lhs);
                var right = GetBuiltinType(binary.rhs);
                var ret = GetBuiltinType(binary.ret);

                foreach (var op in binary.ops)
                {
                    var name = SemanticInfo.GetFunctionNameFromOperation(op);
                    var group = left.Scope.LookupSymbol(name) as CallableGroupSymbol;

                    if (group is null)
                    {
                        group = new CallableGroupSymbol(new CallableGroupType(name, left));
                        Debug.Assert(left.Scope.DeclareSymbol(group));
                    }

                    AddBuiltinBinaryOperation(group.Group, op, ret, left, right);
                }
            }

            foreach (var unary in BuiltinOperationInfo.UnaryOperations)
            {
                var arg = GetBuiltinType(unary.arg);
                var ret = GetBuiltinType(unary.ret);

                foreach (var op in unary.ops)
                {
                    var name = SemanticInfo.GetFunctionNameFromOperation(op);
                    var group = arg.Scope.LookupSymbol(name) as CallableGroupSymbol;

                    if (group is null)
                    {
                        group = new CallableGroupSymbol(new CallableGroupType(name, arg));
                        Debug.Assert(arg.Scope.DeclareSymbol(group));
                    }

                    AddBuiltinUnaryOperation(group.Group, op, ret, arg);
                }
            }

        }

        private void InitializeBuiltinFunctions()
        {
            var classes = KyloeBuiltinsAssembly.MainModule.Types.Where(type => type.IsPublic);

            var methods = classes.SelectMany(cls => cls.Methods).Where(method => method.IsPublic && method.IsStatic);


            foreach (var method in methods) 
            {
                var name = method.Name;
                 

            }


            // foreach (var (name, ret, parameters) in BuiltinFunctionInfo.BuiltinFunctions)
            // {
            //     var group = GlobalScope.LookupSymbol(name) as CallableGroupSymbol;

            //     if (group is null)
            //     {
            //         group = new CallableGroupSymbol(new CallableGroupType(name, null));
            //         Debug.Assert(GlobalScope.DeclareSymbol(group));
            //     }

            //     var builtinFunction = CreateBuiltinFunction(name, group.Group, ret, parameters);

            //     group.Group.Callables.Add(builtinFunction);
            // }
        }

        private BuiltinType GetBuiltinType(BuiltinTypeKind type)
        {
            switch (type)
            {
                case BuiltinTypeKind.Object: return Object;
                case BuiltinTypeKind.Void: return Void;
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

        // private BuiltinFunctionType CreateBuiltinFunction(string name, CallableGroupType group, BuiltinTypeKind ret, ImmutableArray<(string name, BuiltinTypeKind type)> parameters)
        // {
        //     var func = new BuiltinFunctionType(group, GetBuiltinType(ret));

        //     foreach (var (i, param) in parameters.EnumerateIndex())
        //         func.Parameters.Add(new ParameterSymbol(param.name, i, GetBuiltinType(param.type)));

        //     return func;
        // }

        private static void AddBuiltinBinaryOperation(CallableGroupType group, BoundOperation op, TypeInfo ret, TypeInfo left, TypeInfo right)
        {
            var method = new MethodType(group, ret, isStatic: true, isOperator: true);
            method.Parameters.Add(new ParameterSymbol("", 0, left));
            method.Parameters.Add(new ParameterSymbol("", 1, right));
            group.Callables.Add(method);
        }

        private static void AddBuiltinUnaryOperation(CallableGroupType group, BoundOperation op, TypeInfo ret, TypeInfo arg)
        {
            var method = new MethodType(group, ret, isStatic: true, isOperator: true);
            method.Parameters.Add(new ParameterSymbol("", 0, arg));
            group.Callables.Add(method);
        }
    }
}