using System.Collections.Generic;
using Kyloe.Diagnostics;
using Xunit;

namespace Kyloe.Tests.Binding
{
    public class BindingTests
    {
        [Theory]
        [MemberData(nameof(GetErrorData))]
        public void Test_Binding_With_Errors(string text, params DiagnosticType[] errors)
        {
            var compilation = Compilation.Compile(text);
            DiagnosticAssert.HasAll(compilation.GetDiagnostics(), errors);
        }

        [Theory]
        [MemberData(nameof(GetNoErrorData))]
        public void Test_Binding_No_Errors(string text)
        {
            var compilation = Compilation.Compile(text);
            DiagnosticAssert.NoDiagnostics(compilation.GetDiagnostics());
        }


        public static IEnumerable<object[]> GetErrorData()
        {
            yield return new object[] {
                "1 + 1.5;",
                DiagnosticType.UnsupportedBinaryOperation,
            };

            yield return new object[] {
                "-1.2 + true;",
                DiagnosticType.UnsupportedBinaryOperation,
            };

            yield return new object[] {
                "-true == false;",
                DiagnosticType.UnsupportedUnaryOperation,
            };

            yield return new object[] {
                "~'f'",
                DiagnosticType.UnsupportedUnaryOperation,
            };
        }

        public static IEnumerable<object[]> GetNoErrorData()
        {
            yield return new object[] {
                "1 + 2 * 3 / 5 % 9 - 26;"
            };

            yield return new object[] {
                "1.1 + 2.2 * 3.3 / 5.5 % 9.9 - 26.6;"
            };

            yield return new object[] {
                "true == false;"
            };

            yield return new object[] {
                "6 < 2;"
            };

            yield return new object[] {
                "-1;"
            };

            yield return new object[] {
                "-1.5;"
            };

            yield return new object[] {
                "~1 ^ ~1;"
            };

            yield return new object[] {
                "!(5 == 6);"
            };

        }

    }
}