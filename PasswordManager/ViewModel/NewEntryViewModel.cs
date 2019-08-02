using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PasswordManager.Messengers;
using PasswordManager.Model;
using PasswordManager.Repository.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PasswordManager.ViewModel
{
    public class NewEntryViewModel : ViewModelBase
    {
        IDatabaseRepository databaseRepository;

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
                RaisePropertyChanged(nameof(userControlVisibility));
            }
        }

        private PasswordEntryModel newPasswordEntry;
        public PasswordEntryModel NewPasswordEntry
        {
            get
            {
                return newPasswordEntry;
            }
            set
            {
                newPasswordEntry = value;
                RaisePropertyChanged(nameof(NewPasswordEntry));
            }
        }
        public ObservableCollection<string> Categories { get; set; }

        public RelayCommand CreateEntryCommand { get; private set; }

        public RelayCommand CancelEntryCreationCommand { get; private set; }

        public NewEntryViewModel(IDatabaseRepository databaseRepository)
        {
            this.databaseRepository = databaseRepository;
            CreateEntryCommand = new RelayCommand(CreateEntry);
            CancelEntryCreationCommand = new RelayCommand(CancelCreation);
            Messenger.Default.Register<ShowNewEntryViewMessage>(this, ShowUserControl);
            Messenger.Default.Register<EntrySelectedMessage>(this, (x) => UserControlVisibility = Visibility.Hidden);

            NewPasswordEntry = new PasswordEntryModel();
            UserControlVisibility = Visibility.Hidden;

            Categories = new ObservableCollection<string>();
        }

        private void ShowUserControl(ShowNewEntryViewMessage obj)
        {
            UserControlVisibility = Visibility.Visible;

            // Fill categories combobox if this wasn't already done
            if (!Categories.Any())
            {
                // string.Empty for no category
                Categories.Add(string.Empty);
                foreach (var category in databaseRepository.GetDatabase().Categories)
                {
                    Categories.Add(category);
                }
            }
        }

        private void CreateEntry()
        {
            var copy = NewPasswordEntry.Copy();
            databaseRepository.AddPasswordEntry(copy);
            Messenger.Default.Send(new EntryAddedMessage(this, copy));
            NewPasswordEntry = new PasswordEntryModel();

            UserControlVisibility = Visibility.Hidden;
        }

        private void CancelCreation()
        {
            NewPasswordEntry = new PasswordEntryModel();
            UserControlVisibility = Visibility.Hidden;
        }
    }
}
