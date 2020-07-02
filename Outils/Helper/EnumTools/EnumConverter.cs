using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Outils.Helper.EnumTools
{
    public class EnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(parameter is string parameterString))
                return DependencyProperty.UnsetValue;

            if (value == null)
                return DependencyProperty.UnsetValue;

            if (!Enum.IsDefined(value.GetType(), value))
                return DependencyProperty.UnsetValue;

            var parameterValue = Enum.Parse(value.GetType(), parameterString);

            return parameterValue.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(parameter is string parameterString)
                ? DependencyProperty.UnsetValue
                : Enum.Parse(targetType, parameterString);
        }
    }
}