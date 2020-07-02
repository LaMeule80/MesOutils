using System.Collections.Generic;
using System.Linq;

namespace Outils.Controls
{
    public static class IEnumerableExtensions
    {
        public static bool IsNotEmpty<T>(this IEnumerable<T> e)
        {
            return e?.Any() ?? false;
        }
    }
}
