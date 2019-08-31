using Newtonsoft.Json;
using PasswordManager.Model;
using PasswordManager.Repository.Interfaces;
using PasswordManager.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PasswordManager.Repository
{
    /// <summary>
    /// Provides an access to the application's translation bases
    /// </summary>
    public class TranslationRepository : ITranslationRepository
    {
        private ISettingsService settingsService;

        private List<TranslationModel> translationBases;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settingsService"></param>
        public TranslationRepository(ISettingsService settingsService)
        {
            this.settingsService = settingsService;
        }

        /// <summary>
        /// Get the translation base with the current language
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetTranslationBase()
        {
            if (translationBases is null)
            {
                InitializeTranslationBases();
            }

            return translationBases.FirstOrDefault(x => x.Language == settingsService.GetLanguage())?.TranslationBase;
        }

        /// <summary>
        /// Retrieve the language databases from the disk
        /// </summary>
        private void InitializeTranslationBases()
        {
            translationBases = new List<TranslationModel>();

            try
            {
                foreach (var file in Directory.GetFiles(Constants.TranslationsPath))
                {
                    var json = File.ReadAllText(file);
                    var data = JsonConvert.DeserializeObject<TranslationModel>(json);
                    translationBases.Add(data);
                }
            }
            catch (Exception)
            {
                // Ignored, the list is still set
            }
        }
    }
}
