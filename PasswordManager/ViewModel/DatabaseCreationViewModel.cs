using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf.Transitions;
using Microsoft.Win32;
using PasswordManager.Messengers;
using PasswordManager.Model;
using PasswordManager.Repository.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace PasswordManager.ViewModel
{
    public class DatabaseCreationViewModel : ViewModelBase
    {
        IDatabaseRepository databaseRepository;

        private string databasePath;
        /// <summary>
        /// Database file location, returns the filename only
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

        private bool userControlVisibility;
        /// <summary>
        /// DatabaseCreation UserControl visibility
        /// </summary>
        public bool UserControlVisibility
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

        public RelayCommand SelectDatabaseFileCommand { get; private set; }

        public RelayCommand CreateDatabaseCommand { get; private set; }

        public RelayCommand CancelDatabaseCreationCommand { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="databaseService"></param>
        public DatabaseCreationViewModel(IDatabaseRepository databaseRepository)
        {
            this.databaseRepository = databaseRepository;

            UserControlVisibility = false;

            Messenger.Default.Register<ShowDatabaseCreationViewMessage>(this, ShowUserControl);
            Messenger.Default.Register<DatabaseLoadedMessage>(this, HideUserControl);

            SelectDatabaseFileCommand = new RelayCommand(SelectDatabaseFile);
            CreateDatabaseCommand = new RelayCommand(CreateDatabase);
            CancelDatabaseCreationCommand = new RelayCommand(CancelDatabaseCreation);
        }

        /// <summary>
        /// Open a file dialog to select a database file
        /// </summary>
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
            if (!IsCreateDatabaseEnabled)
            {
                return;
            }

            var databaseModel = new DatabaseModel()
            {
                Name = DatabaseName,
                Path = databasePath,
                Categories = new List<string>(),
                PasswordEntries = new List<PasswordEntryModel>()
            };

            var result = databaseRepository.WriteDatabase(databaseModel, Password);
            if (result)
            {
                UserControlVisibility = false;
                Transitioner.MovePreviousCommand.Execute(null, null);
                Messenger.Default.Send(new DatabaseLoadedMessage(this, databaseModel));
            }
            else
            {
                Error = "Couldn't create the database";
            }
        }

        /// <summary>
        /// Switch the view to the database selection view
        /// </summary>
        private void CancelDatabaseCreation()
        {
            Transitioner.MovePreviousCommand.Execute(null, null);
            Messenger.Default.Send(new ShowDatabaseSelectionViewMessage(this, string.Empty));
        }

        /// <summary>
        /// Show the linked UserControl : DatabaseCreationView
        /// </summary>
        /// <param name="message"></param>
        void ShowUserControl(ShowDatabaseCreationViewMessage message)
        {
            UserControlVisibility = true;
        }

        private void HideUserControl(DatabaseLoadedMessage obj)
        {
            UserControlVisibility = false;
        }
    }
}
