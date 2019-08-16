using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using PasswordManager.Messengers;
using PasswordManager.Service.Interfaces;
using System;
using System.IO;
using System.Timers;
using System.Windows;

namespace PasswordManager.ViewModel
{
    public class DatabaseSelectionViewModel : ViewModelBase
    {
        IDatabaseService databaseService;

        private string databasePath;
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

        public bool PasswordEnabled
        {
            get
            {
                return !string.IsNullOrEmpty(databasePath);
            }
        }

        /// <summary>
        /// Indicates wether all the required fields are filled
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

        private Visibility userControlVisibility;
        public Visibility UserControlVisibility
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

        public DatabaseSelectionViewModel(IDatabaseService databaseService)
        {
            this.databaseService = databaseService;
            Messenger.Default.Register<ShowDatabaseSelectionViewMessage>(this, ShowUserControl);
            SelectDatabaseFileCommand = new RelayCommand(SelectDatabaseFile);
            TryOpenDatabaseCommand = new RelayCommand(TryOpenDatabase);
            OpenDatabaseCreationViewCommand = new RelayCommand(OpenDatabaseCreationView);
            UserControlVisibility = Visibility.Hidden;
        }

        private void ShowUserControl(ShowDatabaseSelectionViewMessage obj)
        {
            DatabasePath = obj.Path;
            Password = string.Empty;
            Error = string.Empty;
            UserControlVisibility = Visibility.Visible;
        }

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
            var databaseModel = databaseService.ReadDatabase(databasePath, Password);
            if (databaseModel is null)
            {
                Error = "Couldn't open the database";
            }
            else
            {
                UserControlVisibility = Visibility.Hidden;
                // Need to use dispatcher because timers run in another thread
                Application.Current.Dispatcher.Invoke(() => Messenger.Default.Send(new DatabaseLoadedMessage(this, databaseModel)));
            }

            DatabaseOpeningInProgress = false;
        }

        private void OpenDatabaseCreationView()
        {
            UserControlVisibility = Visibility.Hidden;
            Messenger.Default.Send(new ShowDatabaseCreationViewMessage(this));
        }
    }
}
