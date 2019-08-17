namespace PasswordManager.Model
{
    /// <summary>
    /// Class to identify a password with its details
    /// </summary>
    public class PasswordEntryModel
    {
        /// <summary>
        /// Unique id (GUID)
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Name of the password entry
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Website the password is used on
        /// </summary>
        public string Website { get; set; }

        /// <summary>
        /// Category of the password entry
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Corresponding password Strength
        /// </summary>
        public PasswordStrength PasswordStrength { get; set; }

        /// <summary>
        /// Notes of the password entry
        /// </summary>
        public string Notes { get; set; }
    }
}
