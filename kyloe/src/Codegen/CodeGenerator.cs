using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Kyloe.Lowering;
using Kyloe.Symbols;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Kyloe.Codegen
{
    internal sealed class CodeGenerator
    {
        private readonly Symbols.TypeSystem kyloeTypeSystem;
        private readonly Mono.Cecil.TypeSystem cecilTypeSystem;

        private readonly AssemblyDefinition assembly;
        private readonly TypeDefinition mainClass;

        private readonly TypeResolver resolver;


        public CodeGenerator(string programName, Symbols.TypeSystem kyloeTypeSystem)
        {
            this.kyloeTypeSystem = kyloeTypeSystem;

            var assemblyName = new AssemblyNameDefinition(programName, new Version(0, 1));
            this.assembly = AssemblyDefinition.CreateAssembly(assemblyName, programName, ModuleKind.Dll);
            this.cecilTypeSystem = assembly.MainModule.TypeSystem;
            this.mainClass = new TypeDefinition("", programName, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Abstract | TypeAttributes.Sealed, cecilTypeSystem.Object);
            this.assembly.MainModule.Types.Add(mainClass);
            this.resolver = new TypeResolver(kyloeTypeSystem, mainClass);
        }

        public void WriteTo(string path) 
        {
            assembly.Write(path);
        }

        public void WriteTo(Stream stream) 
        {
            assembly.Write(stream);
        }

        public void GenerateCompiationUnit(LoweredCompilationUnit unit)
        {
            foreach (var (i, func) in unit.LoweredFunctions.EnumerateIndex())
            {
                var method = resolver.ResolveCallable(func.Type).Resolve();
                
                mainClass.Methods.Add(method);

                if (i == unit.MainFunctionIndex)
                    mainClass.Module.EntryPoint = method;

                GenerateFunctionBody(func, method);
            }
        }

        private void GenerateFunctionBody(LoweredFunctionDefinition func, MethodDefinition method)
        {
            var ilProcessor = method.Body.GetILProcessor();

            foreach (var stmt in func.Body)
                GenerateStatement(stmt, ilProcessor);


        }

        private void GenerateStatement(LoweredStatement stmt, ILProcessor ilProcessor)
        {
            switch (stmt.Kind)
            {
                case LoweredNodeKind.LoweredReturnStatement:
                    GenerateReturnStatement((LoweredReturnStatement)stmt, ilProcessor); break;
                case LoweredNodeKind.LoweredExpressionStatement:
                    GenerateExpressionStatement((LoweredExpressionStatement)stmt, ilProcessor); break;
                case LoweredNodeKind.LoweredDeclarationStatement:
                    GenerateDeclarationStatement((LoweredDeclarationStatement)stmt, ilProcessor); break;
                case LoweredNodeKind.LoweredEmptyStatement:
                    GenerateEmptyStatement((LoweredEmptyStatement)stmt, ilProcessor); break;
                case LoweredNodeKind.LoweredGotoStatement:
                    GenerateGotoStatement((LoweredGotoStatement)stmt, ilProcessor); break;
                case LoweredNodeKind.LoweredConditionalGotoStatement:
                    GenerateConditionalGotoStatement((LoweredConditionalGotoStatement)stmt, ilProcessor); break;
                case LoweredNodeKind.LoweredLabelStatement:
                    GenerateLabelStatement((LoweredLabelStatement)stmt, ilProcessor); break;

                default:
                    throw new Exception($"unexpected kind: {stmt.Kind}");
            }
        }

        private void GenerateReturnStatement(LoweredReturnStatement stmt, ILProcessor ilProcessor)
        {
            if (stmt.Expression is not null)
                GenerateExpression(stmt.Expression, ilProcessor);

            ilProcessor.Emit(OpCodes.Ret);
        }

        private void GenerateExpressionStatement(LoweredExpressionStatement stmt, ILProcessor ilProcessor)
        {
            var expr = stmt.Expression;

            GenerateExpression(expr, ilProcessor);

            if (!expr.Type.Equals(kyloeTypeSystem.Void))
                ilProcessor.Emit(OpCodes.Pop);
        }

        private void GenerateDeclarationStatement(LoweredDeclarationStatement stmt, ILProcessor ilProcessor)
        {
            throw new NotImplementedException();
        }

        private void GenerateEmptyStatement(LoweredEmptyStatement stmt, ILProcessor ilProcessor)
        {
            throw new NotImplementedException();
        }

        private void GenerateGotoStatement(LoweredGotoStatement stmt, ILProcessor ilProcessor)
        {
            throw new NotImplementedException();
        }

        private void GenerateConditionalGotoStatement(LoweredConditionalGotoStatement stmt, ILProcessor ilProcessor)
        {
            throw new NotImplementedException();
        }

        private void GenerateLabelStatement(LoweredLabelStatement stmt, ILProcessor ilProcessor)
        {
            throw new NotImplementedException();
        }

        private void GenerateExpression(LoweredExpression expr, ILProcessor ilProcessor)
        {
            switch (expr.Kind)
            {
                case LoweredNodeKind.LoweredLiteralExpression:
                    GenerateLiteralExpression((LoweredLiteralExpression)expr, ilProcessor); break;
                case LoweredNodeKind.LoweredBinaryExpression:
                    GenerateBinaryExpression((LoweredBinaryExpression)expr, ilProcessor); break;
                case LoweredNodeKind.LoweredUnaryExpression:
                    GenerateUnaryExpression((LoweredUnaryExpression)expr, ilProcessor); break;
                case LoweredNodeKind.LoweredAssignment:
                    GenerateAssignment((LoweredAssignment)expr, ilProcessor); break;
                case LoweredNodeKind.LoweredSymbolExpression:
                    GenerateSymbolExpression((LoweredSymbolExpression)expr, ilProcessor); break;
                case LoweredNodeKind.LoweredCallExpression:
                    GenerateCallExpression((LoweredCallExpression)expr, ilProcessor); break;

                default:
                    throw new Exception($"unexpected kind: {expr.Kind}");
            }
        }

        private void GenerateLiteralExpression(LoweredLiteralExpression expr, ILProcessor ilProcessor)
        {
            if (expr.Type.Equals(kyloeTypeSystem.I64))
            {
                ilProcessor.Emit(OpCodes.Ldc_I8, (long)expr.Value);
            }
            else if (expr.Type.Equals(kyloeTypeSystem.Double))
            {
                ilProcessor.Emit(OpCodes.Ldc_R8, (double)expr.Value);
            }
            else if (expr.Type.Equals(kyloeTypeSystem.String))
            {
                ilProcessor.Emit(OpCodes.Ldstr, (string)expr.Value);
            }
            else
            {
                throw new Exception($"unexpected literal type");
            }
        }

        private void GenerateBinaryExpression(LoweredBinaryExpression expr, ILProcessor ilProcessor)
        {
            throw new NotImplementedException();
        }

        private void GenerateUnaryExpression(LoweredUnaryExpression expr, ILProcessor ilProcessor)
        {
            throw new NotImplementedException();
        }

        private void GenerateAssignment(LoweredAssignment expr, ILProcessor ilProcessor)
        {
            throw new NotImplementedException();
        }

        private void GenerateSymbolExpression(LoweredSymbolExpression expr, ILProcessor ilProcessor)
        {
            throw new NotImplementedException();
        }

        private void GenerateCallExpression(LoweredCallExpression expr, ILProcessor ilProcessor)
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class TypeResolver
    {
        private readonly Symbols.TypeSystem kyloeTypeSystem;
        private readonly Mono.Cecil.TypeSystem cecilTypeSystem;
        private readonly TypeDefinition mainClass;

        private readonly Dictionary<TypeInfo, TypeReference> valueTypeCache;
        private readonly Dictionary<CallableType, MethodReference> callableCache;

        public TypeResolver(Symbols.TypeSystem kyloeTypeSystem, TypeDefinition mainClass)
        {
            this.kyloeTypeSystem = kyloeTypeSystem;
            this.mainClass = mainClass;
            this.cecilTypeSystem = mainClass.Module.TypeSystem;
            this.valueTypeCache = new Dictionary<TypeInfo, TypeReference>();
            this.callableCache = new Dictionary<CallableType, MethodReference>();
        }

        public TypeReference ResolveType(Symbols.TypeInfo type)
        {
            if (valueTypeCache.TryGetValue(type, out var res))
                return res;

            var resolved = ResolveTypeImpl(type);

            valueTypeCache.Add(type, resolved);

            return resolved;
        }

        private TypeReference ResolveTypeImpl(TypeInfo type)
        {
            switch (type.Kind)
            {
                case TypeKind.BuiltinType:
                    return ResolveBuiltinType((Symbols.BuiltinType)type);
                case TypeKind.ArrayType:
                    return ResolveArrayType((Symbols.ArrayType)type);
                default:
                    throw new Exception($"unknown kind: {type.Kind}");
            }
        }

        private TypeReference ResolveArrayType(Symbols.ArrayType type)
        {
            throw new NotImplementedException();
        }

        private TypeReference ResolveBuiltinType(BuiltinType type)
        {
            switch (type.Name)
            {
                case "void": return cecilTypeSystem.Void;
                case "char": return cecilTypeSystem.Char;
                case "i8": return cecilTypeSystem.SByte;
                case "i16": return cecilTypeSystem.Int16;
                case "i32": return cecilTypeSystem.Int32;
                case "i64": return cecilTypeSystem.Int64;
                case "u8": return cecilTypeSystem.Byte;
                case "u16": return cecilTypeSystem.UInt16;
                case "u32": return cecilTypeSystem.UInt32;
                case "u64": return cecilTypeSystem.UInt64;
                case "float": return cecilTypeSystem.Single;
                case "double": return cecilTypeSystem.Double;
                case "bool": return cecilTypeSystem.Boolean;
                case "string": return cecilTypeSystem.String;

                default:
                    throw new Exception($"unexpected builtin type: '{type.Name}'");
            }
        }

        public MethodReference ResolveCallable(Symbols.CallableType callable)
        {
            if (callableCache.TryGetValue(callable, out var res))
                return res;

            var resolved = ResolveCallableImpl(callable);

            callableCache.Add(callable, resolved);

            return resolved;
        }

        private MethodReference ResolveCallableImpl(CallableType callable)
        {
            switch (callable.Kind)
            {
                case TypeKind.BuiltinFunctionType:
                    return ResolveBuiltinFunctionType((Symbols.BuiltinFunctionType)callable);
                case TypeKind.FunctionType:
                    return ResolveFunctionType((Symbols.FunctionType)callable);
                case TypeKind.MethodType:
                    return ResolveMethodType((Symbols.MethodType)callable);
                default:
                    throw new Exception($"unknown kind: {callable.Kind}");
            }
        }

        private MethodReference ResolveMethodType(MethodType callable)
        {
            throw new NotImplementedException();
        }

        private MethodReference ResolveFunctionType(FunctionType callable)
        {
            var returnType = ResolveType(callable.ReturnType);
            var method = new MethodDefinition(callable.Name, MethodAttributes.Private | MethodAttributes.Static, returnType);
            return method;
        }

        private MethodReference ResolveBuiltinFunctionType(BuiltinFunctionType callable)
        {
            throw new NotImplementedException();
        }
    }
}