using System.Collections.Generic;

namespace PasswordManager.Repository.Interfaces
{
    /// <summary>
    /// Provides an access to the application's translation bases
    /// </summary>
    public interface ITranslationRepository
    {
        /// <summary>
        /// Get the translation base with the current language
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> GetTranslationBase();
    }
}
