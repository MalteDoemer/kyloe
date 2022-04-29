using System.Collections.Generic;
using Kyloe.Diagnostics;
using Xunit;

namespace Kyloe.Tests.Binding
{
    public class BindingTests
    {
        [Theory]
        [MemberData(nameof(GetStatementData))]
        public void Test_Binding_Statements(string text, params DiagnosticKind[] errors)
        {
            var compilation = Compilation.Compile(AddMainFunction(text));
            DiagnosticAssert.Equal(compilation.GetDiagnostics(), errors);
        }

        private static string AddMainFunction(string text)
        {
            return "func main() {" + text + "}";
        }

        [Theory]
        [MemberData(nameof(GetWholeProgramData))]
        public void Test_Whole_Program_Binding(string text, params DiagnosticKind[] errors)
        {
            var compilation = Compilation.Compile(text);
            DiagnosticAssert.Equal(compilation.GetDiagnostics(), errors);
        }

        public static IEnumerable<object[]> GetStatementData()
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

            yield return new object[] {
                "'hi' == 'hey';"
            };

            yield return new object[] {
                "{}"
            };

            yield return new object[] {
                ";"
            };

            yield return new object[] {
                "{;}"
            };

            yield return new object[] {
                @"{
                    var x = 1;
                }"
            };

            yield return new object[] {
                @"{
                    const x = 1;
                }"
            };

            yield return new object[] {
                @"{
                    var x = 1;
                    var y = 'hi';
                    var z = 1 + x;
                }"
            };

            yield return new object[] {
                @"{
                    const x = 1;
                    const y = 'hi';
                    const z = 1 + x;
                }"
            };

            yield return new object[] {
                @"{
                    var x = 1;
                    {
                        var x = 2;
                        {
                            var x = 3;
                            var y = x + 1;
                        }

                        const y = x + 1;
                    }

                    var y = x + 1;
                }"
            };

            yield return new object[] {
                @"{
                    var x = 5;

                    if (x == 5) {
                        x = 1;
                    } else {
                        x = 2;
                    }
                }"
            };

            yield return new object[] {
                @"{
                    var num1 = 5.6;
                    var num2 = num1 + 1.0;

                    if (num1 == num2) {
                        num1 = 0.0;
                    } else {
                        num1 += num2 - 5.0;
                    }
                }"
            };

            yield return new object[] {
                "var x: i64 = 5;"
            };

            yield return new object[] {
                "var b: bool = 1 == 5;"
            };

            yield return new object[] {
                "var str: string = 'hello world!';"
            };

            yield return new object[] {
                "1 + 1.5;",
                DiagnosticKind.UnsupportedBinaryOperation,
            };

            yield return new object[] {
                "-1.2 + true;",
                DiagnosticKind.UnsupportedBinaryOperation,
            };

            yield return new object[] {
                "-true == false;",
                DiagnosticKind.UnsupportedUnaryOperation,
            };

            yield return new object[] {
                "~'f'",
                DiagnosticKind.UnsupportedUnaryOperation,
            };

            yield return new object[] {
                "if (1) {}",
                DiagnosticKind.MissmatchedTypeError,
            };

            yield return new object[] {
                @"{
                    var x = 5;
                    var y: x = 5;
                }",
                DiagnosticKind.ExpectedTypeNameError,
            };

            yield return new object[] {
                @"{
                    var x = 5;
                    var y = x + 'hi';
                }",
                DiagnosticKind.UnsupportedBinaryOperation,
            };


            yield return new object[] {
                @"{
                    var x = 5;

                    {
                        var x = 6;
                    }

                    var x = 1;
                }",
                DiagnosticKind.NameAlreadyExistsError,
            };

            yield return new object[] {
                @"{
                    var x = x;
                }",
                DiagnosticKind.NonExistantNameError,
            };

            yield return new object[] {
                @"{
                    var x = 5;
                    var z = x + y;
                }",
                DiagnosticKind.NonExistantNameError,
            };

            yield return new object[] {
                @"{
                    var x = 5;
                    x = 'f';
                }",
                DiagnosticKind.MissmatchedTypeError,
            };

            yield return new object[] {
                @"{
                    var x = 5;
                    x += 'f';
                }",
                DiagnosticKind.UnsupportedAssignmentOperation,
            };

            yield return new object[] {
                @"{
                    var x = 5.2;
                    x %= true;
                }",
                DiagnosticKind.UnsupportedAssignmentOperation,
            };

            yield return new object[] {
                @"{
                    1 % 8 = 5;
                }",
                DiagnosticKind.ExpectedModifiableValueError,
            };


            yield return new object[] {
                @"{
                    var x = 1;
                    var y = x = 3;
                    var z = x += 2;
                }",
                DiagnosticKind.ExpectedValueError,
                DiagnosticKind.ExpectedValueError,
            };

            yield return new object[] {
                @"{
                    const x = 1;
                    x = 2;
                }",
                DiagnosticKind.ExpectedModifiableValueError,
            };
        }

        public static IEnumerable<object[]> GetWholeProgramData()
        {
            yield return new object[] {
                @"
                func main() {
                    println('Hello World');
                }
                "
            };

            yield return new object[] {
                @"
                func sayHello(name: string) {
                    println('Hello');
                    println(name);
                }

                func main() {
                    sayHello('Kyloe');
                }
                "
            };

            yield return new object[] {
                @"
                func test(a: i32, a: float) {}

                func main() {}
                ",
                DiagnosticKind.RedefinedParameterError
            };

            yield return new object[] {
                @"
                func test(a: i32) {}

                func test(c: i32) {}

                func main() {}
                ",
                DiagnosticKind.OverloadWithSameParametersExistsError
            };

            yield return new object[] {
                @"
                func test(a: i64) {}

                func main() {
                    test(5, 5);
                }
                ",
                DiagnosticKind.NoMatchingOverloadError
            };


        }

    }
}