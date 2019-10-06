using PasswordManager.Model;
using System;
using System.Globalization;
using System.Windows;
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
                    brush = (SolidColorBrush)Application.Current.Resources["SyncStepStateInactiveColor"];
                    break;
                case SyncStepStates.InProgress:
                    brush = (SolidColorBrush)Application.Current.Resources["SyncStepStateInProgressColor"];
                    break;
                case SyncStepStates.Done:
                    brush = (SolidColorBrush)Application.Current.Resources["SyncStepStateDoneColor"];
                    break;
                case SyncStepStates.Skipped:
                    brush = (SolidColorBrush)Application.Current.Resources["SyncStepStateSkippedColor"];
                    break;
                case SyncStepStates.Failed:
                    brush = (SolidColorBrush)Application.Current.Resources["SyncStepStateFailedColor"];
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
