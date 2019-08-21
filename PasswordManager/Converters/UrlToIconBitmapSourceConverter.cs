using CommonServiceLocator;
using PasswordManager.Service.Interfaces;
using System;
using System.Globalization;
using System.Windows.Data;

namespace PasswordManager.Converters
{
    /// <summary>
    /// Convert an url to a bitmap ressource loaded in the IIconsRepository
    /// </summary>
    public class UrlToIconBitmapSourceConverter : IValueConverter
    {
        private IIconsService iconsService;

        public UrlToIconBitmapSourceConverter()
        {
            iconsService = ServiceLocator.Current.GetInstance<IIconsService>();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var url = value as string;
            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }

            return iconsService.GetIcon(url);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
