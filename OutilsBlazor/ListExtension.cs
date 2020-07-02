using System;
using System.Collections.Generic;
using System.Text;

namespace OutilsBlazor
{
    public static class ListExtension
    {
        public static List<List<T>> SplitList<T>(this List<T> locations, int nSize = 30)
        {
            var list = new List<List<T>>();

            for (int i = 0; i < locations.Count; i += nSize)
            {
                list.Add(locations.GetRange(i, Math.Min(nSize, locations.Count - i)));
            }

            return list;
        }
    }
}
