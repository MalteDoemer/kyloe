using System.IO;

namespace Kyloe.Diagnostics
{
    internal class DiagnosticWriter
    {
        private readonly TextWriter writer;
        private readonly ColorMode colorMode;

        public DiagnosticWriter(TextWriter writer, ColorMode color = ColorMode.None)
        {
            this.writer = writer;
            this.colorMode = color;
        }

        public void Write(DiagnosticResult result)
        {
            foreach (var diagnostic in result.GetAll())
                WriteDiagnostic(diagnostic);
        }

        public void WriteDiagnostic(Diagnostic diagnostic)
        {
            if (colorMode == ColorMode.ConsoleColor)
            {
                if (diagnostic.Severity == DiagnosticSeverity.Error)
                    System.Console.ForegroundColor = System.ConsoleColor.Red;
                else if (diagnostic.Severity == DiagnosticSeverity.Warn)
                    System.Console.ForegroundColor = System.ConsoleColor.Yellow;
            }
            else if (colorMode == ColorMode.AnsiColor)
            {
                if (diagnostic.Severity == DiagnosticSeverity.Error)
                    writer.Write("\u001b[31;1m");
                else if (diagnostic.Severity == DiagnosticSeverity.Warn)
                    writer.Write("\u001b[33;1m");
            }

            writer.WriteLine(diagnostic.Message());

            if (colorMode == ColorMode.ConsoleColor)
                System.Console.ForegroundColor = System.ConsoleColor.White;
            else if (colorMode == ColorMode.AnsiColor)
                writer.Write("\u001b[0m");
        }

        public enum ColorMode
        {
            None,
            ConsoleColor,
            AnsiColor,
        }
    }
}