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

        private bool userControlVisibility;
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

        private bool entryDataVisibility;
        public bool EntryDataVisibility
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

        private bool deleteButtonVisibility;
        public bool DeleteButtonVisibility
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

        private bool editionButtonVisibility;
        public bool EditionButtonVisibility
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

        private bool editionFormVisibility;
        public bool EditionFormVisibility
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

        private bool editionControlButtonsVisibility;
        public bool EditionControlButtonsVisibility
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

        private bool creationControlButtonsVisibility;
        public bool CreationControlButtonsVisibility
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

        private bool deletionConfimationVisibility;
        public bool DeletionConfimationVisibility
        {
            get
            {
                return deletionConfimationVisibility;
            }
            set
            {
                deletionConfimationVisibility = value;
                RaisePropertyChanged(nameof(DeletionConfimationVisibility));
            }
        }

        public RelayCommand StartEditionCommand { get; private set; }

        public RelayCommand ValidateEditionCommand { get; private set; }

        public RelayCommand CancelEditionCommand { get; private set; }

        public RelayCommand DeleteEntryCommand { get; private set; }

        public RelayCommand ValidateDeletionCommand { get; private set; }

        public RelayCommand CancelDeletionCommand { get; private set; }

        public RelayCommand CreateEntryCommand { get; private set; }

        public RelayCommand CancelEntryCreationCommand { get; private set; }

        public RelayCommand CopyPasswordCommand { get; private set; }

        public RelayCommand CopyUsernameCommand { get; private set; }

        public RelayCommand CopyWebsiteCommand { get; private set; }

        public EntryViewModel(IDatabaseRepository databaseRepository,
                              IPasswordService passwordService)
        {
            this.databaseRepository = databaseRepository;
            this.passwordService = passwordService;

            backupPasswordEntry = new PasswordEntryModel();
            Categories = new ObservableCollection<string>();
            UserControlVisibility = false;
            DeletionConfimationVisibility = false;

            Messenger.Default.Register<DatabaseUnloadedMessage>(this, DatabaseUnloadedHandler);
            Messenger.Default.Register<EntrySelectedMessage>(this, EntrySelectedHandler);
            Messenger.Default.Register<ShowNewEntryViewMessage>(this, StartEntryCreation);
            Messenger.Default.Register<CategoryAddedMessage>(this, HandleNewCategory);
            Messenger.Default.Register<CategoryDeletedMessage>(this, CategoryDeletedHandler);
            Messenger.Default.Register<CategoryEditedMessage>(this, CategoryEditedHandler);

            DeleteEntryCommand = new RelayCommand(DeleteEntry);
            ValidateDeletionCommand = new RelayCommand(ValidateDeletion);
            CancelDeletionCommand = new RelayCommand(CancelDeletion);
            StartEditionCommand = new RelayCommand(StartEdition);
            ValidateEditionCommand = new RelayCommand(ValidateEdition);
            CancelEditionCommand = new RelayCommand(CancelEdition);
            CreateEntryCommand = new RelayCommand(CreateEntry);
            CancelEntryCreationCommand = new RelayCommand(CancelCreation);
            CopyPasswordCommand = new RelayCommand(() => CopyToClipboard(nameof(PasswordEntryModel.Password)));
            CopyUsernameCommand = new RelayCommand(() => CopyToClipboard(nameof(PasswordEntryModel.Username)));
            CopyWebsiteCommand = new RelayCommand(() => CopyToClipboard(nameof(PasswordEntryModel.Website)));
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

            UserControlVisibility = true;
            SetElementsVisibility(ViewModes.View);
        }

        private void DeleteEntry()
        {
            DeletionConfimationVisibility = true;
        }

        private void ValidateDeletion()
        {
            databaseRepository.DeletePasswordEntry(PasswordEntry);
            Messenger.Default.Send(new EntryDeletedMessage(this, PasswordEntry));
            UserControlVisibility = false;
            DeletionConfimationVisibility = false;
        }

        private void CancelDeletion()
        {
            DeletionConfimationVisibility = false;
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
            UserControlVisibility = true;
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
            UserControlVisibility = false;
        }

        private void SetElementsVisibility(ViewModes mode)
        {
            switch (mode)
            {
                case ViewModes.View:
                    EntryDataVisibility = true;
                    DeleteButtonVisibility = true;
                    EditionButtonVisibility = true;
                    EditionFormVisibility = false;
                    EditionControlButtonsVisibility = false;
                    CreationControlButtonsVisibility = false;
                    break;
                case ViewModes.Edition:
                    EntryDataVisibility = false;
                    DeleteButtonVisibility = false;
                    EditionButtonVisibility = false;
                    EditionFormVisibility = true;
                    EditionControlButtonsVisibility = true;
                    CreationControlButtonsVisibility = false;
                    break;
                case ViewModes.Creation:
                    EntryDataVisibility = false;
                    DeleteButtonVisibility = false;
                    EditionButtonVisibility = false;
                    EditionFormVisibility = true;
                    EditionControlButtonsVisibility = false;
                    CreationControlButtonsVisibility = true;
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

        private void CopyToClipboard(string property)
        {
            PasswordEntry.CopyDataToClipboard(property);
        }
    }
}
