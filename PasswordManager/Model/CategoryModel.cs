using System;

namespace PasswordManager.Model
{
    /// <summary>
    /// Class to identify a category
    /// </summary>
    public class CategoryModel : ISyncEntry
    {
        /// <summary>
        /// Unique id (GUID)
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Name of the category
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Last time the category has been created / edited
        /// </summary>
        public DateTime LastEditionDate { get; set; }

        /// <summary>
        /// Constructor, set GUID
        /// </summary>
        public CategoryModel()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
