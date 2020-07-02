using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Telerik.Windows.Controls;
using Telerik.Windows.Diagrams.Core;

namespace Outils.Controls.ComboBox
{
    public partial class TbSearch : UserControl
    {
        public TbSearch()
        {
            InitializeComponent();

            _keypressTimer = new Timer();
            _keypressTimer.Elapsed += OnTimedEvent;

            Loaded += tbSearch_Loaded;

            cross.Visibility = Visibility.Collapsed;
            SetCursor(cross);
            cross.MouseLeftButtonDown += (s, e) =>
            {
                textBox.Text = null;
                cross.Visibility = Visibility.Collapsed;
                Focus();
            };
            cross.ToolTip = "Effacer";
            cross.Background = new SolidColorBrush(Colors.White);
            textBox.Padding = new Thickness(3, 2, 18, 2);
        }

        public void Dispose()
        {
            _keypressTimer?.Dispose();
        }

        private delegate void TextChangedCallback();

        #region Variables

        private List<SearchItem> _items = new List<SearchItem>();
        private ILookup<string, SearchItem> _indexLibelleAffichage;

        private readonly Timer _keypressTimer;
        private bool _dontSelect;
        private bool _losingFocus;

        #endregion

        #region Propriétés

        public List<SearchItem> Items
        {
            get => (List<SearchItem>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
            nameof(Items), typeof(List<SearchItem>), typeof(TbSearch),
            new PropertyMetadata(null, HandleItemsChanged));

        public static DependencyProperty DernièreRechercheProperty =
            DependencyProperty.Register(
                nameof(DernièreRecherche),
                typeof(string),
                typeof(TbSearch),
                new PropertyMetadata(null, HandleProperyChanged));

        private static void HandleProperyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == DernièreRechercheProperty)
                (obj as TbSearch)?.OnPropertyChanged(nameof(DernièreRecherche));
        }

        public void OnPropertyChanged(string str)
        {
            var h = CurrentTextChanged;
            h?.Invoke(this, new PropertyChangedEventArgs(nameof(DernièreRecherche))); //MLHIDE
        }

        public string DernièreRecherche
        {
            get => (string)GetValue(DernièreRechercheProperty);
            set => SetValue(DernièreRechercheProperty, value);
        }

        public string Text
        {
            get => textBox.Text;
            set
            {
                InsertText = true;
                textBox.Text = value;
                DernièreRecherche = null;
            }
        }

        private bool _insertText;

        public bool InsertText
        {
            get => _insertText;
            private set
            {
                _insertText = value;
                UpdateCross();
            }
        }

        public event PropertyChangedEventHandler CurrentTextChanged;

        public event PropertyChangedEventHandler CurrentItemChanged;

        private SearchItem _currentItem;

        public SearchItem CurrentItem
        {
            get => _currentItem;
            set
            {
                _currentItem = value;
                OnCurrentItemChanged();
                UpdateCross();
            }
        }

        protected void OnCurrentItemChanged()
        {
            var h = CurrentItemChanged;
            h?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentItem))); //MLHIDE
        }

        #endregion

        #region Méthodes

        private void SetCursor(FrameworkElement elt)
        {
            elt.MouseEnter += (s, e) => { Cursor = Cursors.Hand; };

            elt.MouseLeave += (s, e) => { Cursor = Cursors.Arrow; };
        }

        private void UpdateCross()
        {
            cross.Visibility = InsertText && Text.IsNotEmpty() || CurrentItem != null
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }

        private void FillCombo()
        {
            comboBox.Items.Clear();
            _items.ForEach(p => comboBox.Items.Add(p));
        }

        public string RemoveDiacritics(string input)
        {
            var stFormD = input.Normalize(NormalizationForm.FormD);
            var len = stFormD.Length;
            var sb = new StringBuilder();
            for (var i = 0; i < len; i++)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[i]);
                if (uc != UnicodeCategory.NonSpacingMark)
                    sb.Append(stFormD[i]);
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        private void SortItems(List<SearchItem> rsource)
        {
            var source = rsource.ToList<SearchItem>();
            var sorted = new List<SearchItem>();

            var ordre = source.Clone();
            foreach (var i in ordre)
            {
                sorted.Add(i);
                source.Remove(i);
            }

            for (var i = 0; i < source.Count; i++)
            {
                sorted.Add(source[i]);
            }

            _items = sorted;
            _indexLibelleAffichage = _items.ToLookup(i => i.Libelle);
        }

        private void SetItems(List<SearchItem> values)
        {
            SortItems(values);
        }

        public new void Focus()
        {
            textBox.Focus();
        }

        #endregion

        #region Evenements

        private void tbSearch_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= tbSearch_Loaded;
            textBox.ToolTip = null;
        }

        private static void HandleItemsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var tb = obj as TbSearch;
            tb?.HandleItemsChanged();
        }

        private void HandleItemsChanged()
        {
            if (Items.Count > 0)
                SetItems(Items);
        }

        private void TextChanged()
        {
            comboBox.Items.Clear();
            foreach (var entry in _items)
            {
                var termes = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("libelle", entry.Libelle) //MLHIDE
                };
                foreach (var word in termes)
                    if (word.Value != null)
                    {
                        var wordSansDiacritics = RemoveDiacritics(word.Value.ToLower());
                        var TextSansDiacritics = RemoveDiacritics(textBox.Text.ToLower());

                        if (wordSansDiacritics.Contains(TextSansDiacritics) ||
                            wordSansDiacritics.StartsWith(TextSansDiacritics))
                        {
                            comboBox.Items.Add(entry);
                            break;
                        }
                    }
            }

            comboBox.IsDropDownOpen = comboBox.HasItems;

            DernièreRecherche = textBox.Text;
        }

        private new void LostFocus()
        {
            if (_losingFocus)
            {
                _losingFocus = false;
                return;
            }

            IEnumerable<SearchItem> matches = null;

            if (_indexLibelleAffichage != null)
                matches = _indexLibelleAffichage[textBox.Text];

            if (matches.IsNotEmpty())
            {
                CurrentItem = matches.FirstOrDefault();

                SortItems(_items);

                Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)delegate ()
                {
                    _losingFocus = true;
                    textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                });
            }

            var elementWithFocus = Keyboard.FocusedElement as UIElement;

            // 121227 - Elias - NullReferenceException sporadique
            // Pour mémoire, le test initial était le suivant:
            //if (!(elementWithFocus != null && elementWithFocus.GetType().Name == "RadComboBoxItem") && elementWithFocus != this && elementWithFocus.GetType().Name != "RepeatButton")
            // Pas certain de comprendre ce que ça fait, mais je m'efforce de simplifier en décomposant comme suit:
            if (elementWithFocus == this || elementWithFocus == comboBox || elementWithFocus == textBox) return;
            if (elementWithFocus is RadComboBoxItem) return;
            if (elementWithFocus is RepeatButton) return;

            comboBox.IsDropDownOpen = false;

            if (_currentItem == null)
                textBox.Text = string.Empty;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            _keypressTimer.Stop();
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                new TextChangedCallback(TextChanged));
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (null != comboBox.SelectedItem && (!comboBox.IsDropDownOpen || !_dontSelect))
            {
                InsertText = true;
                var cbItem = (SearchItem)comboBox.SelectedItem;
                textBox.Text = cbItem.Libelle;
            }

            LostFocus();
            _dontSelect = false;
        }

        private void comboBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter)
                comboBox_SelectionChanged(sender, null);
            else if (e.Key >= Key.A && e.Key <= Key.Z || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 ||
                     e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                var key = e.Key.ToString().ToLower();
                if (key.Count() > 1)
                    key = key[key.Count() - 1].ToString();
                textBox.Text += key;
                _dontSelect = true;
                textBox.Focus();
            }
        }

        private void textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.ToolTip = null;
                CurrentItem = null;
            }

            if (textBox.Text == string.Empty && textBox.IsFocused)
            {
                FillCombo();

                if (comboBox.Items.Count > 0)
                    Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle,
                        (Action)delegate () { comboBox.IsDropDownOpen = comboBox.HasItems; });
                else
                    Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                        (Action)delegate () { textBox.SelectAll(); });
            }
            else if (textBox.Text != string.Empty)
            {
                if (InsertText == true)
                {
                    InsertText = false;
                }
                else
                {
                    TextChanged();
                }
            }
        }

        private void textbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                if (comboBox.Items.Count == 0)
                    FillCombo();
                _dontSelect = true;
                comboBox.IsDropDownOpen = true;
                comboBox.SelectedIndex = 0;
                comboBox.Focus();
            }
        }

        private void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (textBox.Text == string.Empty && textBox.IsFocused)
            {
                FillCombo();

                if (comboBox.Items.Count > 0)
                    if (Mouse.LeftButton == MouseButtonState.Pressed)
                        Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                            (Action)delegate () { textBox_GotFocus(sender, e); });
                    else
                        Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                            (Action)delegate () { comboBox.IsDropDownOpen = comboBox.HasItems; });
                else
                    Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                        (Action)delegate () { textBox.SelectAll(); });
            }
            else if (!_dontSelect)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                    (Action)delegate () { textBox.SelectAll(); });
            }
            else
            {
                textBox.CaretIndex = textBox.Text.Count();
                _dontSelect = false;
            }
        }

        private void comboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!textBox.IsFocused && !_dontSelect)
                LostFocus();
        }

        private void textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!comboBox.IsFocused && !_dontSelect)
                LostFocus();
        }

        private void textBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (textBox.Text == string.Empty)
            {
                FillCombo();
                if (comboBox.Items.Count > 0)
                    Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                        (Action)delegate () { comboBox.IsDropDownOpen = comboBox.HasItems; });
                else if (textBox.Text != string.Empty)
                    Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                        (Action)delegate () { textBox.SelectAll(); });
            }
        }

        private void textBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (textBox.Text == string.Empty && e.Key == Key.Back)
                e.Handled = true;
        }

        #endregion
    }
}
