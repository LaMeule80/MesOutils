using System;
using System.Windows;
using System.Windows.Controls;
using Outils.Command;

namespace Outils.Controls.Button
{
    public class ButtonCommand<TItem> : UserControl
    {
        public static readonly DependencyProperty CurrentItemProperty =
            DependencyProperty.Register("CurrentItem", typeof(TItem), typeof(ButtonCommand<TItem>),
                new PropertyMetadata(null));

        private readonly System.Windows.Controls.Button _button;
        private readonly Predicate<TItem> _canExecute;
        private readonly Action<TItem> _execute;

        public event EventHandler<ButtonCommandEventArgs> FinDeLaCommande;
        public void OnFinDeLaCommande()
        {
            if(FinDeLaCommande != null)
                FinDeLaCommande.Invoke(null, new ButtonCommandEventArgs());
        }

        public ButtonCommand(string libelle, Action<TItem> execute, Predicate<TItem> canExecute = null)
        {
            _canExecute = canExecute;
            _execute = execute;

            _button = new System.Windows.Controls.Button
            {
                Content = libelle, 
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                MinHeight = 25,
                MinWidth = 100,
                MaxWidth = 120,
                MaxHeight = 50
            };
            _button.Margin = new Thickness(5);

            _button.Loaded += Button_Loaded;
            Content = _button;
        }

        public TItem CurrentItem
        {
            get => (TItem)GetValue(CurrentItemProperty);
            set => SetValue(CurrentItemProperty, value);
        }

        private void Button_Loaded(object sender, RoutedEventArgs e)
        {
            _button.Command = new RelayCommand<TItem>(Execute, CanExecute);
            _button.CommandParameter = CurrentItem;
        }

        private void Execute(TItem item)
        {
            _execute(CurrentItem);
            OnFinDeLaCommande();
        }

        private bool CanExecute(TItem item)
        {
            return _canExecute == null || _canExecute(CurrentItem);
        }
    }
}
