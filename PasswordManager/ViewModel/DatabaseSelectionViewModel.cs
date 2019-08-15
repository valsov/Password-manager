using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using PasswordManager.Messengers;
using System.IO;
using System.Windows;

namespace PasswordManager.ViewModel
{
    public class DatabaseSelectionViewModel : ViewModelBase
    {
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

        private Visibility databaseChallengeButtonVisibility;
        public Visibility DatabaseChallengeButtonVisibility
        {
            get
            {
                return databaseChallengeButtonVisibility;
            }
            set
            {
                databaseChallengeButtonVisibility = value;
                RaisePropertyChanged(nameof(DatabaseChallengeButtonVisibility));
            }
        }

        public RelayCommand SelectDatabaseFileCommand { get; private set; }

        public RelayCommand OpenDatabaseChallengeViewCommand { get; private set; }

        public RelayCommand OpenDatabaseCreationViewCommand { get; private set; }

        public DatabaseSelectionViewModel()
        {
            Messenger.Default.Register<ShowDatabaseSelectionViewMessage>(this, ShowUserControl);
            SelectDatabaseFileCommand = new RelayCommand(SelectDatabaseFile);
            OpenDatabaseChallengeViewCommand = new RelayCommand(OpenDatabaseChallengeView);
            OpenDatabaseCreationViewCommand = new RelayCommand(OpenDatabaseCreationView);
            UserControlVisibility = Visibility.Hidden;
            DatabaseChallengeButtonVisibility = Visibility.Hidden;
        }

        private void ShowUserControl(ShowDatabaseSelectionViewMessage obj)
        {
            DatabasePath = string.Empty;
            UserControlVisibility = Visibility.Visible;
            DatabaseChallengeButtonVisibility = Visibility.Hidden;
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
                DatabaseChallengeButtonVisibility = Visibility.Visible;
            }
        }

        private void OpenDatabaseChallengeView()
        {
            UserControlVisibility = Visibility.Hidden;
            Messenger.Default.Send(new ShowDatabaseChallengeViewMessage(this, databasePath));
        }

        private void OpenDatabaseCreationView()
        {
            UserControlVisibility = Visibility.Hidden;
            Messenger.Default.Send(new ShowDatabaseCreationViewMessage(this));
        }
    }
}
