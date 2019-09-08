using System;
using System.Globalization;
using System.Windows.Data;

namespace PasswordManager.Converters
{
    /// <summary>
    /// Convert a boolean to its opposite
    /// </summary>
    public class InvertBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
