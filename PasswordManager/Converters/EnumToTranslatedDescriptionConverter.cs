using PasswordManager.Extensions;
using PasswordManager.Service.Interfaces;
using System;
using System.Globalization;
using System.Windows.Data;

namespace PasswordManager.Converters
{
    /// <summary>
    /// Convert an enum value to its description attribute data, then translate it
    /// </summary>
    public class EnumToTranslatedDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var translationService = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstance(typeof(ITranslationService)) as ITranslationService;
            var targetEnum = (Enum)value;
            var descriptionCode = targetEnum.GetDescription();
            return translationService.Translate(descriptionCode);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
