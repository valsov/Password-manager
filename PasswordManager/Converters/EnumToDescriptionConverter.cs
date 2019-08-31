using PasswordManager.Extensions;
using System;
using System.Globalization;
using System.Windows.Data;

namespace PasswordManager.Converters
{
    /// <summary>
    /// Convert an enum value to its description attribute data
    /// </summary>
    public class EnumToDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var targetEnum = (Enum)value;
            return targetEnum.GetDescription();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
