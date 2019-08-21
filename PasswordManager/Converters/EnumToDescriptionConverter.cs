using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
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
            return GetEnumDescription(targetEnum);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private string GetEnumDescription(Enum enumObj)
        {
            FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());
            object[] attribArray = fieldInfo.GetCustomAttributes(false);

            if (attribArray.Length == 0)
            {
                return enumObj.ToString();
            }

            foreach (var att in attribArray)
            {
                if (att is DescriptionAttribute)
                {
                    var attrib = att as DescriptionAttribute;
                    return attrib.Description;
                }
            }

            return enumObj.ToString();
        }
    }
}
