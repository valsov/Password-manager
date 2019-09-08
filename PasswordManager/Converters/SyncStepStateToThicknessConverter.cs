using PasswordManager.Model;
using System;
using System.Globalization;
using System.Windows.Data;

namespace PasswordManager.Converters
{
    /// <summary>
    /// Convert a SyncStepState to a thickness, 4 while in progress and 2 for the other states
    /// </summary>
    public class SyncStepStateToThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = (SyncStepStates)value;
            return state == SyncStepStates.InProgress ? 4 : 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
