
using System.Linq;
using Kyloe.Lowering;

namespace Kyloe.Tests.Lowering
{
    public class VerifyNode
    {
        public VerifyNode(LoweredNodeKind kind, params VerifyNode[] children)
        {
            Kind = kind;
            Children = children;
        }

        public LoweredNodeKind Kind { get; }
        public VerifyNode[] Children { get; }


        public static VerifyNode CompilationUnit(VerifyNode globalStatment, params VerifyNode[] functions)
        {
            var children = Enumerable.Repeat(globalStatment, 1).Concat(functions);
            return new VerifyNode(LoweredNodeKind.LoweredCompilationUnit, children.ToArray());
        }

        public static VerifyNode FunctionDefinition(params VerifyNode[] statements)
        {
            return new VerifyNode(LoweredNodeKind.LoweredFunctionDefinition, BlockStatement(statements));
        }

        public static VerifyNode Arguments(params VerifyNode[] args)
        {
            return new VerifyNode(LoweredNodeKind.LoweredArguments, args);
        }

        public static VerifyNode LiteralExpression()
        {
            return new VerifyNode(LoweredNodeKind.LoweredLiteralExpression);
        }

        public static VerifyNode BinaryExpression(VerifyNode left, VerifyNode right)
        {
            return new VerifyNode(LoweredNodeKind.LoweredBinaryExpression, left, right);
        }

        public static VerifyNode UnaryExpression(VerifyNode expr)
        {
            return new VerifyNode(LoweredNodeKind.LoweredUnaryExpression, expr);
        }

        public static VerifyNode Assignment(VerifyNode left, VerifyNode right)
        {
            return new VerifyNode(LoweredNodeKind.LoweredAssignment, left, right);
        }

        public static VerifyNode SymbolAccessExpression()
        {
            return new VerifyNode(LoweredNodeKind.LoweredSymbolExpression);
        }

        public static VerifyNode BlockStatement(params VerifyNode[] statements)
        {
            return new VerifyNode(LoweredNodeKind.LoweredBlockStatement, statements);
        }

        public static VerifyNode ReturnStatement(VerifyNode? expr)
        {
            if (expr is not null)
                return new VerifyNode(LoweredNodeKind.LoweredReturnStatement, expr);

            return new VerifyNode(LoweredNodeKind.LoweredReturnStatement);
        }

        public static VerifyNode ExpressionStatement(VerifyNode expr)
        {
            return new VerifyNode(LoweredNodeKind.LoweredExpressionStatement, expr);
        }

        public static VerifyNode DeclarationStatement(VerifyNode? initializer)
        {
            if (initializer is not null)
                return new VerifyNode(LoweredNodeKind.LoweredDeclarationStatement, initializer);

            return new VerifyNode(LoweredNodeKind.LoweredDeclarationStatement);
        }

        public static VerifyNode EmptyStatement()
        {
            return new VerifyNode(LoweredNodeKind.LoweredEmptyStatement);
        }

        public static VerifyNode GotoStatement()
        {
            return new VerifyNode(LoweredNodeKind.LoweredGotoStatement);
        }

        public static VerifyNode ConditionalGotoStatement(VerifyNode condition)
        {
            return new VerifyNode(LoweredNodeKind.LoweredConditionalGotoStatement, condition);
        }

        public static VerifyNode LabelStatement()
        {
            return new VerifyNode(LoweredNodeKind.LoweredLabelStatement);
        }

        public static VerifyNode CallExpression(VerifyNode expr, params VerifyNode[] args)
        {
            return new VerifyNode(LoweredNodeKind.LoweredCallExpression, expr, Arguments(args));
        }

    }
}