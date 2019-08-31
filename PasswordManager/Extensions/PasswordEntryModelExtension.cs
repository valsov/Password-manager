using PasswordManager.Model;

namespace PasswordManager.Extensions
{
    /// <summary>
    /// Extension methods for the PasswordEntryModel class
    /// </summary>
    public static class PasswordEntryModelExtension
    {
        /// <summary>
        /// Create a copy of a PasswordEntryModel
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Open the Website property of the given PasswordEntryModel in the default browser
        /// </summary>
        /// <param name="entry"></param>
        public static void OpenWebsite(this PasswordEntryModel entry)
        {
            var url = entry.Website;
            if (string.IsNullOrWhiteSpace(url))
            {
                return;
            }

            // Need to add the protocol to be considered as a website url
            if (!url.StartsWith("https://") || !url.StartsWith("http://"))
            {
                url = $"https://{url}";
            }

            System.Diagnostics.Process.Start(url);
        }
    }
}
