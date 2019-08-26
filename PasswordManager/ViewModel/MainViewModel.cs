using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PasswordManager.Messengers;
using PasswordManager.Repository.Interfaces;
using PasswordManager.Service.Interfaces;

namespace PasswordManager.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        ISettingsService settingsService;

        IDatabaseRepository databaseRepository;

        IIconsService iconsService;

        IEncryptionService encryptionService;

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

        public RelayCommand LoadedCommand { get; private set; }

        public RelayCommand CloseDatabaseCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class
        /// </summary>
        public MainViewModel(ISettingsService settingsService,
                             IDatabaseRepository databaseRepository,
                             IIconsService iconsService,
                             IEncryptionService encryptionService)
        {
            this.settingsService = settingsService;
            this.databaseRepository = databaseRepository;
            this.iconsService = iconsService;
            this.encryptionService = encryptionService;

            MainViewVisibility = false;
            DatabaseOpeningGroupVisibility = true;

            Messenger.Default.Register<DatabaseLoadedMessage>(this, DatabaseLoadedHandler);

            LoadedCommand = new RelayCommand(ViewLoadedHandler);
            CloseDatabaseCommand = new RelayCommand(CloseDatabase);
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
    }
}
