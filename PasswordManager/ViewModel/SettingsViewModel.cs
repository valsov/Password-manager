using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PasswordManager.Messengers;
using PasswordManager.Model;
using PasswordManager.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PasswordManager.ViewModel
{
    public class SettingsViewModel : BaseViewModel
    {
        private ISettingsService settingsService;

        /// <summary>
        /// List of available languages
        /// </summary>
        public IEnumerable<Languages> LanguagesList
        {
            get
            {
                return Enum.GetValues(typeof(Languages)).Cast<Languages>();
            }
        }

        /// <summary>
        /// Currently selected language
        /// </summary>
        public Languages SelectedLanguage
        {
            get
            {
                return settingsService.GetLanguage();
            }
            set
            {
                settingsService.SaveLanguage(value);
                Messenger.Default.Send(new LanguageChangedMessage(this, SelectedLanguage));
                RaisePropertyChanged(nameof(SelectedLanguage));
            }
        }

        /// <summary>
        /// List of available color themes
        /// </summary>
        public IEnumerable<ColorThemes> ColorThemesList
        {
            get
            {
                return Enum.GetValues(typeof(ColorThemes)).Cast<ColorThemes>();
            }
        }

        /// <summary>
        /// Currently selected color theme
        /// </summary>
        public ColorThemes SelectedColorTheme
        {
            get
            {
                return settingsService.GetTheme();
            }
            set
            {
                settingsService.SaveTheme(value);
                RaisePropertyChanged(nameof(SelectedColorTheme));
            }
        }

        /// <summary>
        /// Duration for the clipboard timer
        /// </summary>
        public int ClipboardTimerDuration
        {
            get
            {
                return settingsService.GetClipboardTimerDuration();
            }
            set
            {
                settingsService.SaveClipboardTimerDuration(value);
                RaisePropertyChanged(nameof(ClipboardTimerDuration));
            }
        }

        public RelayCommand CloseCommand
        {
            get
            {
                return new RelayCommand(() => Messenger.Default.Send(new ToggleSettingsViewMessage(this, false)));
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="translationService"></param>
        /// <param name="settingsService"></param>
        public SettingsViewModel(ITranslationService translationService,
                                 ISettingsService settingsService)
            : base(translationService)
        {
            this.settingsService = settingsService;

            Messenger.Default.Register<LanguageChangedMessage>(this, LanguageChangedHandler);
        }

        /// <summary>
        /// Raise ClipboardTimerDuration property changed when the application's language change
        /// </summary>
        /// <param name="obj"></param>
        private void LanguageChangedHandler(LanguageChangedMessage obj)
        {
            RaisePropertyChanged(nameof(ClipboardTimerDuration));
        }
    }
}
