using System.Linq;
using System.Collections.Generic;

namespace Kyloe.Grammar
{
    public static class EnumerateExtension
    {
        public static IEnumerable<(int, T)> EnumerateIndex<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Select((x, i) => (i, x));
        }
    }
}