using System;

namespace Kyloe.Utility
{
    public struct SourceLocation
    {
        private SourceLocation(int start, int length)
        {
            Start = start;
            Length = length;
        }

        public int Start { get; }
        public int Length { get; }
        public int End => Start + Length;

        public static SourceLocation FromBounds(int start, int end) => new SourceLocation(start, end - start);

        public static SourceLocation FromLength(int start, int length) => new SourceLocation(start, length);

        public static SourceLocation CreateAround(SourceLocation loc1, SourceLocation loc2)
        {
            var start = Math.Min(loc1.Start, loc2.Start);
            var end = Math.Max(loc1.End, loc2.End);
            return new SourceLocation(start, end);
        }

        public override string ToString() => $"{Start}..{End}";
    }
}