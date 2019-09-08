using PasswordManager.Model;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PasswordManager.Converters
{
    /// <summary>
    /// Convert a SyncStepState to its corresponding ColorBrush
    /// </summary>
    public class SyncStepStateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = (SyncStepStates)value;
            var brush = new SolidColorBrush();

            switch (state)
            {
                case SyncStepStates.Inactive:
                    brush.Color = Colors.White;
                    break;
                case SyncStepStates.InProgress:
                    brush.Color = Color.FromRgb(18, 151, 224);
                    break;
                case SyncStepStates.Done:
                    brush.Color = Color.FromRgb(46, 204, 113);
                    break;
                case SyncStepStates.Skipped:
                    brush.Color = Color.FromRgb(149, 165, 166);
                    break;
                case SyncStepStates.Failed:
                    brush.Color = Color.FromRgb(215, 60, 44);
                    break;
            }

            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
