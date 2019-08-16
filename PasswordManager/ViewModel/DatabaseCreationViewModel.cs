using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using PasswordManager.Messengers;
using PasswordManager.Service.Interfaces;
using System.IO;
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

        private string databasePath;
        /// <summary>
        /// Database file location
        /// </summary>
        public string DatabasePath
        {
            get
            {
                return Path.GetFileName(databasePath);
            }
            set
            {
                databasePath = value;
                RaisePropertyChanged(nameof(DatabasePath));
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
                return !string.IsNullOrEmpty(databasePath) && !string.IsNullOrEmpty(DatabaseName) && !string.IsNullOrEmpty(Password);
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

        public RelayCommand SelectDatabaseFileCommand { get; private set; }

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
            SelectDatabaseFileCommand = new RelayCommand(SelectDatabaseFile);
            CreateDatabaseCommand = new RelayCommand(CreateDatabase);
            CancelDatabaseCreationCommand = new RelayCommand(CancelDatabaseCreation);
            UserControlVisibility = Visibility.Collapsed;
            Messenger.Default.Register<ShowDatabaseCreationViewMessage>(this, ShowUserControl);
        }

        private void SelectDatabaseFile()
        {
            var fileDialog = new SaveFileDialog()
            {
                Title = "Choose your Database file",
                Filter = "Password Databases|*.crypt"
            };

            var result = fileDialog.ShowDialog();
            if (result == true)
            {
                DatabasePath = fileDialog.FileName;
            }
        }

        /// <summary>
        /// Create a password database with the input data
        /// </summary>
        void CreateDatabase()
        {
            if (string.IsNullOrEmpty(databasePath) || string.IsNullOrEmpty(DatabaseName) || string.IsNullOrEmpty(Password))
            {
                return;
            }

            var databaseModel = databaseService.CreateDatabase(databasePath, DatabaseName, Password);
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
            Messenger.Default.Send(new ShowDatabaseSelectionViewMessage(this, string.Empty));
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
