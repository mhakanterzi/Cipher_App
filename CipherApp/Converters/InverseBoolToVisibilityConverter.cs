using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CipherApp
{
    /// <summary>
    /// Inverts boolean values to map true->Collapsed and false->Visible for WPF bindings.
    /// </summary>
    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = value is bool v && v;
            return b ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility v) return v != Visibility.Visible;
            return true;
        }
    }
}

