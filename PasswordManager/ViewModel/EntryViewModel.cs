using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PasswordManager.Messengers;
using PasswordManager.Model;
using PasswordManager.Repository.Interfaces;
using PasswordManager.Service.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PasswordManager.ViewModel
{
    public class EntryViewModel : ViewModelBase
    {
        IDatabaseRepository databaseRepository;

        IPasswordService passwordService;

        private PasswordEntryModel backupPasswordEntry;

        private PasswordEntryModel passwordEntry;
        public PasswordEntryModel PasswordEntry
        {
            get
            {
                return passwordEntry;
            }
            set
            {
                passwordEntry = value;
                RaisePropertyChanged(nameof(PasswordEntry));
            }
        }

        public ObservableCollection<string> Categories { get; set; }

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

        private Visibility entryDataVisibility;
        public Visibility EntryDataVisibility
        {
            get
            {
                return entryDataVisibility;
            }
            set
            {
                entryDataVisibility = value;
                RaisePropertyChanged(nameof(EntryDataVisibility));
            }
        }

        private Visibility editionButtonVisibility;
        public Visibility EditionButtonVisibility
        {
            get
            {
                return editionButtonVisibility;
            }
            set
            {
                editionButtonVisibility = value;
                RaisePropertyChanged(nameof(EditionButtonVisibility));
            }
        }

        private Visibility editionFormVisibility;
        public Visibility EditionFormVisibility
        {
            get
            {
                return editionFormVisibility;
            }
            set
            {
                editionFormVisibility = value;
                RaisePropertyChanged(nameof(EditionFormVisibility));
            }
        }

        private Visibility editionControlButtonsVisibility;
        public Visibility EditionControlButtonsVisibility
        {
            get
            {
                return editionControlButtonsVisibility;
            }
            set
            {
                editionControlButtonsVisibility = value;
                RaisePropertyChanged(nameof(EditionControlButtonsVisibility));
            }
        }

        public RelayCommand StartEditionCommand { get; private set; }

        public RelayCommand ValidateEditionCommand { get; private set; }

        public RelayCommand CancelEditionCommand { get; private set; }

        public EntryViewModel(IDatabaseRepository databaseRepository,
                              IPasswordService passwordService)
        {
            this.databaseRepository = databaseRepository;
            this.passwordService = passwordService;
            Messenger.Default.Register<EntrySelectedMessage>(this, EntrySelectedHandler);
            Messenger.Default.Register<ShowNewEntryViewMessage>(this, (x) => UserControlVisibility = Visibility.Hidden);
            StartEditionCommand = new RelayCommand(StartEdition);
            ValidateEditionCommand = new RelayCommand(ValidateEdition);
            CancelEditionCommand = new RelayCommand(CancelEdition);

            backupPasswordEntry = new PasswordEntryModel();
            Categories = new ObservableCollection<string>();

            UserControlVisibility = Visibility.Hidden;
            EntryDataVisibility = Visibility.Visible;
            EditionButtonVisibility = Visibility.Visible;
            EditionFormVisibility = Visibility.Hidden;
            EditionControlButtonsVisibility = Visibility.Hidden;
        }

        private void EntrySelectedHandler(EntrySelectedMessage obj)
        {
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
            PasswordEntry = obj.passwordEntry;

            UserControlVisibility = Visibility.Visible;
            SetElementsVisibility(false);
        }

        private void StartEdition()
        {
            backupPasswordEntry = PasswordEntry.Copy();
            SetElementsVisibility(true);
        }

        private void ValidateEdition()
        {
            PasswordEntry.PasswordStrength = passwordService.CheckPasswordStrength(PasswordEntry.Password);
            RaisePropertyChanged(nameof(PasswordEntry));

            // Copy to avoid same instance manipulation
            var copy = PasswordEntry.Copy();
            databaseRepository.UpdatePasswordEntry(copy);
            Messenger.Default.Send(new EntryEditedMessage(this, copy));
            SetElementsVisibility(false);
        }

        private void CancelEdition()
        {
            PasswordEntry = backupPasswordEntry.Copy();
            SetElementsVisibility(false);
        }

        private void SetElementsVisibility(bool isInEditionMode)
        {
            if (isInEditionMode)
            {
                EntryDataVisibility = Visibility.Hidden;
                EditionButtonVisibility = Visibility.Hidden;
                EditionFormVisibility = Visibility.Visible;
                EditionControlButtonsVisibility = Visibility.Visible;
            }
            else
            {
                EntryDataVisibility = Visibility.Visible;
                EditionButtonVisibility = Visibility.Visible;
                EditionFormVisibility = Visibility.Hidden;
                EditionControlButtonsVisibility = Visibility.Hidden;
            }
        }
    }
}
