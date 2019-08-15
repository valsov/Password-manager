using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PasswordManager.Messengers;
using PasswordManager.Service.Interfaces;
using System;
using System.Windows;

namespace PasswordManager.ViewModel
{
    public class DatabaseChallengeViewModel : ViewModelBase
    {
        IDatabaseService databaseService;

        string databasePath;

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
                RaisePropertyChanged(nameof(TryOpenDatabaseEnabled));
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

        /// <summary>
        /// Command to create a database
        /// </summary>
        public RelayCommand TryOpenDatabaseCommand { get; private set; }

        public RelayCommand CancelDatabaseOpeningCommand { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="databaseService"></param>
        public DatabaseChallengeViewModel(IDatabaseService databaseService)
        {
            this.databaseService = databaseService;
            TryOpenDatabaseCommand = new RelayCommand(TryOpenDatabase);
            CancelDatabaseOpeningCommand = new RelayCommand(CancelDatabaseOpening);
            UserControlVisibility = Visibility.Collapsed;
            Messenger.Default.Register<ShowDatabaseChallengeViewMessage>(this, ShowUserControl);
        }

        /// <summary>
        /// Try to decrypt the current database with the input password
        /// </summary>
        void TryOpenDatabase()
        {
            if (string.IsNullOrEmpty(databasePath))
            {
                return;
            }

            var databaseModel = databaseService.ReadDatabase(databasePath, Password);
            if(databaseModel is null)
            {
                Error = "Couldn't open the database";
            }
            else
            {
                UserControlVisibility = Visibility.Collapsed;
                Messenger.Default.Send(new DatabaseLoadedMessage(this, databaseModel));
            }
        }

        private void CancelDatabaseOpening()
        {
            UserControlVisibility = Visibility.Hidden;
            Messenger.Default.Send(new ShowDatabaseSelectionViewMessage(this));
        }

        /// <summary>
        /// Show the linked UserControl : DatabaseChallengeView
        /// </summary>
        /// <param name="message"></param>
        void ShowUserControl(ShowDatabaseChallengeViewMessage message)
        {
            UserControlVisibility = Visibility.Visible;
            databasePath = message.DatabasePath;
        }
    }
}
