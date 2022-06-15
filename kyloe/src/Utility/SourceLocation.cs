using System;
using System.Diagnostics;

namespace Kyloe.Utility
{
    public struct SourceLocation
    {
        private SourceLocation(int start, int length, SourceText sourceText)
        {
            Start = start;
            Length = length;
            SourceText = sourceText;
        }

        public int Start { get; }
        public int Length { get; }
        public SourceText SourceText { get; }

        public int End => Start + Length;

        public static SourceLocation FromBounds(SourceText sourceText, int start, int end) => new SourceLocation(start, end - start, sourceText);

        public static SourceLocation FromLength(SourceText sourceText, int start, int length) => new SourceLocation(start, length, sourceText);

        public static SourceLocation CreateAround(SourceLocation loc1, SourceLocation loc2)
        {
            Debug.Assert(loc1.SourceText == loc2.SourceText);

            var start = Math.Min(loc1.Start, loc2.Start);
            var end = Math.Max(loc1.End, loc2.End);
            return new SourceLocation(start, end, loc1.SourceText);
        }

        public override string ToString() => $"{Start}..{End}";
    }
}