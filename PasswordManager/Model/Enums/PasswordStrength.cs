using System.ComponentModel;

namespace PasswordManager.Model
{
    /// <summary>
    /// Enumeration describing the different password security levels
    /// </summary>
    public enum PasswordStrength
    {
        [Description("")]
        Blank = 0,

        [Description("Very weak")]
        VeryWeak = 1,

        [Description("Weak")]
        Weak = 2,

        [Description("Medium")]
        Medium = 3,

        [Description("Strong")]
        Strong = 4,

        [Description("Very strong")]
        VeryStrong = 5
    }
}
