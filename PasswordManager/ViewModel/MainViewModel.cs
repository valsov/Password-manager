using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PasswordManager.Messengers;
using PasswordManager.Repository.Interfaces;
using PasswordManager.Service.Interfaces;
using System.Windows;

namespace PasswordManager.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        ISettingsService settingsService;

        IDatabaseRepository databaseRepository;

        private Visibility mainViewVisibility;
        public Visibility MainViewVisibility
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

        public RelayCommand LoadedCommand { get; private set; }

        public RelayCommand CloseDatabaseCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(ISettingsService settingsService,
                             IDatabaseRepository databaseRepository)
        {
            this.settingsService = settingsService;
            this.databaseRepository = databaseRepository;
            Messenger.Default.Register<DatabaseLoadedMessage>(this, DatabaseLoadedHandler);
            LoadedCommand = new RelayCommand(ViewLoadedHandler);
            CloseDatabaseCommand = new RelayCommand(CloseDatabase);
            MainViewVisibility = Visibility.Hidden;
        }

        private void DatabaseLoadedHandler(DatabaseLoadedMessage obj)
        {
            MainViewVisibility = Visibility.Visible;
        }

        void ViewLoadedHandler()
        {
            var path = settingsService.GetDatabasePath();
            if (string.IsNullOrEmpty(path) || !databaseRepository.CheckDatabaseExists(path))
            {
                // Database path isn't set or database doesn't exist anymore
                Messenger.Default.Send(new ShowDatabaseSelectionViewMessage(this));
            }
            else
            {
                Messenger.Default.Send(new ShowDatabaseChallengeViewMessage(this, path));
            }
        }

        private void CloseDatabase()
        {
            MainViewVisibility = Visibility.Hidden;
            databaseRepository.UnloadDatabase();
            Messenger.Default.Send(new DatabaseUnloadedMessage(this));
            Messenger.Default.Send(new ShowDatabaseSelectionViewMessage(this));
        }
    }
}
