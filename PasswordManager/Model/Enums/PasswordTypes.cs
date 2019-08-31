using System.ComponentModel;

namespace PasswordManager.Model
{
    /// <summary>
    /// Types of password to generate
    /// </summary>
    public enum PasswordTypes
    {
        [Description("PasswordTypeAll")]
        Full,

        [Description("PasswordTypeLettersNumbers")]
        AlphaAndNum,

        [Description("PasswordTypeLetters")]
        Alpha
    }
}
