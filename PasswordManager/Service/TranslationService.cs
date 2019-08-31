using PasswordManager.Repository.Interfaces;
using PasswordManager.Service.Interfaces;

namespace PasswordManager.Service
{
    /// <summary>
    /// Service handling translations
    /// </summary>
    public class TranslationService : ITranslationService
    {
        private ITranslationRepository translationRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="translationRepository"></param>
        public TranslationService(ITranslationRepository translationRepository)
        {
            this.translationRepository = translationRepository;
        }

        /// <summary>
        /// Translate the given code with the current language
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Translate(string key)
        {
            var translationBase = translationRepository.GetTranslationBase();
            if (translationBase is null)
            {
                return key;
            }

            if (translationBase.TryGetValue(key, out string result))
            {
                return result;
            }

            return key;
        }
    }
}
