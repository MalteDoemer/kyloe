using Kyloe.Syntax;
using Kyloe.Diagnostics;
using System;

namespace Kyloe.Semantics
{
    internal class Binder
    {
        private DiagnosticCollector diagnostics;

        public Binder(DiagnosticCollector diagnostics)
        {
            this.diagnostics = diagnostics;
        }


        public BoundExpression BindExpression(SyntaxExpression expr)
        {
            switch (expr.Type)
            {
                case SyntaxNodeType.MalformedExpression:
                    return BindMalformedExpression((MalformedExpression)expr);
                case SyntaxNodeType.LiteralExpression:
                    return BindLiteralExpression((LiteralExpression)expr);
                case SyntaxNodeType.UnaryExpression:
                    return BindUnaryExpression((UnaryExpression)expr);
                case SyntaxNodeType.BinaryExpression:
                    return BindBinaryExpression((BinaryExpression)expr);
                case SyntaxNodeType.ParenthesizedExpression:
                    return BindParenthesizedExpression((ParenthesizedExpression)expr);
                case SyntaxNodeType.NameExpression:
                    return BindNameExpression((NameExpression)expr);
                case SyntaxNodeType.MemberAccessExpression:
                    return BindMemberAccessExpression((MemberAccessExpression)expr);
                case SyntaxNodeType.SubscriptExpression:
                    return BindSubscriptExpression((SubscriptExpression)expr);
                case SyntaxNodeType.CallExpression:
                    return BindCallExpression((CallExpression)expr);
                case SyntaxNodeType.AssignmentExpression:
                    return BindAssignmentExpression((AssignmentExpression)expr);
                default:
                    throw new System.Exception($"Unexpected SyntaxExpression: {expr.Type}");
            }
        }

        private BoundExpression BindAssignmentExpression(AssignmentExpression expr)
        {
            throw new NotImplementedException();
        }

        private BoundExpression BindCallExpression(CallExpression expr)
        {
            throw new NotImplementedException();
        }

        private BoundExpression BindSubscriptExpression(SubscriptExpression expr)
        {
            throw new NotImplementedException();
        }

        private BoundExpression BindMemberAccessExpression(MemberAccessExpression expr)
        {
            throw new NotImplementedException();
        }

        private BoundExpression BindNameExpression(NameExpression expr)
        {
            throw new NotImplementedException();
        }

        private BoundExpression BindParenthesizedExpression(ParenthesizedExpression expr)
        {
            throw new NotImplementedException();
        }

        private BoundExpression BindBinaryExpression(BinaryExpression expr)
        {
            throw new NotImplementedException();
        }

        private BoundExpression BindUnaryExpression(UnaryExpression expr)
        {
            throw new NotImplementedException();
        }

        private BoundExpression BindLiteralExpression(LiteralExpression expr)
        {
            throw new NotImplementedException();
        }

        private BoundExpression BindMalformedExpression(MalformedExpression expr)
        {
            throw new NotImplementedException();
        }
    }
}