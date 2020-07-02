using System;
using System.Windows;

namespace Outils
{
    public static class AfficherWin
    {
        public static void Show(System.Type type, IService service)
        {
            var w = (Window)Activator.CreateInstance(type, service);
            w.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            w.Owner = Application.Current.MainWindow;
            w.Show();
        }

        public static void ShowDialog(System.Type type, IService service)
        {
            var w = (Window)Activator.CreateInstance(type, service);
            w.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            w.Owner = Application.Current.MainWindow;
            w.ShowDialog();
        }
    }
}