using System;
using System.Collections.Generic;

namespace Outils.Helper
{
    [Serializable]
    public class DataProvider<T1, T2, TValue> : Dictionary<Tuple<T1, T2>, TValue>
    {
        public TValue this[T1 key1, T2 key2] => base[Tuple.Create(key1, key2)];

        public void Add(T1 key1, T2 key2, TValue value)
        {
            base.Add(new Tuple<T1, T2>(key1, key2), value);
        }

        public void AddRange(List<Tuple<T1, T2, TValue>> values)
        {
            values.ForEach(x => Add(x.Item1, x.Item2, x.Item3));
        }

        public bool ContainsKey(T1 key1, T2 key2)
        {
            return base.ContainsKey(Tuple.Create(key1, key2));
        }
    }

    [Serializable]
    public class DataProvider<T1, TValue> : Dictionary<T1, TValue>
    {
        public new void Add(T1 key, TValue value)
        {
            if (!ContainsKey(key))
                base.Add(key, value);
        }

        public void AddRange(List<Tuple<T1, TValue>> values)
        {
            values.ForEach(x => Add(x.Item1, x.Item2));
        }

        public new bool ContainsKey(T1 key)
        {
            return base.ContainsKey(key);
        }
    }
}