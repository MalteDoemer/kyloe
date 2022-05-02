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