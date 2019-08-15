using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using PasswordManager.Messengers;
using PasswordManager.Service.Interfaces;

namespace PasswordManager.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        ISettingsService settingsService;

        /// <summary>
        /// Constructor
        /// </summary>
        public SettingsViewModel(ISettingsService settingsService)
        {
            this.settingsService = settingsService;
            Messenger.Default.Register<DatabaseLoadedMessage>(this, DatabaseLoadedHandler);
        }

        void DatabaseLoadedHandler(DatabaseLoadedMessage message)
        {
            // Save database path
            settingsService.SaveDatabasePath(message.DatabaseModel.Path);
        }
    }
}
