using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Outils.Module
{
    public class ListView<T> : ObservableCollection<T>
    {
        public ListView(IEnumerable<T> items) : base(items)
        {
            Modified = new List<T>();
            Added = new List<T>();
        }

        public List<T> Added { get; }

        public List<T> Modified { get; }

        public bool IsChanged => Modified.Count > 0 || Added.Count > 0;

        public void Modify(T item)
        {
            if (!Added.Contains(item))
                Modified.Add(item);
        }

        public new void Add(T item)
        {
            Added.Add(item);
        }
    }
}