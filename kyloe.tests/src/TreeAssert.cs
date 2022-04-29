using System;
using System.Linq;
using System.Diagnostics;
using Xunit;
using Kyloe.Syntax;
using System.Collections.Generic;

namespace Kyloe.Tests
{
    public class VerifyNode
    {
        public VerifyNode(SyntaxTokenKind expected, params VerifyNode[] children)
        {
            Kind = expected;
            Children = children;
        }

        public SyntaxTokenKind Kind { get; }
        public VerifyNode[] Children { get; }

        
    }

    internal class TreeAssert
    {
        public static void Verify(SyntaxTree tree, VerifyNode verify)
        {
            Verify(tree.GetRoot(), verify);
        }

        public static void Verify(SyntaxToken token, VerifyNode verify)
        {
            Assert.Equal(token.Kind, verify.Kind);
            Assert.Equal(verify.Children.Length, token.Children().Count());

            foreach (var (child, verifyChild) in token.Children().Zip(verify.Children))
                Verify(child, verifyChild);
        }

    }
}