using System;
using System.Collections.Generic;
using System.Diagnostics;
using Kyloe.Lowering;
using Kyloe.Semantics;
using Kyloe.Symbols;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;


namespace Kyloe.Codegen
{
    internal sealed class MethodGenerator
    {
        private ILProcessor ilProcessor => Method.Body.GetILProcessor();

        private Dictionary<Symbol, VariableDefinition> locals;
        private Dictionary<LoweredLabel, int> lables;
        private List<(int index, LoweredLabel jump)> jumps;

        public MethodGenerator(MethodDefinition method, TypeResolver resolver)
        {
            Method = method;
            Resolver = resolver;

            locals = new Dictionary<Symbol, VariableDefinition>();
            lables = new Dictionary<LoweredLabel, int>();
            jumps = new List<(int index, LoweredLabel jump)>();
        }

        public MethodDefinition Method { get; }
        public TypeResolver Resolver { get; }

        public void GenerateFunctionBody(LoweredFunctionDefinition function)
        {
            foreach (var stmt in function.Body)
                GenerateStatement(stmt);


            // fill in all jump instructions
            foreach (var (jumpInstructionIndex, targetLabel) in jumps)
            {
                var jumpInstruction = Method.Body.Instructions[jumpInstructionIndex];
                var targetInstruction = Method.Body.Instructions[lables[targetLabel]];
                jumpInstruction.Operand = targetInstruction;
            }

            Method.Body.Optimize();
        }

        public void GenerateStatement(LoweredStatement stmt)
        {
            switch (stmt.Kind)
            {
                case LoweredNodeKind.LoweredReturnStatement:
                    GenerateReturnStatement((LoweredReturnStatement)stmt); break;
                case LoweredNodeKind.LoweredExpressionStatement:
                    GenerateExpressionStatement((LoweredExpressionStatement)stmt); break;
                case LoweredNodeKind.LoweredDeclarationStatement:
                    GenerateDeclarationStatement((LoweredDeclarationStatement)stmt); break;
                case LoweredNodeKind.LoweredEmptyStatement:
                    GenerateEmptyStatement((LoweredEmptyStatement)stmt); break;
                case LoweredNodeKind.LoweredGotoStatement:
                    GenerateGotoStatement((LoweredGotoStatement)stmt); break;
                case LoweredNodeKind.LoweredConditionalGotoStatement:
                    GenerateConditionalGotoStatement((LoweredConditionalGotoStatement)stmt); break;
                case LoweredNodeKind.LoweredLabelStatement:
                    GenerateLabelStatement((LoweredLabelStatement)stmt); break;

                default:
                    throw new Exception($"unexpected kind: {stmt.Kind}");
            }
        }

        private void GenerateLabelStatement(LoweredLabelStatement stmt)
        {
            // Add a label to the next instruction
            lables.Add(stmt.Label, ilProcessor.Body.Instructions.Count);
        }

        private void GenerateConditionalGotoStatement(LoweredConditionalGotoStatement stmt)
        {
            GenerateExpression(stmt.Condition);

            // Only add a empty jump instruction here, the destination will be replaced later.
            jumps.Add((ilProcessor.Body.Instructions.Count, stmt.Label));
            ilProcessor.Emit(OpCodes.Brtrue, Instruction.Create(OpCodes.Nop));
        }

        private void GenerateGotoStatement(LoweredGotoStatement stmt)
        {
            jumps.Add((ilProcessor.Body.Instructions.Count, stmt.Label));
            ilProcessor.Emit(OpCodes.Br, Instruction.Create(OpCodes.Nop));
        }

        private void GenerateEmptyStatement(LoweredEmptyStatement stmt)
        {
        }

        private void GenerateDeclarationStatement(LoweredDeclarationStatement stmt)
        {
            // We are in a function body, so declaration should be a local variable.
            Debug.Assert(stmt.Symbol.Kind == SymbolKind.LocalVariableSymbol);

            // The optimizers should have eliminated the initializer.
            Debug.Assert(stmt.Initializer is null);

            var type = Resolver.ResolveType(stmt.Symbol.Type);
            var local = new VariableDefinition(type);

            locals.Add(stmt.Symbol, local);
            Method.Body.Variables.Add(local);
        }

        private void GenerateExpressionStatement(LoweredExpressionStatement stmt)
        {
            var expr = stmt.Expression;

            GenerateExpression(expr);

            if (!expr.Type.Equals(Resolver.TypeSystem.Void))
                ilProcessor.Emit(OpCodes.Pop);
        }

        private void GenerateReturnStatement(LoweredReturnStatement stmt)
        {
            if (stmt.Expression is not null)
                GenerateExpression(stmt.Expression);

            ilProcessor.Emit(OpCodes.Ret);
        }

        private void GenerateExpression(LoweredExpression expr)
        {
            switch (expr.Kind)
            {
                case LoweredNodeKind.LoweredLiteralExpression:
                    GenerateLiteralExpression((LoweredLiteralExpression)expr); break;
                case LoweredNodeKind.LoweredBinaryExpression:
                    GenerateBinaryExpression((LoweredBinaryExpression)expr); break;
                case LoweredNodeKind.LoweredUnaryExpression:
                    GenerateUnaryExpression((LoweredUnaryExpression)expr); break;
                case LoweredNodeKind.LoweredAssignment:
                    GenerateAssignment((LoweredAssignment)expr); break;
                case LoweredNodeKind.LoweredSymbolExpression:
                    GenerateSymbolExpression((LoweredSymbolExpression)expr); break;
                case LoweredNodeKind.LoweredCallExpression:
                    GenerateCallExpression((LoweredCallExpression)expr); break;

                default:
                    throw new Exception($"unexpected kind: {expr.Kind}");
            }
        }

        private void GenerateCallExpression(LoweredCallExpression expr)
        {
            // currently not used, but later, this will load the this pointer on the stack
            GenerateExpression(expr.Expression);

            foreach (var arg in expr.Arguments)
                GenerateExpression(arg);

            var method = Resolver.ResolveCallable(expr.CallableType);

            ilProcessor.Emit(OpCodes.Call, method);
        }

        private void GenerateSymbolExpression(LoweredSymbolExpression expr)
        {
            switch (expr.Symbol.Kind)
            {
                case SymbolKind.LocalVariableSymbol:
                    var local = locals[(LocalVariableSymbol)expr.Symbol];
                    ilProcessor.Emit(OpCodes.Ldloc, local);
                    break;

                case SymbolKind.CallableGroupSymbol:
                    // nothing to do here
                    break;

                case SymbolKind.ParameterSymbol:
                    var paramSymbol = (ParameterSymbol)expr.Symbol;
                    var param = Method.Parameters[paramSymbol.Index];
                    ilProcessor.Emit(OpCodes.Ldarg, param);
                    break;

                case SymbolKind.TypeNameSymbol:
                case SymbolKind.FieldSymbol:
                case SymbolKind.OperationSymbol:
                case SymbolKind.GlobalVariableSymbol:
                    throw new NotImplementedException();
            }
        }

        private void GenerateAssignment(LoweredAssignment expr)
        {
            // The optimizers should have eliminated all compound assignments. 
            Debug.Assert(expr.Operation == AssignmentOperation.Assign);

            if (expr.LeftExpression is LoweredSymbolExpression left)
            {
                switch (left.Symbol.Kind)
                {
                    case SymbolKind.LocalVariableSymbol:
                        var local = locals[left.Symbol];
                        GenerateExpression(expr.RightExpression);
                        ilProcessor.Emit(OpCodes.Stloc, local);
                        break;

                    case SymbolKind.GlobalVariableSymbol:
                    case SymbolKind.FieldSymbol:
                    case SymbolKind.ParameterSymbol:
                        throw new NotImplementedException();

                    default:
                        throw new Exception($"Unexpected symbol kind: {left.Symbol.Kind}");
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void GenerateUnaryExpression(LoweredUnaryExpression expr)
        {
            // TODO: handle non-builtin unary operations

            GenerateExpression(expr.Expression);

            switch (expr.Operation)
            {
                case BoundOperation.Identity:
                    break;
                case BoundOperation.Negation:
                    ilProcessor.Emit(OpCodes.Neg); break;
                case BoundOperation.BitwiseNot:
                    ilProcessor.Emit(OpCodes.Not); break;
                case BoundOperation.LogicalNot:
                    ilProcessor.Emit(OpCodes.Ldc_I4_0);
                    ilProcessor.Emit(OpCodes.Ceq);
                    break;

                default:
                    throw new Exception($"unknown operation {expr.Operation}");
            }
        }

        private void GenerateBinaryExpression(LoweredBinaryExpression expr)
        {
            // TODO: handle non-builtin binary operations

            GenerateExpression(expr.LeftExpression);
            GenerateExpression(expr.RightExpression);

            switch (expr.Operation)
            {
                case BoundOperation.Addition:
                    ilProcessor.Emit(OpCodes.Add); break;
                case BoundOperation.Subtraction:
                    ilProcessor.Emit(OpCodes.Sub); break;
                case BoundOperation.Multiplication:
                    ilProcessor.Emit(OpCodes.Mul); break;
                case BoundOperation.Division:
                    ilProcessor.Emit(OpCodes.Div); break;
                case BoundOperation.Modulo:
                    ilProcessor.Emit(OpCodes.Rem); break;
                case BoundOperation.BitwiseAnd:
                    ilProcessor.Emit(OpCodes.And); break;
                case BoundOperation.BitwiseOr:
                    ilProcessor.Emit(OpCodes.Or); break;
                case BoundOperation.BitwiseXor:
                    ilProcessor.Emit(OpCodes.Xor); break;
                case BoundOperation.LessThan:
                    ilProcessor.Emit(OpCodes.Clt); break;
                case BoundOperation.GreaterThan:
                    ilProcessor.Emit(OpCodes.Cgt); break;
                case BoundOperation.LessThanOrEqual:
                    ilProcessor.Emit(OpCodes.Cgt);
                    ilProcessor.Emit(OpCodes.Ceq);
                    break;
                case BoundOperation.GreaterThanOrEqual:
                    ilProcessor.Emit(OpCodes.Clt);
                    ilProcessor.Emit(OpCodes.Ceq);
                    break;
                case BoundOperation.Equal:
                    ilProcessor.Emit(OpCodes.Ceq); break;
                case BoundOperation.NotEqual:
                    ilProcessor.Emit(OpCodes.Ceq);
                    ilProcessor.Emit(OpCodes.Ldc_I4_0);
                    ilProcessor.Emit(OpCodes.Ceq);
                    break;
                default:
                    throw new Exception($"unknown operation {expr.Operation}");
            }
        }

        private void GenerateLiteralExpression(LoweredLiteralExpression expr)
        {
            if (expr.Type.Equals(Resolver.TypeSystem.I64))
            {
                ilProcessor.Emit(OpCodes.Ldc_I8, (long)expr.Value);
            }
            else if (expr.Type.Equals(Resolver.TypeSystem.Double))
            {
                ilProcessor.Emit(OpCodes.Ldc_R8, (double)expr.Value);
            }
            else if (expr.Type.Equals(Resolver.TypeSystem.String))
            {
                ilProcessor.Emit(OpCodes.Ldstr, (string)expr.Value);
            }
            else if (expr.Type.Equals(Resolver.TypeSystem.Bool))
            {
                bool val = (bool)expr.Value;

                if (val)
                    ilProcessor.Emit(OpCodes.Ldc_I4_1);
                else
                    ilProcessor.Emit(OpCodes.Ldc_I4_0);
            }
            else
            {
                throw new Exception($"unexpected literal type");
            }
        }
    }

}