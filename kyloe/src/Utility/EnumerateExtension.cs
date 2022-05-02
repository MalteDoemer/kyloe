using System.Collections.Generic;
using System.Linq;

namespace Kyloe
{
    public static class EnumerateExtension
    {
        public static IEnumerable<(int, T)> EnumerateIndex<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Select((x, i) => (i, x));
        }
    }
}