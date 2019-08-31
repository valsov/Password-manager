using System.Runtime.CompilerServices;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using PasswordManager.Messengers;
using PasswordManager.Service.Interfaces;

namespace PasswordManager.ViewModel
{
    /// <summary>
    /// Class all ViewModels must inherit from, it handles the translations
    /// </summary>
    public class BaseViewModel : ViewModelBase
    {
        private ITranslationService translationService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="translationService"></param>
        public BaseViewModel(ITranslationService translationService)
        {
            this.translationService = translationService;

            Messenger.Default.Register<LanguageChangedMessage>(this, LanguageChangedHandler);
        }

        /// <summary>
        /// Translation property, indexer
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [IndexerName("Item")]
        public string this[string key]
        {
            get
            {
                if (translationService is null)
                {
                    return key;
                }

                return translationService.Translate(key);
            }
        }

        /// <summary>
        /// Raise indexer property changed when the application's language change
        /// </summary>
        /// <param name="obj"></param>
        private void LanguageChangedHandler(LanguageChangedMessage obj)
        {
            RaisePropertyChanged("Item[]");
        }
    }
}
