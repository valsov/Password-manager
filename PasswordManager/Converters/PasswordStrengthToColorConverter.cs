using PasswordManager.Model;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PasswordManager.Converters
{
    public class PasswordStrengthToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var strength = (PasswordStrength)value;
            var color = new SolidColorBrush(Colors.White);
            switch (strength)
            {
                case PasswordStrength.VeryWeak:
                    color.Color = Colors.Red;
                    break;
                case PasswordStrength.Weak:
                    color.Color = Colors.Orange;
                    break;
                case PasswordStrength.Medium:
                    color.Color = Colors.Gold;
                    break;
                case PasswordStrength.Strong:
                    color.Color = Colors.GreenYellow;
                    break;
                case PasswordStrength.VeryStrong:
                    color.Color = Colors.DeepSkyBlue;
                    break;
            }

            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
