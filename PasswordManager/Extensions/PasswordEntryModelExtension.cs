using PasswordManager.Model;
using System;
using System.Timers;
using System.Windows;

namespace PasswordManager.Extensions
{
    /// <summary>
    /// Extension methods for the PasswordEntryModel class
    /// </summary>
    public static class PasswordEntryModelExtension
    {
        private static Timer clipboardTimer;

        /// <summary>
        /// Copy data started event
        /// </summary>
        public static event EventHandler CopyDataStart;

        /// <summary>
        /// Copy data ended event
        /// </summary>
        public static event EventHandler CopyDataEnd;

        /// <summary>
        /// Constructor
        /// </summary>
        static PasswordEntryModelExtension()
        {
            clipboardTimer = new Timer(Constants.ClipboardTimerDuration)
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

            if (data is null)
                return;

            clipboardTimer.Stop();
            Clipboard.SetText(data);
            CopyDataStart?.Invoke(null, null);
            clipboardTimer.Start();
        }

        /// <summary>
        /// Method called when clipboardTimer raises the Elapsed event, reset the clipboard content
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ClipboardTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => CopyDataEnd.Invoke(null, null));
            Application.Current.Dispatcher.Invoke(() => Clipboard.SetText(string.Empty));
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
