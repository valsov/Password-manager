using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf.Transitions;
using Microsoft.Win32;
using PasswordManager.Messengers;
using PasswordManager.Model;
using PasswordManager.Repository.Interfaces;
using PasswordManager.Service.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace PasswordManager.ViewModel
{
    public class DatabaseSelectionViewModel : ViewModelBase
    {
        IDatabaseRepository databaseRepository;

        IIconsService iconsService;

        private bool databaseOpeningResult;

        private DateTime databaseOpeningStartTime;

        private DatabaseModel database;

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
        public DatabaseSelectionViewModel(IDatabaseRepository databaseRepository,
                                          IIconsService iconsService)
        {
            this.databaseRepository = databaseRepository;
            this.iconsService = iconsService;

            iconsService.IconsLoadedEvent += IconsLoadedEventHandler;

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
            database = null;
            Password = string.Empty;
            Error = string.Empty;
            UserControlVisibility = true;
        }

        /// <summary>
        /// Set UserControlVisibility to false
        /// </summary>
        /// <param name="obj"></param>
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
        /// Try to decrypt the database with the given password
        /// </summary>
        void TryOpenDatabase()
        {
            if (!TryOpenDatabaseEnabled || DatabaseOpeningInProgress) return;

            DatabaseOpeningInProgress = true;
            databaseOpeningStartTime = DateTime.Now;

            database = databaseRepository.LoadDatabase(databasePath, Password);
            if (database is null)
            {
                databaseOpeningResult = false;
                // Call this even if the icons aren't loaded
                IconsLoadedEventHandler(this, null);
            }
            else
            {
                databaseOpeningResult = true;
                iconsService.LoadIcons();
            }
        }

        /// <summary>
        /// Icons loaded event handler, wait for 2 seconds in total to display data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IconsLoadedEventHandler(object sender, EventArgs e)
        {
            var timespan = DateTime.Now - databaseOpeningStartTime;

            var remaining = 2000 - timespan.TotalMilliseconds;
            if (remaining > 0)
            {
                // Set a timer to avoid bruteforce on the database opening
                var timer = new Timer(remaining)
                {
                    Enabled = true,
                    AutoReset = false
                };
                timer.Elapsed += OpenDatabaseTimerElapsed;
            }
            else
            {
                OpenDatabaseTimerElapsed(this, null);
            }
        }

        /// <summary>
        /// Display the data opening attempt informations (open or show error)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenDatabaseTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (databaseOpeningResult)
            {
                UserControlVisibility = false;
                Application.Current.Dispatcher.Invoke(() => Messenger.Default.Send(new DatabaseLoadedMessage(this, database)));
            }
            else
            {
                Error = "Couldn't open the database";
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
