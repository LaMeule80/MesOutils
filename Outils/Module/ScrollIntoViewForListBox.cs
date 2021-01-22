using System;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Outils.Module
{
    public class ScrollIntoViewForListBox : Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached(); 
            AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
        }

        void AssociatedObject_SelectionChanged(object sender,
            SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox?.SelectedItem != null)
            {
                listBox.Dispatcher.BeginInvoke(
                    (Action)(() =>
                    {
                        listBox.UpdateLayout();
                        if (listBox.SelectedItem !=
                            null)
                            listBox.ScrollIntoView(
                                listBox.SelectedItem);
                    }));
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectionChanged -=
                AssociatedObject_SelectionChanged;

        }
    }
}
