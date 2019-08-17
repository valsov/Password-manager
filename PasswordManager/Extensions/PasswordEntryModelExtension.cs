using PasswordManager.Model;
using System.Timers;
using System.Windows;

namespace PasswordManager.Extensions
{
    /// <summary>
    /// Extension methods for the PasswordEntryModel class
    /// </summary>
    public static class PasswordEntryModelExtension
    {
        const int clipboardTimerDuration = 7000;

        private static Timer clipboardTimer;

        /// <summary>
        /// Constructor
        /// </summary>
        static PasswordEntryModelExtension()
        {
            clipboardTimer = new Timer(clipboardTimerDuration)
            {
                AutoReset = false,
                Enabled = false
            };
            clipboardTimer.Elapsed += ClipboardTimerElapsed;
        }

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
        /// Copy to the clipboard the given property of a PasswordEntryModel
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="property"></param>
        public static void CopyDataToClipboard(this PasswordEntryModel entry, string property)
        {
            clipboardTimer.Stop();

            var data = string.Empty;
            switch (property)
            {
                case "Password":
                    data = entry.Password;
                    break;
                case "Username":
                    data = entry.Username;
                    break;
                case "Website":
                    data = entry.Website;
                    break;
            }

            Clipboard.SetText(data);
            clipboardTimer.Start();
        }

        /// <summary>
        /// Method called when clipboardTimer raises the Elapsed event, reset the clipboard content
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ClipboardTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => Clipboard.SetText(string.Empty));
        }
    }
}
