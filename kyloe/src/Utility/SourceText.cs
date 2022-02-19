
using System.IO;

namespace Kyloe.Utility
{
    public abstract class SourceText
    {
        public abstract string? FileName { get; }

        public abstract TextReader GetReader();

        public (int, int) GetStartLineColumn(SourceLocation location)
        {
            var lineEndPos = 0;
            var lineStartPos = 0;
            var lineCount = 1;

            using (var reader = GetReader())
            {
                while (reader.ReadLine() is string line)
                {
                    lineStartPos = lineEndPos;
                    lineEndPos += line.Length;

                    if (location.Start <= lineEndPos)
                    {
                        var lineOffset = location.Start - lineStartPos;
                        var col = lineOffset + 1;

                        return (lineCount, col);
                    }

                    lineCount++;
                }
            }

            return (-1, -1);
        }

        public (int, int) GetEndLineColumn(SourceLocation location)
        {
            var lineEndPos = 0;
            var lineStartPos = 0;
            var lineCount = 1;

            using (var reader = GetReader())
            {
                while (reader.ReadLine() is string line)
                {
                    lineStartPos = lineEndPos;
                    lineEndPos += line.Length;

                    if (location.End <= lineEndPos)
                    {
                        var lineOffset = location.End - lineStartPos;
                        var col = lineOffset + 1;

                        return (lineCount, col);
                    }

                    lineCount++;
                }
            }

            return (-1, -1);
        }

        public static SourceText FromText(string text) => new StringSourceText(text);
    }


    internal class StringSourceText : SourceText
    {
        private readonly string text;

        internal StringSourceText(string text)
        {
            this.text = text;
        }

        public override string? FileName => null;

        public override TextReader GetReader() => new StringReader(text);
    }
}