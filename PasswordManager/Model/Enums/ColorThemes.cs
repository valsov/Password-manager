using System.ComponentModel;

namespace PasswordManager.Model
{
    /// <summary>
    /// Available color themes for the application
    /// </summary>
    public enum ColorThemes
    {
        [Description("#0000ff")]
        Blue,

        [Description("#00ff00")]
        Green,

        [Description("#ff0000")]
        Red,

        [Description("#551A8B")]
        Purple,

        [Description("#00ffff")]
        Cyan
    }
}
