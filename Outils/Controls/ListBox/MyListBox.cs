using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Outils.Helper;

namespace Outils.Controls.ListBox
{
    public class MyListBox<TItem> : UserControl
    {
        public static readonly DependencyProperty CurrentItemProperty =
            DependencyProperty.Register("CurrentItem", typeof(TItem), typeof(MyListBox<TItem>),
                new PropertyMetadata(null));

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(List<TItem>), typeof(MyListBox<TItem>),
                new PropertyMetadata(null, ItemsSource_Changed));

        private readonly Func<TItem, ItemInfo> _itemInfoProvider;
        private System.Windows.Controls.ListBox _listBox;

        private List<Item> _source;

        public MyListBox(Func<TItem, ItemInfo> provider)
        {
            _itemInfoProvider = provider;
        }

        public TItem CurrentItem
        {
            get => (TItem)GetValue(CurrentItemProperty);
            set => SetValue(CurrentItemProperty, value);
        }

        public List<TItem> ItemsSource
        {
            get => (List<TItem>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        private static void ItemsSource_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var user = d as MyListBox<TItem>;
            user.OnItemsSourceChanged();
        }

        private void OnCurrentItemChanged()
        {
            if (_listBox == null)
                OnItemsSourceChanged();

            if (_listBox != null && _source.Count > 0)
            {
                if (CurrentItem == null)
                {
                    _listBox.SelectedItem = _source.First();
                }
                else
                {
                    var c = _itemInfoProvider(CurrentItem);
                    if (_source.Any(x => GuidHelper.IsEqual(x.Id, c.ItemId)))
                        _listBox.SelectedItem = _source.First(x => GuidHelper.IsEqual(x.Id, c.ItemId));
                    else
                        _listBox.SelectedItem = _source.First();
                }
            }
        }

        private void OnItemsSourceChanged()
        {
            if (ItemsSource == null)
            {
                _listBox = null;
                Content = null;
                _source = null;
            }
            else
            {
                _source = new List<Item>();
                foreach (var item in ItemsSource)
                {
                    var t = _itemInfoProvider(item);
                    _source.Add(new Item(t.ItemId, t.Libelle));
                }

                _listBox = new System.Windows.Controls.ListBox();
                _listBox.MinWidth = 80;
                _listBox.MinHeight = 100;
                _listBox.ItemsSource = _source;
                _listBox.DisplayMemberPath = "Libelle";
                _listBox.SelectionChanged += ListBox_SelectionChanged;

                OnCurrentItemChanged();

                Content = _listBox;
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var added = e.AddedItems;
            var count = added.Count;
            if (count == 1)
                CurrentItem = ItemsSource.First(x => _itemInfoProvider(x).ItemId == ((Item)added[0]).Id);
        }

        private class Item
        {
            public Item(Guid id, string libelle)
            {
                Id = id;
                Libelle = libelle;
            }

            public Guid Id { get; }

            public string Libelle { get; }
        }

        public struct ItemInfo
        {
            public Guid ItemId;
            public string Libelle;
        }
    }
}