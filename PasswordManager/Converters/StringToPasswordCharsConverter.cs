using System;
using System.Globalization;
using System.Windows.Data;

namespace PasswordManager.Converters
{
    public class StringToPasswordCharsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var data = value as string;
            if (data is null || data.Length == 0)
            {
                return string.Empty;
            }

            return new string('•', 20);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
