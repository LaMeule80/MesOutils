using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Outils.Helper;

namespace Outils.Controls.ComboBox
{
    public abstract class UcSearch<TItem> : UserControl, IDisposable
        where TItem : class
    {
        private readonly Func<TItem, ItemInfo> _itemInfoProvider;
        private Dictionary<Guid, TItem> _dico;
        private List<SearchItem> _sources;
        private TbSearch _tbSearch;

        protected UcSearch(Func<TItem, ItemInfo> itemInfoProvider)
        {
            _itemInfoProvider = itemInfoProvider;
        }

        public void Dispose()
        {
            _tbSearch?.Dispose();
        }

        protected struct ItemInfo
        {
            public Guid ItemId;
            public string Text;
            public bool EstFavori;
            public int NbreElts;
        }

        #region Propriétés

        public IEnumerable<TItem> ItemsSource
        {
            get => (IEnumerable<TItem>) GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(IEnumerable<TItem>),
                typeof(UcSearch<TItem>),
                new PropertyMetadata(null, HandleProperyChanged));

        public TItem CurrentItem
        {
            get => (TItem) GetValue(CurrentItemProperty);
            set => SetValue(CurrentItemProperty, value);
        }

        public static DependencyProperty CurrentItemProperty =
            DependencyProperty.Register(
                nameof(CurrentItem),
                typeof(TItem),
                typeof(UcSearch<TItem>),
                new PropertyMetadata(null, HandleProperyChanged));

        public Guid? CurrentItemValue
        {
            get => (Guid?) GetValue(CurrentItemValuePropertyKey.DependencyProperty);
            private set => SetValue(CurrentItemValuePropertyKey, value);
        }

        private static readonly DependencyPropertyKey CurrentItemValuePropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(CurrentItemValue),
                typeof(Guid?),
                typeof(UcSearch<TItem>), new PropertyMetadata(null));

        #endregion

        #region Evenements

        private static void HandleProperyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is UcSearch<TItem> s))
                return;

            if (e.Property == ItemsSourceProperty)
                s.OnItemsSourceChanged();
            else if (e.Property == CurrentItemProperty)
                s.OnCurrentItemChanged();
        }

        private void OnItemsSourceChanged()
        {
            if (_tbSearch != null)
                _tbSearch.CurrentItemChanged -= TbSearchCurrentItemChanged;

            if (ItemsSource == null)
            {
                if (_tbSearch == null) return;
                Content = null;
                _tbSearch = null;
            }
            else
            {
                _tbSearch = new TbSearch {Margin = new Thickness(0)};
                _sources = new List<SearchItem>();
                _dico = new Dictionary<Guid, TItem>();

                foreach (var c in ItemsSource)
                {
                    var context = DataContext as ISceneForm;
                    if (context != null)
                    {
                        if (Filtrer(context, c))
                        {
                            var itemInfo = _itemInfoProvider(c);

                            _dico.Add(itemInfo.ItemId, c);
                            _sources.Add(new SearchItem(itemInfo.ItemId, itemInfo.Text, itemInfo.EstFavori,
                                itemInfo.NbreElts));
                        }
                        else
                        {
                            var itemInfo = _itemInfoProvider(c);
                            _dico.Add(itemInfo.ItemId, c);
                            _sources.Add(new SearchItem(itemInfo.ItemId, itemInfo.Text, itemInfo.EstFavori,
                                itemInfo.NbreElts));
                        }
                    }
                    else
                    {
                        var itemInfo = _itemInfoProvider(c);
                        _dico.Add(itemInfo.ItemId, c);
                        _sources.Add(new SearchItem(itemInfo.ItemId, itemInfo.Text, itemInfo.EstFavori,
                            itemInfo.NbreElts));
                    }
                }

                ChargementSurFavori();

                var binding = new Binding(nameof(Width)) {Mode = BindingMode.OneWay, Source = this};
                BindingOperations.SetBinding(_tbSearch, WidthProperty, binding);

                binding = new Binding(nameof(Height)) {Mode = BindingMode.OneWay, Source = this};
                BindingOperations.SetBinding(_tbSearch, HeightProperty, binding);

                Content = _tbSearch;

                OnCurrentItemChanged();

                _tbSearch.CurrentItemChanged += TbSearchCurrentItemChanged;
            }
        }

        public virtual bool Filtrer(ISceneForm dataContext, TItem item)
        {
            return true;
        }

        private void ChargementSurFavori()
        {
            if (_sources.Any(x => x.EstFavori))
            {
                var sort = _sources.Where(x => x.EstFavori).OrderBy(x => x.Libelle).ToList();
                var source = _sources.Where(x => !x.EstFavori).OrderBy(x => x.Libelle).ToList();
                for (var i = 0; i < sort.Count; i++)
                    source.Insert(i, sort[i]);
                _tbSearch.Items = source;
            }
            else
            {
                _tbSearch.Items = _sources.OrderBy(x => x.Libelle).ToList();
            }
        }

        private void TbSearchCurrentItemChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_tbSearch.CurrentItem != null)
            {
                TItem value;
                if (_dico.TryGetValue(_tbSearch.CurrentItem.Id, out value))
                {
                    CurrentItem = value;
                    CurrentItemValue = _tbSearch.CurrentItem.Id;
                }
            }
            else
            {
                CurrentItem = default;
                CurrentItemValue = default;
            }
        }

        private void OnCurrentItemChanged()
        {
            if (_tbSearch == null) OnItemsSourceChanged();

            if (_tbSearch != null &&
                _sources != null &&
                _sources.Count > 0)
            {
                if (CurrentItem == null)
                    _tbSearch.CurrentItem = null;
                else if (_sources.Any(x => GuidHelper.IsEqual(x.Id, _itemInfoProvider(CurrentItem).ItemId)))
                    _tbSearch.CurrentItem =
                        _sources.First(x => GuidHelper.IsEqual(x.Id, _itemInfoProvider(CurrentItem).ItemId));
                _tbSearch.Text = _tbSearch.CurrentItem?.Libelle;
            }
        }

        #endregion
    }
}