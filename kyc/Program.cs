using System;
using System.Text;
using Kyloe.Syntax;
using Kyloe.Diagnostics;
using Kyloe.Utility;

namespace Kyc
{
    class Program
    {
        public static void Main()
        {
            Console.WriteLine("Code:");

            var diagnostics = new DiagnosticCollecter();

            var parser = new Parser(Console.In, diagnostics);
            var tree = parser.Parse();

            Console.WriteLine();
            PrintDiagnostics(diagnostics);
            var writer = new PrettyWriter(Console.Out);
            writer.Write(tree);

        }

        private static void PrintDiagnostics(DiagnosticCollecter diagnostics)
        {
            var prevColor = Console.ForegroundColor;

            if (diagnostics.HasWarnings())
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                foreach (var warning in diagnostics.GetWarnings())
                    PrintDiagnostic(warning);
            }

            if (diagnostics.HasErrors())
            {
                Console.ForegroundColor = ConsoleColor.Red;

                foreach (var error in diagnostics.GetErrors())
                    PrintDiagnostic(error);
            }

            Console.ForegroundColor = prevColor;
        }

        private static void PrintDiagnostic(Diagnostic diagnostic)
        {
            Console.WriteLine(diagnostic.Message());
        }

        private static bool EvaluteDollarCommand(string input)
        {
            if (input.StartsWith("$exit"))
            {
                return false;
            }
            else if (input.StartsWith("$clear"))
            {
                Console.Clear();
                return true;
            }
            else
            {
                Console.WriteLine($"Invalid dollar command: {input}");
                return true;
            }
        }
    }
}