using PasswordManager.Model;
using System.Timers;
using System.Windows;

namespace PasswordManager.Extensions
{
    public static class PasswordEntryModelExtension
    {
        private static Timer clipboardTimer;

        static PasswordEntryModelExtension()
        {
            clipboardTimer = new Timer(7000)
            {
                AutoReset = false,
                Enabled = false
            };
            clipboardTimer.Elapsed += ClipboardTimerElapsed;
        }

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

        private static void ClipboardTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => Clipboard.SetText(string.Empty));
        }
    }
}
