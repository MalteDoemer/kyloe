using System.Collections.Generic;
using System.Linq;
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

        private static string AddScope(string text)
        {
            return "{" + text + "}";
        }


        [Theory]
        [MemberData(nameof(GetCombinedStatementData))]
        public void Test_Binding_Statements_Combined(string text, IEnumerable<DiagnosticKind> errors)
        {
            var compilation = Compilation.Compile(AddMainFunction(text));
            DiagnosticAssert.Equal(compilation.GetDiagnostics(), errors);
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
                "if true {} elif false {} elif true {} else {}"
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
                "var str: string = 'hello' + ' ' + 'world!';"
            };

            yield return new object[] {
                "var x = 'hello' ==  'world!';"
            };

            yield return new object[] {
                "var x = 'hello' !=  'world!';"
            };

            yield return new object[] {
                "var x: i32 = i32(5);"
            };

            yield return new object[] {
                "var x = float(5); var f: float = x;"
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
                "!5;",
                DiagnosticKind.UnsupportedUnaryOperation,
            };

            yield return new object[] {
                "~'f';",
                DiagnosticKind.UnsupportedUnaryOperation,
            };

            yield return new object[] {
                @"while true {
                    println('hi');
                }"
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
                "1 % 8 = 5;",
                DiagnosticKind.ExpectedModifiableValueError,
            };

            yield return new object[] {
                "var x = i32;",
                DiagnosticKind.ExpectedValueError,
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

            yield return new object[] {
                "if true {} else {} else {}",
                DiagnosticKind.IllegalElseStatement,
            };

            yield return new object[] {
                "if true {} else {} elif true {}",
                DiagnosticKind.IllegalElifStatement,
            };

            yield return new object[] {
                @"while 1 {
                    println('hi');
                }",
                DiagnosticKind.MissmatchedTypeError,
            };

            yield return new object[] {
                "5(1, 2);",
                DiagnosticKind.NotCallableError,
            };


            yield return new object[] {
                "println('x')();",
                DiagnosticKind.NotCallableError,
            };

            yield return new object[] {
                "9223372036854775808;",
                DiagnosticKind.InvalidLiteralError,
            };

            yield return new object[] {
                "var x: float = float('hi');",
                DiagnosticKind.NoExplicitConversionExists,
            };
        }

        public static IEnumerable<object[]> GetCombinedStatementData()
        {
            var data = GetStatementData().Select(arr => ((string)arr[0], arr.Skip(1).Cast<DiagnosticKind>()));

            foreach (var (d1, d2) in data.Zip(data))
            {
                // we have to add scopes, otherwise it could cause redefined name errors
                var text = AddScope(d1.Item1) + AddScope(d2.Item1);
                var errors = d1.Item2.Concat(d2.Item2);

                yield return new object[] { text, errors };
            }
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
                func sayHello(name: ThisTypeDoesNotExist) {
                    println('Hello');
                    println(name);
                }

                func main() {
                    sayHello('Kyloe');
                }
                ",
                DiagnosticKind.NonExistantNameError,
                DiagnosticKind.NoMatchingOverloadError,
            };

            yield return new object[] {
                @"
                func main() {
                    sayHello('Kyloe');
                }
                ",
                DiagnosticKind.NonExistantNameError,
            };

            yield return new object[] {
                @"
                func test(a: i32, b: float, c: double, b: float) {}
                ",
                DiagnosticKind.RedefinedParameterError
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
                func test() {}

                func test() {}
                ",
                DiagnosticKind.OverloadWithSameParametersExistsError
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
                var test = 5;

                func test(c: i32) {}

                ",
                DiagnosticKind.NameAlreadyExistsError,
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

            yield return new object[] {
                @"
                func test() -> i64 { return 6; }

                func main() {
                    var x: string = test();
                }
                ",
                DiagnosticKind.MissmatchedTypeError,
            };


            yield return new object[] {
                @"
                const CONSTANT = 7;

                func main() {
                    CONSTANT = 5;    
                }
                ",
                DiagnosticKind.ExpectedModifiableValueError,
            };

            yield return new object[] {
                @"
                func other() {}
                func test() -> other { return 5; } 
                ",
                DiagnosticKind.ExpectedTypeNameError,
            };

            yield return new object[] {
                @"
                func test() -> i64 { return 5; } 
                func main() {
                    test() = 5;    
                }
                ",
                DiagnosticKind.ExpectedModifiableValueError,
            };

            yield return new object[] {
                @"
                func main() {
                    break;
                }
                ",
                DiagnosticKind.IllegalBreakStatement,
            };

            yield return new object[] {
                @"
                func main() {
                    continue;
                }
                ",
                DiagnosticKind.IllegalContinueStatement,
            };

        }

    }
}