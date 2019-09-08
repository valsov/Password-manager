using PasswordManager.Model;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PasswordManager.Converters
{
    /// <summary>
    /// Convert a SyncStepState to a visibility, hidden if it is inactive
    /// </summary>
    public class SyncStepStateToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = (SyncStepStates)value;
            return state == SyncStepStates.Inactive ? Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
