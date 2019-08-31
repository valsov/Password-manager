namespace PasswordManager.Service.Interfaces
{
    /// <summary>
    /// Service handling translations
    /// </summary>
    public interface ITranslationService
    {
        /// <summary>
        /// Translate the given code with the current language
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string Translate(string key);
    }
}
