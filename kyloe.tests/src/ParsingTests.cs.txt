using Xunit;

using Kyloe.Syntax;
using Kyloe.Diagnostics;
using System.Collections.Generic;

namespace Kyloe.Tests.Parsing
{
    public class ParsingTests
    {
        [Theory]
        [MemberData(nameof(GetExpressionErrorData))]
        public void Test_Parsing_Expression_With_Errors(string text, params DiagnosticKind[] errors)
        {
            var tree = SyntaxTree.ParseExpression(text);
            DiagnosticAssert.HasAll(tree.GetDiagnostics(), errors);
        }

        [Theory]
        [MemberData(nameof(GetStatementErrorData))]
        public void Test_Parsing_Statements_With_Errors(string text, params DiagnosticKind[] errors)
        {
            var tree = SyntaxTree.ParseStatement(text);
            DiagnosticAssert.HasAll(tree.GetDiagnostics(), errors);
        }

        [Theory]
        [MemberData(nameof(GetExpressionTreeData))]
        public void Test_Expression_Tree_Structure(string text, VerifyNode node)
        {
            var tree = SyntaxTree.ParseExpression(text);
            DiagnosticAssert.NoErrors(tree.GetDiagnostics());
            TreeAssert.Verify(tree, node);
        }

        [Theory]
        [MemberData(nameof(GetStatementTreeData))]
        public void Test_Statement_Tree_Structure(string text, VerifyNode node)
        {
            var tree = SyntaxTree.ParseStatement(text);
            DiagnosticAssert.NoErrors(tree.GetDiagnostics());
            TreeAssert.Verify(tree, node);
        }

        [Theory]
        [MemberData(nameof(GetProgramTreeData))]
        public void Test_Program_Tree_Structure(string text, VerifyNode node)
        {
            var tree = SyntaxTree.Parse(text);
            DiagnosticAssert.NoErrors(tree.GetDiagnostics());
            TreeAssert.Verify(tree, node);
        }

        public static IEnumerable<object[]> GetExpressionErrorData()
        {
            yield return new object[] {
                "arr[1)]",
                DiagnosticKind.ExpectedTokenError,
            };

            yield return new object[] {
                "()",
                DiagnosticKind.ExpectedExpressionError,
            };

            yield return new object[] {
                "((1)",
                DiagnosticKind.ExpectedTokenError,
            };

            yield return new object[] {
                "(1))",
                DiagnosticKind.ExpectedTokenError,
            };

            yield return new object[] {
                "1 + ",
                DiagnosticKind.ExpectedExpressionError
            };

            yield return new object[] {
                "x += ",
                DiagnosticKind.ExpectedExpressionError
            };

            yield return new object[] {
                "(1 ",
                DiagnosticKind.ExpectedTokenError
            };

            yield return new object[] {
                "hello(1, 2",
                DiagnosticKind.ExpectedTokenError
            };

            yield return new object[] {
                "data[36 + 1",
                DiagnosticKind.ExpectedTokenError
            };

            yield return new object[] {
                "x.y.",
                DiagnosticKind.ExpectedTokenError
            };
        }

        public static IEnumerable<object[]> GetStatementErrorData()
        {
            yield return new object[] {
                "1",
                DiagnosticKind.ExpectedTokenError,
            };

            yield return new object[] {
                "(1",
                DiagnosticKind.ExpectedTokenError,
            };

            yield return new object[] {
                "a[];",
                DiagnosticKind.ExpectedExpressionError,
            };

            yield return new object[] {
                "()",
                DiagnosticKind.ExpectedExpressionError,
                DiagnosticKind.ExpectedTokenError,
            };

            yield return new object[] {
                "var ",
                DiagnosticKind.ExpectedTokenError,
            };

            yield return new object[] {
                "var x = ;",
                DiagnosticKind.ExpectedExpressionError,
            };
        }

        public static IEnumerable<object[]> GetExpressionTreeData()
        {
            yield return new object[] {
                "-1",
                VerifyNode.UnaryExpression(
                    SyntaxTokenKind.Minus,
                    VerifyNode.LiteralExpression(SyntaxTokenKind.IntLiteral)
                )
            };

            yield return new object[] {
                "10.5 + -47.1",
                VerifyNode.BinaryExpression(
                    VerifyNode.LiteralExpression(SyntaxTokenKind.FloatLiteral),
                    SyntaxTokenKind.Plus,
                    VerifyNode.UnaryExpression(
                        SyntaxTokenKind.Minus,
                        VerifyNode.LiteralExpression(SyntaxTokenKind.FloatLiteral)
                    )
                )
            };

            yield return new object[] {
                "1 - -1",
                VerifyNode.BinaryExpression(
                    VerifyNode.LiteralExpression(SyntaxTokenKind.IntLiteral),
                    SyntaxTokenKind.Minus,
                    VerifyNode.UnaryExpression(
                        SyntaxTokenKind.Minus,
                        VerifyNode.LiteralExpression(SyntaxTokenKind.IntLiteral)
                    )
                )
            };

            yield return new object[] {
                "1 + 2",
                VerifyNode.BinaryExpression(
                    VerifyNode.LiteralExpression(SyntaxTokenKind.IntLiteral),
                    SyntaxTokenKind.Plus,
                    VerifyNode.LiteralExpression(SyntaxTokenKind.IntLiteral)
                )
            };

            yield return new object[] {
                "1 + 2 * 3",
                VerifyNode.BinaryExpression(
                    VerifyNode.LiteralExpression(SyntaxTokenKind.IntLiteral),
                    SyntaxTokenKind.Plus,
                    VerifyNode.BinaryExpression(
                        VerifyNode.LiteralExpression(SyntaxTokenKind.IntLiteral),
                        SyntaxTokenKind.Star,
                        VerifyNode.LiteralExpression(SyntaxTokenKind.IntLiteral)
                    )
                )
            };


            yield return new object[] {
                "(1 + 2) * 3",
                VerifyNode.BinaryExpression(
                    VerifyNode.ParenthsizedExpression(
                        VerifyNode.BinaryExpression(
                            VerifyNode.LiteralExpression(SyntaxTokenKind.IntLiteral),
                            SyntaxTokenKind.Plus,
                            VerifyNode.LiteralExpression(SyntaxTokenKind.IntLiteral)
                        )
                    ),
                    SyntaxTokenKind.Star,
                    VerifyNode.LiteralExpression(SyntaxTokenKind.IntLiteral)
                )
            };

            yield return new object[] {
                "arr[5]",
                VerifyNode.SubscriptExpression(
                    VerifyNode.IdentifierExpression(),
                    VerifyNode.LiteralExpression(SyntaxTokenKind.IntLiteral)
                )
            };

            yield return new object[] {
                "x.y",
                VerifyNode.MemberAccessExpression(
                    VerifyNode.IdentifierExpression()
                )
            };

            yield return new object[] {
                "arr[5].x.y",
                VerifyNode.MemberAccessExpression(
                    VerifyNode.MemberAccessExpression(
                        VerifyNode.SubscriptExpression(
                            VerifyNode.IdentifierExpression(),
                            VerifyNode.LiteralExpression(SyntaxTokenKind.IntLiteral)
                        )
                    )
                )
            };

            yield return new object[] {
                "hello()",
                VerifyNode.CallExpression(
                    VerifyNode.IdentifierExpression()
                )
            };

            yield return new object[] {
                "hello(1, 'hello')",
                VerifyNode.CallExpression(
                    VerifyNode.IdentifierExpression(),
                    VerifyNode.LiteralExpression(SyntaxTokenKind.IntLiteral),
                    VerifyNode.LiteralExpression(SyntaxTokenKind.StringLiteral)
                )
            };

            yield return new object[] {
                "x += 3",
                VerifyNode.AssignmentExpression(
                    VerifyNode.IdentifierExpression(),
                    SyntaxTokenKind.PlusEquals,
                    VerifyNode.LiteralExpression(SyntaxTokenKind.IntLiteral)
                )
            };
        }

        public static IEnumerable<object[]> GetStatementTreeData()
        {
            yield return new object[] {
                "x += 3;",
                VerifyNode.ExpressionStatement(
                    VerifyNode.AssignmentExpression(
                        VerifyNode.IdentifierExpression(),
                        SyntaxTokenKind.PlusEquals,
                        VerifyNode.LiteralExpression(SyntaxTokenKind.IntLiteral)
                    )
                )
            };

            yield return new object[] {
                "hello(1.2);",
                VerifyNode.ExpressionStatement(
                    VerifyNode.CallExpression(
                        VerifyNode.IdentifierExpression(),
                        VerifyNode.LiteralExpression(SyntaxTokenKind.FloatLiteral)
                    )
                )
            };

            yield return new object[] {
                ";",
                VerifyNode.EmptyStatement()
            };

            yield return new object[] {
                "var x = 5;",
                VerifyNode.DeclarationStatement(
                    SyntaxTokenKind.VarKeyword,
                    VerifyNode.LiteralExpression(SyntaxTokenKind.IntLiteral)
                )
            };

            yield return new object[] {
                @"
                if true == false {
                    ;
                }   
                ",

                VerifyNode.IfStatement(
                    VerifyNode.BinaryExpression(
                        VerifyNode.LiteralExpression(SyntaxTokenKind.BoolLiteral),
                        SyntaxTokenKind.DoubleEqual,
                        VerifyNode.LiteralExpression(SyntaxTokenKind.BoolLiteral)
                    ),
                    VerifyNode.BlockStatement(
                        VerifyNode.EmptyStatement()
                    )
                )
            };

            yield return new object[] {
                @"
                if true == false {
                    ;
                } else {
                    ;
                }
                ",

                VerifyNode.IfElseStatement(
                    VerifyNode.BinaryExpression(
                        VerifyNode.LiteralExpression(SyntaxTokenKind.BoolLiteral),
                        SyntaxTokenKind.DoubleEqual,
                        VerifyNode.LiteralExpression(SyntaxTokenKind.BoolLiteral)
                    ),
                    VerifyNode.BlockStatement(
                        VerifyNode.EmptyStatement()
                    ),

                    VerifyNode.BlockStatement(
                        VerifyNode.EmptyStatement()
                    )
                )
            };

            yield return new object[] {
                "{ var x = 5; var y = x + 25; } ",
                VerifyNode.BlockStatement(
                    VerifyNode.DeclarationStatement(
                        SyntaxTokenKind.VarKeyword,
                        VerifyNode.LiteralExpression(SyntaxTokenKind.IntLiteral)
                    ),
                    VerifyNode.DeclarationStatement(
                        SyntaxTokenKind.VarKeyword,
                        VerifyNode.BinaryExpression(
                            VerifyNode.IdentifierExpression(),
                            SyntaxTokenKind.Plus,
                            VerifyNode.LiteralExpression(SyntaxTokenKind.IntLiteral)
                        )
                    )

                )
            };
        }

        public static IEnumerable<object[]> GetProgramTreeData()
        {
            yield return new object[] {
                @"
                func main() {
                }
                ",

                VerifyNode.CompilationUnitSyntax(
                    VerifyNode.FunctionDefinition(0, VerifyNode.BlockStatement(), false)
                ),
            };

            yield return new object[] {
                @"
                func test(a: i32, b: double, c: float) -> bool {
                }
                ",

                VerifyNode.CompilationUnitSyntax(
                    VerifyNode.FunctionDefinition(3, VerifyNode.BlockStatement(), true)
                ),
            };

            yield return new object[] {
                @"
                var globalVar = 12345;
                const globalConst = 5.4;

                func test(a: i32, b: double, c: float) -> bool {
                    var x = globalVar;
                }
                ",

                VerifyNode.CompilationUnitSyntax(
                    VerifyNode.DeclarationStatement(
                        SyntaxTokenKind.VarKeyword,
                        VerifyNode.LiteralExpression(SyntaxTokenKind.IntLiteral)
                    ),
                    VerifyNode.DeclarationStatement(
                        SyntaxTokenKind.ConstKeyword,
                        VerifyNode.LiteralExpression(SyntaxTokenKind.FloatLiteral)
                    ),
                    VerifyNode.FunctionDefinition(
                        3,
                        VerifyNode.BlockStatement(
                            VerifyNode.DeclarationStatement(
                                SyntaxTokenKind.VarKeyword, 
                                VerifyNode.IdentifierExpression()
                            )
                        ),
                        true
                    )
                ),
            };
        }
    }
}