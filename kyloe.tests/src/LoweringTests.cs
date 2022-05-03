using Xunit;
using System.Collections.Generic;

using static Kyloe.Tests.Lowering.VerifyNode;

namespace Kyloe.Tests.Lowering
{
    public class LoweringTests
    {
        [Theory]
        [MemberData(nameof(GetStatementData))]
        public void Test_Statement_Lowering(string text, params VerifyNode[] statements)
        {
            text = $"func main() {{ {text} }}";
            var node = CompilationUnit(BlockStatement(), FunctionDefinition(statements));

            var compilation = Compilation.Compile(text);

            DiagnosticAssert.NoErrors(compilation.GetDiagnostics());
            LoweredTreeAssert.AssertNode(node, compilation.GetRoot()!); // not null if no errors
        }

        public static IEnumerable<object[]> GetStatementData()
        {
            yield return new object[] {
                "var x = 5;",
                DeclarationStatement(null),
                ExpressionStatement(Assignment(
                    VariableAccessExpression(),
                    LiteralExpression()
                )),
            };

            yield return new object[] {
                "var x = 1; x += 1;",
                DeclarationStatement(null),
                ExpressionStatement(Assignment(
                    VariableAccessExpression(),
                    LiteralExpression()
                )),
                ExpressionStatement(Assignment(
                    VariableAccessExpression(),
                    BinaryExpression(
                        VariableAccessExpression(),
                        LiteralExpression()
                    )
                )),
            };

            yield return new object[] {
                @"{
                    var x = 1;
                    {
                        var x = 2;
                    }
                }",
                DeclarationStatement(null),
                ExpressionStatement(Assignment(
                    VariableAccessExpression(),
                    LiteralExpression()
                )),
                DeclarationStatement(null),
                ExpressionStatement(Assignment(
                    VariableAccessExpression(),
                    LiteralExpression()
                )),
            };

            var printVerifyNode = ExpressionStatement(CallExpression(FunctionAccessExpression(), LiteralExpression()));

            yield return new object[] {
                @"{
                    if true {
                        println('hi');
                    }
                }",
                ConditionalGotoStatement(UnaryExpression(LiteralExpression())), // goto end if not condition
                printVerifyNode,
                LabelStatement(), // end: 
            };

            yield return new object[] {
                @"{
                    if false {
                        println('hi');
                    } else {
                        println('hey');
                    }
                }",
                
                ConditionalGotoStatement(UnaryExpression(LiteralExpression())), // goto else if not condition
                printVerifyNode,
                GotoStatement(), // goto end
                LabelStatement(), // else: 
                printVerifyNode,
                LabelStatement(), // end: 
            };

            yield return new object[] {
                @"{
                    if false {
                        println('hi');
                    } elif 1 == 2 {
                        println('ney');
                    } else {
                        println('hey');
                    }
                }",
                
                ConditionalGotoStatement(UnaryExpression(LiteralExpression())), // goto outerElse if not condition
                printVerifyNode,
                GotoStatement(), // goto outerEnd
                LabelStatement(), // outerElse:
                ConditionalGotoStatement(UnaryExpression(BinaryExpression(LiteralExpression(), LiteralExpression()))), // goto innerElse if not condition
                printVerifyNode,
                GotoStatement(),  // innerEnd:
                LabelStatement(), // innerElse:
                printVerifyNode,
                LabelStatement(), // innerEnd:
                LabelStatement(), // outerEnd:
            };


            yield return new object[] {
                @"{
                    var x = 0;
                    while true {
                        println('hi');

                        if x == 5 {
                            break;
                        }
                        
                        x += 1;
                    }
                }",
                

                DeclarationStatement(null),
                ExpressionStatement(Assignment(VariableAccessExpression(), LiteralExpression())),
                GotoStatement(),  // goto check_condition
                LabelStatement(), // continue:
                printVerifyNode, // println('hi');
                ConditionalGotoStatement(UnaryExpression(BinaryExpression(VariableAccessExpression(), LiteralExpression()))), // goto end if not condition
                GotoStatement(),  // goto break 
                LabelStatement(), // end:
                ExpressionStatement(Assignment(VariableAccessExpression(), BinaryExpression(VariableAccessExpression(), LiteralExpression()))), // x += 1
                LabelStatement(), // check_condition:
                ConditionalGotoStatement(LiteralExpression()), // goto continue if condition
                LabelStatement(), // break:
            };
        }

        [Theory]
        [MemberData(nameof(GetWholeProgramData))]
        public void Test_Whole_Program_Lowering(string text, VerifyNode node)
        {
            var compilation = Compilation.Compile(text);

            DiagnosticAssert.NoErrors(compilation.GetDiagnostics());
            LoweredTreeAssert.AssertNode(node, compilation.GetRoot()!); // not null if no errors
        }

        public static IEnumerable<object[]> GetWholeProgramData()
        {
            yield return new object[] {
                "func main() { }",
                VerifyNode.CompilationUnit(
                    BlockStatement(),
                    FunctionDefinition()
                )
            };
        }
    }
}