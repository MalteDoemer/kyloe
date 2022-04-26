using System.IO;
using Kyloe.Utility;

namespace Kyloe.Diagnostics
{
    internal class DiagnosticWriter
    {
        private readonly TextWriter writer;
        private readonly SourceText sourceText;
        private readonly ColorMode colorMode;


        public DiagnosticWriter(TextWriter writer, SourceText sourceText, ColorMode color = ColorMode.None)
        {
            this.writer = writer;
            this.sourceText = sourceText;
            this.colorMode = color;
        }

        public void Write(DiagnosticResult result)
        {
            foreach (var diagnostic in result.GetAll())
                WriteDiagnostic(diagnostic);
        }

        public void WriteDiagnostic(Diagnostic diagnostic)
        {
            SetColor(diagnostic.Severity);

            if (diagnostic.Location is SourceLocation location)
            {
                var (line, col) = sourceText.GetStartLineColumn(location);
                if (line >= 1 && col >= 1)
                    if (sourceText.FileName is not null)
                        writer.Write($"{sourceText.FileName}:{line}:{col}: ");
                    else 
                        writer.Write($"{line}:{col}: ");
            }

            writer.WriteLine(diagnostic.Message);

            ResetColor();
        }

        private void SetColor(DiagnosticSeverity severity)
        {
            if (colorMode == ColorMode.ConsoleColor)
            {
                if (severity == DiagnosticSeverity.Error)
                    System.Console.ForegroundColor = System.ConsoleColor.Red;
                else if (severity == DiagnosticSeverity.Warn)
                    System.Console.ForegroundColor = System.ConsoleColor.Yellow;
            }
            else if (colorMode == ColorMode.AnsiColor)
            {
                if (severity == DiagnosticSeverity.Error)
                    writer.Write("\u001b[31;1m");
                else if (severity == DiagnosticSeverity.Warn)
                    writer.Write("\u001b[33;1m");
            }
        }

        private void ResetColor()
        {
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