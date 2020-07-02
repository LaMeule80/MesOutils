using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.DataGrid;

namespace OutilsBlazor
{
    public abstract class Pages<T> : Base where T : IEntity
    {
        private T _item;
        public T Item 
        {
            get => _item;
            set
            {
                _item = value;
                ItemRefresh();
            }
        }

        public virtual void ItemRefresh()
        {

        }

        public List<T> Items { get; set; }

        protected override Task OnInitializedAsync()
        {
            Items = GetItems().OrderBy(x => x.Nom).ToList();
            return Task.CompletedTask;
        }

        public abstract List<T> GetItems();

        public void Refresh()
        {
            OnInitializedAsync();
        }
        
        public void Insert(SavedRowItem<T, Dictionary<string, object>> e)
        {
            e.Item.Insert();
            Refresh();
        }

        public void Updated(SavedRowItem<T, Dictionary<string, object>> e)
        {
            e.Item.Update();
        }

        public void Removed(T e)
        {
            e.Delete();
            Refresh();
        }
    }
}