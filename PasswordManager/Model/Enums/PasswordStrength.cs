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

        [Description("PasswordStrengthVeryWeak")]
        VeryWeak = 1,

        [Description("PasswordStrengthWeak")]
        Weak = 2,

        [Description("PasswordStrengthMedium")]
        Medium = 3,

        [Description("PasswordStrengthStrong")]
        Strong = 4,

        [Description("PasswordStrengthVeryStrong")]
        VeryStrong = 5
    }
}
