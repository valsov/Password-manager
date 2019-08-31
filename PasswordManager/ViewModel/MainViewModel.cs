using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PasswordManager.Messengers;
using PasswordManager.Repository.Interfaces;
using PasswordManager.Service.Interfaces;
using System;

namespace PasswordManager.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        IDatabaseRepository databaseRepository;

        IIconsService iconsService;

        IEncryptionService encryptionService;

        public ISettingsService settingsService { get; set; }

        public IClipboardService clipboardService { get; set; }

        private string databaseName;
        /// <summary>
        /// Name of the loaded database, to be displayed to the user
        /// </summary>
        public string DatabaseName
        {
            get
            {
                return databaseName;
            }
            set
            {
                databaseName = value;
                RaisePropertyChanged(nameof(DatabaseName));
            }
        }

        private bool mainViewVisibility;
        /// <summary>
        /// Visibility of the main view
        /// </summary>
        public bool MainViewVisibility
        {
            get
            {
                return mainViewVisibility;
            }
            set
            {
                mainViewVisibility = value;
                RaisePropertyChanged(nameof(MainViewVisibility));
            }
        }

        private bool databaseOpeningGroupVisibility;
        /// <summary>
        /// Visibility of the group containing database opening and creation views
        /// </summary>
        public bool DatabaseOpeningGroupVisibility
        {
            get
            {
                return databaseOpeningGroupVisibility;
            }
            set
            {
                databaseOpeningGroupVisibility = value;
                RaisePropertyChanged(nameof(DatabaseOpeningGroupVisibility));
            }
        }

        private bool settingsVisibility;
        public bool SettingsVisibility
        {
            get
            {
                return settingsVisibility;
            }
            set
            {
                settingsVisibility = value;
                RaisePropertyChanged(nameof(SettingsVisibility));
            }
        }

        public RelayCommand LoadedCommand
        {
            get
            {
                return new RelayCommand(ViewLoadedHandler);
            }
        }

        public RelayCommand OpenSettingsViewCommand
        {
            get
            {
                return new RelayCommand(OpenSettingsView);
            }
        }

        public RelayCommand OpenSyncViewCommand
        {
            get
            {
                return new RelayCommand(OpenSyncView);
            }
        }

        public RelayCommand CloseDatabaseCommand
        {
            get
            {
                return new RelayCommand(CloseDatabase);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settingsService"></param>
        /// <param name="databaseRepository"></param>
        /// <param name="iconsService"></param>
        /// <param name="encryptionService"></param>
        /// <param name="clipboardService"></param>
        public MainViewModel(ISettingsService settingsService,
                             IDatabaseRepository databaseRepository,
                             IIconsService iconsService,
                             IEncryptionService encryptionService,
                             IClipboardService clipboardService)
        {
            this.settingsService = settingsService;
            this.databaseRepository = databaseRepository;
            this.iconsService = iconsService;
            this.encryptionService = encryptionService;
            this.clipboardService = clipboardService;

            MainViewVisibility = false;
            DatabaseOpeningGroupVisibility = true;
            SettingsVisibility = false;

            Messenger.Default.Register<DatabaseLoadedMessage>(this, DatabaseLoadedHandler);
            Messenger.Default.Register<ToggleSettingsViewMessage>(this, SettingsViewVisibilityHandler);
        }

        /// <summary>
        /// Show the main view
        /// </summary>
        /// <param name="obj"></param>
        private void DatabaseLoadedHandler(DatabaseLoadedMessage obj)
        {
            DatabaseName = obj.DatabaseModel.Name;
            DatabaseOpeningGroupVisibility = false;
            MainViewVisibility = true;
        }

        /// <summary>
        /// Handle the settings view toggle
        /// </summary>
        /// <param name="obj"></param>
        private void SettingsViewVisibilityHandler(ToggleSettingsViewMessage obj)
        {
            SettingsVisibility = obj.Visibility;
        }

        /// <summary>
        /// Show the database selection view when the view is loaded
        /// </summary>
        void ViewLoadedHandler()
        {
            var path = settingsService.GetDatabasePath();
            Messenger.Default.Send(new ShowDatabaseSelectionViewMessage(this, path));
        }

        /// <summary>
        /// Reset the view to the database selection view and remove all the database related ressources
        /// </summary>
        private void CloseDatabase()
        {
            MainViewVisibility = false;
            DatabaseOpeningGroupVisibility = true;
            DatabaseName = string.Empty;

            var path = settingsService.GetDatabasePath();
            databaseRepository.UnloadDatabase();
            iconsService.UnloadIcons();
            encryptionService.Clear();

            Messenger.Default.Send(new DatabaseUnloadedMessage(this));
            Messenger.Default.Send(new ShowDatabaseSelectionViewMessage(this, path));
        }

        /// <summary>
        /// Show the settings view modal
        /// </summary>
        private void OpenSettingsView()
        {
            Messenger.Default.Send(new ToggleSettingsViewMessage(this, true));
        }

        /// <summary>
        /// Show the sync view modal
        /// </summary>
        private void OpenSyncView()
        {
            throw new NotImplementedException();
        }
    }
}
