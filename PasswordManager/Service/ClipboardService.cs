using PasswordManager.Model;
using PasswordManager.Service.Interfaces;
using System;
using System.Timers;
using System.Windows;

namespace PasswordManager.Service
{
    /// <summary>
    /// Service handling the clipboard manipulation
    /// </summary>
    public class ClipboardService : IClipboardService
    {
        ISettingsService settingsService;

        private Timer clipboardTimer;

        /// <summary>
        /// Copy data started event
        /// </summary>
        public event EventHandler CopyDataStart;

        /// <summary>
        /// Copy data ended event
        /// </summary>
        public event EventHandler CopyDataEnd;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settingsService"></param>
        public ClipboardService(ISettingsService settingsService)
        {
            this.settingsService = settingsService;
        }

        /// <summary>
        /// Copy to the clipboard the given property of a PasswordEntryModel
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="property"></param>
        public void CopyDataToClipboard(PasswordEntryModel entry, string property)
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

            clipboardTimer?.Stop();

            var duration = settingsService.GetClipboardTimerDuration();
            if (duration != 0)
            {
                clipboardTimer = new Timer(settingsService.GetClipboardTimerDuration() * 1000)
                {
                    AutoReset = false,
                    Enabled = false
                };
                clipboardTimer.Elapsed += ClipboardTimerElapsed;

                CopyDataStart?.Invoke(null, null);
                clipboardTimer.Start();
            }

            Clipboard.SetText(data);
        }

        /// <summary>
        /// Method called when clipboardTimer raises the Elapsed event, reset the clipboard content
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClipboardTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => CopyDataEnd.Invoke(null, null));
            Application.Current.Dispatcher.Invoke(() => Clipboard.SetText(string.Empty));
        }
    }
}
