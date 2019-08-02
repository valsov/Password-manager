namespace PasswordManager.Model
{
    public class PasswordEntryModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Description { get; set; }

        public PasswordEntryModel Copy()
        {
            return new PasswordEntryModel()
            {
                Id = this.Id,
                Name = this.Name,
                Category = this.Category,
                Username = this.Username,
                Password = this.Password,
                Description = this.Description
            };
        }
    }
}
