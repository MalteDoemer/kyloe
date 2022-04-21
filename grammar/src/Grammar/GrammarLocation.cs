
using System;

namespace Kyloe.Grammar
{

    public struct GrammarLocation
    {
        private GrammarLocation(int start, int length, int line, int column)
        {
            Start = start;
            Length = length;
            Line = line;
            Column = column;
        }

        public int Start { get; }
        public int Length { get; }
        public int Line { get; }
        public int Column { get; }

        public int End => Start + Length;

        public static GrammarLocation FromBounds(int start, int end, int line, int column) => new GrammarLocation(start, end - start, line, column);

        public static GrammarLocation FromLength(int start, int length, int line, int column) => new GrammarLocation(start, length, line, column);

        public static GrammarLocation CreateAround(GrammarLocation loc1, GrammarLocation loc2)
        {
            var start = loc1.Start <= loc2.Start ? loc1 : loc2;
            var end = loc1.End >= loc2.End ? loc1 : loc2;
            return FromBounds(start.Start, end.End, start.Line, start.Column);
        }

        public override string ToString() => $"{Line}:{Column}";
    }
}