using GalaSoft.MvvmLight;
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
    public class SettingsViewModel : ViewModelBase
    {
        ISettingsService settingsService;

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
        /// <param name="settingsService"></param>
        public SettingsViewModel(ISettingsService settingsService)
        {
            this.settingsService = settingsService;

            Messenger.Default.Register<DatabaseLoadedMessage>(this, DatabaseLoadedHandler);
        }

        /// <summary>
        /// Save the database path
        /// </summary>
        /// <param name="message"></param>
        void DatabaseLoadedHandler(DatabaseLoadedMessage message)
        {
            settingsService.SaveDatabasePath(message.DatabaseModel.Path);
        }
    }
}
