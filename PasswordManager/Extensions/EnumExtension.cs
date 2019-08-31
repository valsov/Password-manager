using System;
using System.ComponentModel;
using System.Reflection;

namespace PasswordManager.Extensions
{
    /// <summary>
    /// Extension class for the Enum type
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// Get the Description attribute of the given Enum value
        /// </summary>
        /// <param name="enumObj"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum enumObj)
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
