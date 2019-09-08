using System;

namespace PasswordManager.Model
{
    /// <summary>
    /// Interface which all sync entries should implement
    /// </summary>
    public interface ISyncEntry
    {
        /// <summary>
        /// Unique item id
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Last time the item has been created / edited
        /// </summary>
        DateTime LastEditionDate { get; set; }
    }
}
