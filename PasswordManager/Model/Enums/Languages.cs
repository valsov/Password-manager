using System.ComponentModel;

namespace PasswordManager.Model
{
    /// <summary>
    /// Available languages for the application
    /// </summary>
    public enum Languages
    {
        [Description("../Resources/uk_flag.png")]
        English,

        [Description("../Resources/france_flag.png")]
        French
    }
}
