using System;
using System.IO;

using Kyloe.Semantics;

namespace Kyloe.Lowering
{
    internal sealed class LoweredTreeWriter
    {
        private int INDENT = 4;

        private readonly TextWriter writer;

        private int indent = 0;

        public LoweredTreeWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        public void WriteNode(LoweredNode node)
        {
            switch (node.Kind)
            {
                case LoweredNodeKind.LoweredCompilationUnit:
                    WriteCompilationUnit((LoweredCompilationUnit)node); break;
                case LoweredNodeKind.LoweredFunctionDefinition:
                    WriteFunctionDefinition((LoweredFunctionDefinition)node); break;
                case LoweredNodeKind.LoweredArguments:
                    WriteArguments((LoweredArguments)node); break;
                case LoweredNodeKind.LoweredLiteralExpression:
                    WriteLiteralExpression((LoweredLiteralExpression)node); break;
                case LoweredNodeKind.LoweredBinaryExpression:
                    WriteBinaryExpression((LoweredBinaryExpression)node); break;
                case LoweredNodeKind.LoweredUnaryExpression:
                    WriteUnaryExpression((LoweredUnaryExpression)node); break;
                case LoweredNodeKind.LoweredAssignment:
                    WriteAssignment((LoweredAssignment)node); break;
                case LoweredNodeKind.LoweredVariableAccessExpression:
                    WriteVariableAccessExpression((LoweredVariableAccessExpression)node); break;
                case LoweredNodeKind.LoweredFunctionAccessExpression:
                    WriteFunctionAccessExpression((LoweredFunctionAccessExpression)node); break;
                case LoweredNodeKind.LoweredCallExpression:
                    WriteCallExpression((LoweredCallExpression)node); break;
                case LoweredNodeKind.LoweredBlockStatement:
                    WriteBlockStatement((LoweredBlockStatement)node); break;
                case LoweredNodeKind.LoweredContinueStatement:
                    WriteContinueStatement((LoweredContinueStatement)node); break;
                case LoweredNodeKind.LoweredBreakStatement:
                    WriteBreakStatement((LoweredBreakStatement)node); break;
                case LoweredNodeKind.LoweredReturnStatement:
                    WriteReturnStatement((LoweredReturnStatement)node); break;
                case LoweredNodeKind.LoweredWhileStatement:
                    WriteWhileStatement((LoweredWhileStatement)node); break;
                case LoweredNodeKind.LoweredExpressionStatement:
                    WriteExpressionStatement((LoweredExpressionStatement)node); break;
                case LoweredNodeKind.LoweredDeclarationStatement:
                    WriteDeclarationStatement((LoweredDeclarationStatement)node); break;
                case LoweredNodeKind.LoweredEmptyStatement:
                    WriteEmptyStatement((LoweredEmptyStatement)node); break;
                case LoweredNodeKind.LoweredIfStatement:
                    WriteIfStatement((LoweredIfStatement)node); break;
                case LoweredNodeKind.LoweredGotoStatement:
                    WriteGotoStatement((LoweredGotoStatement)node); break;
                case LoweredNodeKind.LoweredConditionalGotoStatement:
                    WriteConditionalGotoStatement((LoweredConditionalGotoStatement)node); break;
                case LoweredNodeKind.LoweredLabelStatement:
                    WriteLabelStatement((LoweredLabelStatement)node); break;
                default:
                    throw new System.Exception($"unexpected kind: {node.Kind}");
            }
        }

        private void WriteCompilationUnit(LoweredCompilationUnit node)
        {
            WriteNode(node.GlobalStatement);
            WriteLine();

            foreach (var func in node.LoweredFunctions)
            {
                WriteLine();
                WriteNode(func);
            }

            WriteLine();
        }

        private void WriteFunctionDefinition(LoweredFunctionDefinition node)
        {
            WriteLine();
            writer.Write(node.FunctionType.FullName());
            writer.Write(' ');
            WriteNode(node.Body);
        }

        private void WriteArguments(LoweredArguments node)
        {
            foreach (var (i, expr) in node.EnumerateIndex())
            {
                WriteNode(expr);
                if (i != node.Arguments.Length - 1)
                    writer.Write(", ");
            }
        }

        private void WriteLiteralExpression(LoweredLiteralExpression node)
        {
            writer.Write(node.Value);
        }

        private void WriteBinaryExpression(LoweredBinaryExpression node)
        {
            writer.Write('(');
            WriteNode(node.LeftExpression);
            writer.Write($" {node.Operation.GetSymbolOrName()} ");
            WriteNode(node.RightExpression);
            writer.Write(')');
        }

        private void WriteUnaryExpression(LoweredUnaryExpression node)
        {
            writer.Write(node.Operation.GetSymbolOrName());
            WriteNode(node.Expression);
        }

        private void WriteAssignment(LoweredAssignment node)
        {
            writer.Write('(');
            WriteNode(node.LeftExpression);
            writer.Write($" {node.Operation.GetSymbolOrName()} ");
            WriteNode(node.RightExpression);
            writer.Write(')');
        }

        private void WriteVariableAccessExpression(LoweredVariableAccessExpression node)
        {
            writer.Write(node.VariableSymbol.Name);
        }

        private void WriteFunctionAccessExpression(LoweredFunctionAccessExpression node)
        {
            writer.Write(node.FunctionGroup.Name);
        }

        private void WriteCallExpression(LoweredCallExpression node)
        {
            WriteNode(node.Expression);
            writer.Write('(');
            WriteArguments(node.Arguments);
            writer.Write(')');
        }

        private void WriteBlockStatement(LoweredBlockStatement node)
        {
            writer.Write('{');
            Indent();

            foreach (var stmt in node.Statements)
                WriteNode(stmt);

            Unindent();
            WriteLine();
            writer.Write('}');
        }

        private void WriteContinueStatement(LoweredContinueStatement node)
        {
            WriteLine();
            writer.Write("continue;");
        }

        private void WriteBreakStatement(LoweredBreakStatement node)
        {
            WriteLine();
            writer.Write("break;");
        }

        private void WriteReturnStatement(LoweredReturnStatement node)
        {
            WriteLine();
            writer.Write("return");

            if (node.Expression is not null)
            {
                writer.Write(' ');
                WriteNode(node.Expression);
            }

            writer.Write(';');
        }

        private void WriteWhileStatement(LoweredWhileStatement node)
        {
            WriteLine();
            writer.Write("while ");
            WriteNode(node.Condition);
            writer.Write(" ");
            WriteNode(node.Body);
        }

        private void WriteExpressionStatement(LoweredExpressionStatement node)
        {
            WriteLine();
            WriteNode(node.Expression);
            writer.Write(';');
        }

        private void WriteDeclarationStatement(LoweredDeclarationStatement node)
        {
            WriteLine();
            writer.Write($"var {node.Symbol.Name}: {node.Symbol.Type.FullName()}");

            if (node.Initializer is not null)
            {
                writer.Write(" = ");
                WriteNode(node.Initializer);
            }

            writer.Write(';');
        }

        private void WriteEmptyStatement(LoweredEmptyStatement node)
        {
            WriteLine();
            writer.Write(';');
        }

        private void WriteIfStatement(LoweredIfStatement node)
        {
            WriteLine();
            writer.Write("if ");
            WriteNode(node.Condition);
            writer.Write(" ");
            WriteNode(node.Body);

            if (node.ElseStatment is not LoweredEmptyStatement)
            {
                WriteLine();
                writer.Write("else ");
                WriteNode(node.ElseStatment);
            }
        }

        private void WriteGotoStatement(LoweredGotoStatement node)
        {
            WriteLine();
            writer.Write($"goto {node.Label};");
        }

        private void WriteConditionalGotoStatement(LoweredConditionalGotoStatement node)
        {
            WriteLine();
            writer.Write($"goto {node.Label} if ");
            WriteNode(node.Condition);
            writer.Write(';');
        }

        private void WriteLabelStatement(LoweredLabelStatement node)
        {
            Unindent();
            WriteLine();
            writer.Write($"{node.Label}:");
            Indent();
        }

        private void WriteLine()
        {
            writer.WriteLine();
            writer.Write(new string(' ', indent));
        }

        private void Indent()
        {
            indent += INDENT;
        }

        private void Unindent()
        {
            indent -= INDENT;
        }
    }
}