using PasswordManager.Model;
using System;
using System.Globalization;
using System.Windows.Data;

namespace PasswordManager.Converters
{
    /// <summary>
    /// Convert a password strength to its label
    /// </summary>
    public class PasswordStrengthToLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var strength = (PasswordStrength)value;
            var label = string.Empty;
            switch (strength)
            {
                case PasswordStrength.VeryWeak:
                    label = "Very Weak";
                    break;
                case PasswordStrength.Weak:
                    label = "Weak";
                    break;
                case PasswordStrength.Medium:
                    label = "Medium";
                    break;
                case PasswordStrength.Strong:
                    label = "Strong";
                    break;
                case PasswordStrength.VeryStrong:
                    label = "Very Strong";
                    break;
            }

            return label;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
