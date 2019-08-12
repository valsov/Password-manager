using PasswordManager.Model;

namespace PasswordManager.Extensions
{
    public static class PasswordEntryModelExtension
    {
        public static PasswordEntryModel Copy(this PasswordEntryModel entry)
        {
            return new PasswordEntryModel()
            {
                Id = entry.Id,
                Name = entry.Name,
                Website = entry.Website,
                Category = entry.Category,
                Username = entry.Username,
                Password = entry.Password,
                PasswordStrength = entry.PasswordStrength,
                Notes = entry.Notes
            };
        }
    }
}
