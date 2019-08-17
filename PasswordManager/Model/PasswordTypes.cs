using System.ComponentModel;

namespace PasswordManager.Model
{
    /// <summary>
    /// Types of password to generate
    /// </summary>
    public enum PasswordTypes
    {
        [Description("All characters")]
        Full,

        [Description("Letters and numbers")]
        AlphaAndNum,

        [Description("Letters only")]
        Alpha
    }
}
