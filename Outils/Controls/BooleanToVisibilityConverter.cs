using System;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Outils.Controls
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        ///     Instance par défaut
        /// </summary>
        public static readonly BooleanToVisibilityConverter Default = new BooleanToVisibilityConverter();

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool v;

            if (value == null)
                v = true;
            else if (value is bool)
                v = (bool)value;
            else
                v = Convert.ToBoolean(value);

            if (parameter is string p && p == "!")
                v = !v;
            return v ? Visibility.Visible : Visibility.Collapsed;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new ReadOnlyException();
        }
    }
}
