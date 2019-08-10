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

        public PasswordEntryModel Copy()
        {
            return new PasswordEntryModel()
            {
                Id = this.Id,
                Name = this.Name,
                Website = this.Website,
                Category = this.Category,
                Username = this.Username,
                Password = this.Password,
                PasswordStrength = this.PasswordStrength,
                Notes = this.Notes
            };
        }
    }
}
