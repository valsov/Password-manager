using PasswordManager.Model;
using System;

namespace PasswordManager.Service.Interfaces
{
    /// <summary>
    /// Service handling the clipboard manipulation
    /// </summary>
    public interface IClipboardService
    {
        /// <summary>
        /// Copy data started event
        /// </summary>
        event EventHandler CopyDataStart;

        /// <summary>
        /// Copy data ended event
        /// </summary>
        event EventHandler CopyDataEnd;

        /// <summary>
        /// Copy to the clipboard the given property of a PasswordEntryModel
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="property"></param>
        void CopyDataToClipboard(PasswordEntryModel entry, string property);
    }
}
