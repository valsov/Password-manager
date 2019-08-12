namespace PasswordManager.Model
{
    public class PasswordEntryModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Website { get; set; }

        public string Category { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public PasswordStrength PasswordStrength { get; set; }

        public string Notes { get; set; }
    }
}
