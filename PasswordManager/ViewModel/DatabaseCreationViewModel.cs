using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PasswordManager.Messengers;
using PasswordManager.Service.Interfaces;
using System;
using System.Windows;

namespace PasswordManager.ViewModel
{
    public class DatabaseCreationViewModel : ViewModelBase
    {
        IDatabaseService databaseService;

        private Visibility userControlVisibility;
        /// <summary>
        /// DatabaseCreation UserControl visibility
        /// </summary>
        public Visibility UserControlVisibility
        {
            get
            {
                return userControlVisibility;
            }
            set
            {
                userControlVisibility = value;
                RaisePropertyChanged(nameof(userControlVisibility));
            }
        }

        private string databaseLocation;
        /// <summary>
        /// Database file location
        /// </summary>
        public string DatabaseLocation
        {
            get
            {
                return databaseLocation;
            }
            set
            {
                databaseLocation = value;
                RaisePropertyChanged(nameof(IsCreateDatabaseEnabled));
            }
        }

        private string databaseName;
        /// <summary>
        /// Name of the database
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
                RaisePropertyChanged(nameof(IsCreateDatabaseEnabled));
            }
        }

        private string password;
        /// <summary>
        /// Password used to secure the database
        /// </summary>
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
                RaisePropertyChanged(nameof(IsCreateDatabaseEnabled));
            }
        }

        /// <summary>
        /// Indicates wether all the required fields are filled
        /// </summary>
        public bool IsCreateDatabaseEnabled
        {
            get
            {
                return !string.IsNullOrEmpty(DatabaseLocation) && !string.IsNullOrEmpty(DatabaseName) && !string.IsNullOrEmpty(Password);
            }
        }

        private string error;
        /// <summary>
        /// Error message
        /// </summary>
        public string Error
        {
            get
            {
                return error;
            }
            set
            {
                error = value;
                RaisePropertyChanged(nameof(Error));
            }
        }

        /// <summary>
        /// Command to create a database
        /// </summary>
        public RelayCommand CreateDatabaseCommand { get; private set; }

        public RelayCommand CancelDatabaseCreationCommand { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="databaseService"></param>
        public DatabaseCreationViewModel(IDatabaseService databaseService)
        {
            this.databaseService = databaseService;
            CreateDatabaseCommand = new RelayCommand(CreateDatabase);
            CancelDatabaseCreationCommand = new RelayCommand(CancelDatabaseCreation);
            UserControlVisibility = Visibility.Collapsed;
            Messenger.Default.Register<ShowDatabaseCreationViewMessage>(this, ShowUserControl);
        }

        /// <summary>
        /// Create a password database with the input data
        /// </summary>
        void CreateDatabase()
        {
            if (string.IsNullOrEmpty(DatabaseLocation) || string.IsNullOrEmpty(DatabaseName) || string.IsNullOrEmpty(Password))
            {
                return;
            }

            var databaseModel = databaseService.CreateDatabase(DatabaseLocation, DatabaseName, Password);
            if (databaseModel is null)
            {
                Error = "IO error: Couldn't create the database";
            }
            else
            {
                UserControlVisibility = Visibility.Collapsed;
                Messenger.Default.Send(new DatabaseLoadedMessage(this, databaseModel));
            }
        }

        private void CancelDatabaseCreation()
        {
            UserControlVisibility = Visibility.Hidden;
            Messenger.Default.Send(new ShowDatabaseSelectionViewMessage(this));
        }

        /// <summary>
        /// Show the linked UserControl : DatabaseCreationView
        /// </summary>
        /// <param name="message"></param>
        void ShowUserControl(ShowDatabaseCreationViewMessage message)
        {
            UserControlVisibility = Visibility.Visible;
        }
    }
}
