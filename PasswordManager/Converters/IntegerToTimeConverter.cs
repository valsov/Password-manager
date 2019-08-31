﻿using PasswordManager.Service.Interfaces;
using System;
using System.Globalization;
using System.Windows.Data;

namespace PasswordManager.Converters
{
    /// <summary>
    /// Add "seconds" after the given integer, if the integer is 0, return infinite
    /// </summary>
    public class IntegerToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var translationService = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstance(typeof(ITranslationService)) as ITranslationService;
            var data = (int)value;
            if(data == 0)
            {
                return translationService.Translate("Infinite");
            }

            return $"{data} {translationService.Translate("Seconds")}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
