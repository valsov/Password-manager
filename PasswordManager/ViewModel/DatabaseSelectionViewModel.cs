using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf.Transitions;
using Microsoft.Win32;
using PasswordManager.Messengers;
using PasswordManager.Repository.Interfaces;
using System;
using System.IO;
using System.Timers;
using System.Windows;

namespace PasswordManager.ViewModel
{
    public class DatabaseSelectionViewModel : ViewModelBase
    {
        IDatabaseRepository databaseRepository;

        private string databasePath;
        /// <summary>
        /// Path of the database to open
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
                RaisePropertyChanged(nameof(PasswordEnabled));
            }
        }

        private string password;
        /// <summary>
        /// Password to decrypt the database
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
                RaisePropertyChanged(nameof(Password));
                RaisePropertyChanged(nameof(TryOpenDatabaseEnabled));
            }
        }

        /// <summary>
        /// Indicates wether the databasePath property is filled
        /// </summary>
        public bool PasswordEnabled
        {
            get
            {
                return !string.IsNullOrEmpty(databasePath);
            }
        }

        /// <summary>
        /// Indicates wether the password property is filled
        /// </summary>
        public bool TryOpenDatabaseEnabled
        {
            get
            {
                return !string.IsNullOrEmpty(Password);
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

        private bool databaseOpeningInProgress;
        /// <summary>
        /// Indicate if the database is being opened
        /// </summary>
        public bool DatabaseOpeningInProgress
        {
            get
            {
                return databaseOpeningInProgress;
            }
            set
            {
                databaseOpeningInProgress = value;
                RaisePropertyChanged(nameof(DatabaseOpeningInProgress));
            }
        }

        private bool userControlVisibility;
        /// <summary>
        /// DatabaseSelection UserControl visibility
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
                RaisePropertyChanged(nameof(UserControlVisibility));
            }
        }

        public RelayCommand SelectDatabaseFileCommand { get; private set; }

        public RelayCommand TryOpenDatabaseCommand { get; private set; }

        public RelayCommand OpenDatabaseCreationViewCommand { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="databaseRepository"></param>
        public DatabaseSelectionViewModel(IDatabaseRepository databaseRepository)
        {
            this.databaseRepository = databaseRepository;

            UserControlVisibility = false;

            Messenger.Default.Register<ShowDatabaseSelectionViewMessage>(this, ShowUserControl);
            Messenger.Default.Register<DatabaseLoadedMessage>(this, HideUserControl);

            SelectDatabaseFileCommand = new RelayCommand(SelectDatabaseFile);
            TryOpenDatabaseCommand = new RelayCommand(TryOpenDatabase);
            OpenDatabaseCreationViewCommand = new RelayCommand(OpenDatabaseCreationView);
        }

        /// <summary>
        /// Display the UserControl and set the database path
        /// </summary>
        /// <param name="obj"></param>
        private void ShowUserControl(ShowDatabaseSelectionViewMessage obj)
        {
            DatabasePath = obj.Path;
            Password = string.Empty;
            Error = string.Empty;
            UserControlVisibility = true;
        }

        private void HideUserControl(DatabaseLoadedMessage obj)
        {
            UserControlVisibility = false;
        }

        /// <summary>
        /// Open a file dialog to select a database file
        /// </summary>
        private void SelectDatabaseFile()
        {
            var fileDialog = new OpenFileDialog()
            {
                Title = "Choose your Database file",
                Filter = "Password Databases|*.crypt",
                CheckFileExists = true
            };

            var result = fileDialog.ShowDialog();
            if(result == true)
            {
                DatabasePath = fileDialog.FileName;
            }
        }

        /// <summary>
        /// Set a timer to avoid bruteforce on the database opening and display an animation
        /// </summary>
        void TryOpenDatabase()
        {
            if (DatabaseOpeningInProgress) return;

            DatabaseOpeningInProgress = true;
            // Wait 1.5 sec before performing the operation
            var timer = new Timer(2000)
            {
                Enabled = true,
                AutoReset = false
            };
            timer.Elapsed += OpenDatabaseTimerElapsed;
        }

        /// <summary>
        /// Try to decrypt the current database with the input password
        /// </summary>
        private void OpenDatabaseTimerElapsed(object sender, ElapsedEventArgs e)
        {
            var databaseModel = databaseRepository.LoadDatabase(databasePath, Password);
            if (databaseModel is null)
            {
                Error = "Couldn't open the database";
            }
            else
            {
                UserControlVisibility = false;
                // Need to use dispatcher because timers run in another thread
                Application.Current.Dispatcher.Invoke(() => Messenger.Default.Send(new DatabaseLoadedMessage(this, databaseModel)));
            }

            DatabaseOpeningInProgress = false;
        }

        /// <summary>
        /// Switch to the database creation view
        /// </summary>
        private void OpenDatabaseCreationView()
        {
            Transitioner.MoveNextCommand.Execute(null, null);
            Messenger.Default.Send(new ShowDatabaseCreationViewMessage(this));
        }
    }
}
