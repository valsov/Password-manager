using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PasswordManager.Extensions;
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

        private Visibility deleteButtonVisibility;
        public Visibility DeleteButtonVisibility
        {
            get
            {
                return deleteButtonVisibility;
            }
            set
            {
                deleteButtonVisibility = value;
                RaisePropertyChanged(nameof(DeleteButtonVisibility));
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

        private Visibility creationControlButtonsVisibility;
        public Visibility CreationControlButtonsVisibility
        {
            get
            {
                return creationControlButtonsVisibility;
            }
            set
            {
                creationControlButtonsVisibility = value;
                RaisePropertyChanged(nameof(CreationControlButtonsVisibility));
            }
        }
        public RelayCommand DeleteEntryCommand { get; private set; }

        public RelayCommand StartEditionCommand { get; private set; }

        public RelayCommand ValidateEditionCommand { get; private set; }

        public RelayCommand CancelEditionCommand { get; private set; }

        public RelayCommand CreateEntryCommand { get; private set; }

        public RelayCommand CancelEntryCreationCommand { get; private set; }

        public EntryViewModel(IDatabaseRepository databaseRepository,
                              IPasswordService passwordService)
        {
            this.databaseRepository = databaseRepository;
            this.passwordService = passwordService;
            Messenger.Default.Register<DatabaseUnloadedMessage>(this, DatabaseUnloadedHandler);
            Messenger.Default.Register<EntrySelectedMessage>(this, EntrySelectedHandler);
            Messenger.Default.Register<ShowNewEntryViewMessage>(this, StartEntryCreation);
            Messenger.Default.Register<CategoryAddedMessage>(this, HandleNewCategory);
            Messenger.Default.Register<CategoryDeletedMessage>(this, CategoryDeletedHandler);
            Messenger.Default.Register<CategoryEditedMessage>(this, CategoryEditedHandler);
            DeleteEntryCommand = new RelayCommand(DeleteEntry);
            StartEditionCommand = new RelayCommand(StartEdition);
            ValidateEditionCommand = new RelayCommand(ValidateEdition);
            CancelEditionCommand = new RelayCommand(CancelEdition);
            CreateEntryCommand = new RelayCommand(CreateEntry);
            CancelEntryCreationCommand = new RelayCommand(CancelCreation);

            backupPasswordEntry = new PasswordEntryModel();
            Categories = new ObservableCollection<string>();

            UserControlVisibility = Visibility.Hidden;
        }

        private void DatabaseUnloadedHandler(DatabaseUnloadedMessage obj)
        {
            Categories.Clear();
            backupPasswordEntry = null;
            PasswordEntry = null;
            SetElementsVisibility(ViewModes.View);
        }

        private void EntrySelectedHandler(EntrySelectedMessage obj)
        {
            AddCategories();
            PasswordEntry = obj.passwordEntry;

            UserControlVisibility = Visibility.Visible;
            SetElementsVisibility(ViewModes.View);
        }

        private void DeleteEntry()
        {
            databaseRepository.DeletePasswordEntry(PasswordEntry);
            Messenger.Default.Send(new EntryDeletedMessage(this, PasswordEntry));
            UserControlVisibility = Visibility.Hidden;
        }

        private void StartEdition()
        {
            backupPasswordEntry = PasswordEntry.Copy();
            SetElementsVisibility(ViewModes.Edition);
        }

        private void ValidateEdition()
        {
            PasswordEntry.PasswordStrength = passwordService.CheckPasswordStrength(PasswordEntry.Password);
            RaisePropertyChanged(nameof(PasswordEntry));

            // Copy to avoid same instance manipulation
            var copy = PasswordEntry.Copy();
            databaseRepository.UpdatePasswordEntry(copy);
            Messenger.Default.Send(new EntryEditedMessage(this, copy));
            SetElementsVisibility(ViewModes.View);
        }

        private void CancelEdition()
        {
            PasswordEntry = backupPasswordEntry.Copy();
            SetElementsVisibility(ViewModes.View);
        }

        private void StartEntryCreation(ShowNewEntryViewMessage obj)
        {
            AddCategories();
            PasswordEntry = new PasswordEntryModel();
            UserControlVisibility = Visibility.Visible;
            SetElementsVisibility(ViewModes.Creation);
        }

        private void CreateEntry()
        {
            PasswordEntry.PasswordStrength = passwordService.CheckPasswordStrength(PasswordEntry.Password);
            var copy = PasswordEntry.Copy();
            databaseRepository.AddPasswordEntry(copy);
            Messenger.Default.Send(new EntryAddedMessage(this, copy));
        }

        private void CancelCreation()
        {
            UserControlVisibility = Visibility.Hidden;
        }

        private void SetElementsVisibility(ViewModes mode)
        {
            switch (mode)
            {
                case ViewModes.View:
                    EntryDataVisibility = Visibility.Visible;
                    DeleteButtonVisibility = Visibility.Visible;
                    EditionButtonVisibility = Visibility.Visible;
                    EditionFormVisibility = Visibility.Hidden;
                    EditionControlButtonsVisibility = Visibility.Hidden;
                    CreationControlButtonsVisibility = Visibility.Hidden;
                    break;
                case ViewModes.Edition:
                    EntryDataVisibility = Visibility.Hidden;
                    DeleteButtonVisibility = Visibility.Hidden;
                    EditionButtonVisibility = Visibility.Hidden;
                    EditionFormVisibility = Visibility.Visible;
                    EditionControlButtonsVisibility = Visibility.Visible;
                    CreationControlButtonsVisibility = Visibility.Hidden;
                    break;
                case ViewModes.Creation:
                    EntryDataVisibility = Visibility.Hidden;
                    DeleteButtonVisibility = Visibility.Hidden;
                    EditionButtonVisibility = Visibility.Hidden;
                    EditionFormVisibility = Visibility.Visible;
                    EditionControlButtonsVisibility = Visibility.Hidden;
                    CreationControlButtonsVisibility = Visibility.Visible;
                    break;
            }
        }

        private void AddCategories()
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
        }

        private void HandleNewCategory(CategoryAddedMessage obj)
        {
            if (Categories.Any())
            {
                Categories.Add(obj.NewCategory);
            }
        }

        private void CategoryEditedHandler(CategoryEditedMessage obj)
        {
            if (PasswordEntry is null) return;

            var updateEntry = PasswordEntry.Category == obj.BaseCategory;
            var index = Categories.IndexOf(obj.BaseCategory);
            if(index != -1)
            {
                Categories[index] = obj.NewCategory;
            }

            if (updateEntry)
            {
                PasswordEntry.Category = obj.NewCategory;
                RaisePropertyChanged(nameof(PasswordEntry));
            }
        }

        private void CategoryDeletedHandler(CategoryDeletedMessage obj)
        {
            Categories.Remove(obj.Category);
        }
    }
}
