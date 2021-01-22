using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Outils.Controls
{
    public static class NavigationItemExtension
    {
        public static string GetName(this IBaseId item)
        {
            return NavigationItemHelper.GetName(item.Nom);
        }
    }

    public static class NavigationItemHelper
    {
        public static string GetName(string name)
        {
            return name.Replace(" ", "");
        }
    }

    public abstract class NavigationManager<TItem>
        where TItem : IBaseId
    {
        private const double NodePanelDefaultHeight = 26;

        private const string Name = "Conteneur";

        private List<Tuple<object, List<TItem>>> _keys;
        private List<TItem> _items;
        public TItem Item { get; private set; }

        private Control _currentLabelOrMenuItem;

        public event EventHandler ChangementItem;

        public void OnChangementItem() => ChangementItem?.Invoke(null, new EventArgs());

        public abstract List<Tuple<object, List<TItem>>> GetItems();

        public void BuildNavigationMainPanel(UIElementCollection uiElementCollection)
        {
            _keys = GetItems();
            _items = _keys.SelectMany(x => x.Item2).ToList();

            var navMainPanel = new StackPanel { Name = Name };

            foreach (var statut in _keys)
                AddNodePanel(navMainPanel, statut);

            foreach (var nodePanel in navMainPanel.Children.OfType<StackPanel>())
            {
                nodePanel.PreviewMouseLeftButtonDown += (s, e) =>
                {
                    ((nodePanel.Triggers[0] as EventTrigger)
                            ?.Actions[0] as BeginStoryboard)
                        ?.Storyboard
                        ?.Begin();
                };

                var storyboard = BuildStoryboard(navMainPanel, nodePanel);
                var eventTrigger = new EventTrigger { RoutedEvent = UIElement.MouseLeftButtonDownEvent };
                eventTrigger.Actions.Add(new BeginStoryboard { Storyboard = storyboard });
                nodePanel.Triggers.Add(eventTrigger);
            }

            uiElementCollection.Add(navMainPanel);
        }

        private void AddNodePanel(Panel navMainPanel, Tuple<object, List<TItem>> key)
        {
            if (!key.Item2.Any())
                return;
            var panel = BuildNodePanel(key);
            navMainPanel.Children.Add(panel);
        }

        private StackPanel BuildNodePanel(Tuple<object, List<TItem>> key)
        {
            var nodePanel = new StackPanel
            {
                Name = key.Item1.ToString(),
                Height = NodePanelDefaultHeight
            };
            var headerButton = BuildNodeHeader(key);
            nodePanel.Children.Add(headerButton);

            foreach (var nodeAction in key.Item2)
                BuildNodeLabel(nodePanel, nodeAction);

            return nodePanel;
        }

        private System.Windows.Controls.Button BuildNodeHeader(Tuple<object, List<TItem>> key)
        {
            var button = new System.Windows.Controls.Button
            {
                Content = $"{key.Item1} ({key.Item2.Count})",
                HorizontalContentAlignment = HorizontalAlignment.Left,
                Background = Brushes.LightCyan
            };
            return button;
        }

        public virtual Style GetStyle(TItem item)
        {
            return null;
        }

        private void BuildNodeLabel(Panel nodePanel, TItem item)
        {
            var style = GetStyle(item);
            var label = new Label
            {
                Name = item.GetName(),
                Content = item.Nom,
                Style = style
            };
            label.MouseLeftButtonDown += NodeItemLabelMouseLeftButtonDown;
            nodePanel.Children.Add(label);
        }

        private Storyboard BuildStoryboard(Panel navMainPanel, StackPanel selectedNodePanel)
        {
            var storyboard = new Storyboard();
            foreach (var nodePanel in navMainPanel.Children.OfType<StackPanel>())
            {
                var panelHeight = nodePanel == selectedNodePanel
                    ? nodePanel.Children.Count * NodePanelDefaultHeight
                    : NodePanelDefaultHeight;
                var doubleAnimation = new DoubleAnimation(panelHeight + 5, new Duration(TimeSpan.FromMilliseconds(500)))
                {
                    AccelerationRatio = 0.6,
                    DecelerationRatio = 0.4
                };

                Storyboard.SetTarget(doubleAnimation, nodePanel);
                Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(FrameworkElement.HeightProperty));
                storyboard.Children.Add(doubleAnimation);
            }

            return storyboard;
        }

        public abstract object GetParent(TItem item);

        public void SelectNodeHeaderAndItem(Grid grid, TItem item)
        {
            var items = grid.Children.OfType<StackPanel>().First(x => x.Name == Name);

            Item = item;
            var nodePanel = items.Children.OfType<StackPanel>().FirstOrDefault(x => x.Name == GetParent(item).ToString());
            if (nodePanel == null)
                return;

            var storyboard = ((nodePanel.Triggers[0] as EventTrigger)?.Actions[0] as BeginStoryboard)?.Storyboard;
            if (storyboard == null)
                return;

            storyboard.Begin();
            ClickNodeItem(nodePanel, item);
        }

        private void ClickNodeItem(Panel nodePanel, TItem item)
        {
            var nodeItemLabel = nodePanel.Children.OfType<Label>().FirstOrDefault(x => NavigationItemHelper.GetName(x.Name) == item.GetName());

            if (nodeItemLabel == null)
                return;

            nodeItemLabel.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice,
                new TimeSpan(DateTime.Now.Ticks).Milliseconds, MouseButton.Left)
            {
                RoutedEvent = UIElement.MouseLeftButtonDownEvent,
                Source = nodeItemLabel
            });
            OnChangementItem();
        }

        private void NodeItemLabelMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LabelOrMenuItemClick((Label)sender);
        }

        private void LabelOrMenuItemClick(Control selectedItem)
        {
            if (selectedItem == _currentLabelOrMenuItem)
                return;
            Item = _items.First(x => x.GetName() == selectedItem.Name);
            OnChangementItem();
            HandleFontDisplay(selectedItem);
            _currentLabelOrMenuItem = selectedItem;
        }

        private void HandleFontDisplay(Control selectedItem)
        {
            if (_currentLabelOrMenuItem != null)
            {
                if (_currentLabelOrMenuItem is Label)
                {
                    _currentLabelOrMenuItem.FontWeight = FontWeights.Normal;
                }
                else if (_currentLabelOrMenuItem.Parent is MenuItem menuItem)
                {
                    menuItem.FontWeight = FontWeights.Normal;
                    foreach (var item in menuItem.Items)
                        ((MenuItem)item).FontWeight = FontWeights.Normal;
                }
            }

            if (selectedItem is Label)
            {
                selectedItem.FontWeight = FontWeights.Bold;}
            else if (selectedItem.Parent is MenuItem menuItem)
            {
                menuItem.FontWeight = FontWeights.Bold;

                foreach (var item in menuItem.Items)
                    ((MenuItem)item).FontWeight = Equals(item, selectedItem) ?
                        FontWeights.Bold :
                        FontWeights.Normal;
            }
        }
    }
}