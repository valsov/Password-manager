using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PasswordManager.Extensions;
using PasswordManager.Messengers;
using PasswordManager.Model;
using PasswordManager.Repository.Interfaces;
using PasswordManager.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace PasswordManager.ViewModel
{
    public class EntryViewModel : ViewModelBase
    {
        IDatabaseRepository databaseRepository;

        IPasswordService passwordService;

        IIconsService iconsService;

        /// <summary>
        /// Password entry used to backup before an edition
        /// </summary>
        private PasswordEntryModel backupPasswordEntry;

        private PasswordEntryModel passwordEntry;
        /// <summary>
        /// Password entry being viewed / edited / created
        /// </summary>
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

        /// <summary>
        /// List of categories to be used
        /// </summary>
        public ObservableCollection<string> Categories { get; set; }

        private bool showPassword;
        /// <summary>
        /// Should display password as plaintext or confidentiality dots
        /// </summary>
        public bool ShowPassword
        {
            get
            {
                return showPassword;
            }
            set
            {
                showPassword = value;
                RaisePropertyChanged(nameof(ShowPassword));
            }
        }

        /// <summary>
        /// List of password types for password generation
        /// </summary>
        public IEnumerable<PasswordTypes> PasswordTypesList
        {
            get
            {
                return Enum.GetValues(typeof(PasswordTypes)).Cast<PasswordTypes>();
            }
        }

        private PasswordTypes selectedPasswordType;
        public PasswordTypes SelectedPasswordType
        {
            get
            {
                return selectedPasswordType;
            }
            set
            {
                selectedPasswordType = value;
                RaisePropertyChanged(nameof(SelectedPasswordType));
            }
        }

        private string passwordLength;
        public string PasswordLength
        {
            get
            {
                return passwordLength;
            }
            set
            {
                passwordLength = value;
                RaisePropertyChanged(nameof(PasswordLength));
            }
        }

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

        private bool passwordGenerationDialogVisibility;
        public bool PasswordGenerationDialogVisibility
        {
            get
            {
                return passwordGenerationDialogVisibility;
            }
            set
            {
                passwordGenerationDialogVisibility = value;
                RaisePropertyChanged(nameof(PasswordGenerationDialogVisibility));
            }
        }

        public RelayCommand StartEditionCommand
        {
            get
            {
                return new RelayCommand(StartEdition);
            }
        }

        public RelayCommand ValidateEditionCommand
        {
            get
            {
                return new RelayCommand(ValidateEdition);
            }
        }

        public RelayCommand CancelEditionCommand
        {
            get
            {
                return new RelayCommand(CancelEdition);
            }
        }

        public RelayCommand DeleteEntryCommand
        {
            get
            {
                return new RelayCommand(DeleteEntry);
            }
        }

        public RelayCommand ValidateDeletionCommand
        {
            get
            {
                return new RelayCommand(ValidateDeletion);
            }
        }

        public RelayCommand CancelDeletionCommand
        {
            get
            {
                return new RelayCommand(CancelDeletion);
            }
        }

        public RelayCommand CreateEntryCommand
        {
            get
            {
                return new RelayCommand(CreateEntry);
            }
        }

        public RelayCommand CancelEntryCreationCommand
        {
            get
            {
                return new RelayCommand(CancelCreation);
            }
        }

        public RelayCommand CopyPasswordCommand
        {
            get
            {
                return new RelayCommand(() => CopyToClipboard(nameof(PasswordEntryModel.Password)));
            }
        }

        public RelayCommand CopyUsernameCommand
        {
            get
            {
                return new RelayCommand(() => CopyToClipboard(nameof(PasswordEntryModel.Username)));
            }
        }

        public RelayCommand OpenWebsiteCommand
        {
            get
            {
                return new RelayCommand(OpenWebsite);
            }
        }

        public RelayCommand OpenPasswordGenerationDialogCommand
        {
            get
            {
                return new RelayCommand(OpenPasswordGenerationDialog);
            }
        }

        public RelayCommand GeneratePasswordCommand
        {
            get
            {
                return new RelayCommand(GeneratePassword);
            }
        }

        public RelayCommand CancelPasswordGenerationCommand
        {
            get
            {
                return new RelayCommand(CancelPasswordGeneration);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="databaseRepository"></param>
        /// <param name="passwordService"></param>
        public EntryViewModel(IDatabaseRepository databaseRepository,
                              IPasswordService passwordService,
                              IIconsService iconsService)
        {
            this.databaseRepository = databaseRepository;
            this.passwordService = passwordService;
            this.iconsService = iconsService;

            iconsService.IconDownloadedEvent += IconDownloadedEventHandler;

            backupPasswordEntry = new PasswordEntryModel();
            Categories = new ObservableCollection<string>();
            UserControlVisibility = false;
            DeletionConfimationVisibility = false;
            PasswordGenerationDialogVisibility = false;

            Messenger.Default.Register<DatabaseUnloadedMessage>(this, DatabaseUnloadedHandler);
            Messenger.Default.Register<EntrySelectedMessage>(this, EntrySelectedHandler);
            Messenger.Default.Register<ShowNewEntryViewMessage>(this, StartEntryCreation);
            Messenger.Default.Register<CategoryAddedMessage>(this, HandleNewCategory);
            Messenger.Default.Register<CategoryDeletedMessage>(this, CategoryDeletedHandler);
            Messenger.Default.Register<CategoryEditedMessage>(this, CategoryEditedHandler);
        }

        /// <summary>
        /// Reset the view and the database related ressources
        /// </summary>
        /// <param name="obj"></param>
        private void DatabaseUnloadedHandler(DatabaseUnloadedMessage obj)
        {
            Categories.Clear();
            backupPasswordEntry = null;
            PasswordEntry = null;
            SetElementsVisibility(ViewModes.View);
            UserControlVisibility = false;
        }

        /// <summary>
        /// Show the selected entry
        /// </summary>
        /// <param name="obj"></param>
        private void EntrySelectedHandler(EntrySelectedMessage obj)
        {
            AddCategories();
            PasswordEntry = obj.passwordEntry;

            UserControlVisibility = true;
            SetElementsVisibility(ViewModes.View);
        }

        /// <summary>
        /// Show the deletion confirmation dialog
        /// </summary>
        private void DeleteEntry()
        {
            DeletionConfimationVisibility = true;
        }

        /// <summary>
        /// Commit the deletion
        /// </summary>
        private void ValidateDeletion()
        {
            databaseRepository.DeletePasswordEntry(PasswordEntry);
            Messenger.Default.Send(new EntryDeletedMessage(this, PasswordEntry));
            UserControlVisibility = false;
            DeletionConfimationVisibility = false;
        }

        /// <summary>
        /// Hide the deletion confirmation dialog
        /// </summary>
        private void CancelDeletion()
        {
            DeletionConfimationVisibility = false;
        }

        /// <summary>
        /// Set the view mode to edition
        /// </summary>
        private void StartEdition()
        {
            backupPasswordEntry = PasswordEntry.Copy();
            PasswordEntry.PropertyChanged += PasswordPropertyChanged;
            SetElementsVisibility(ViewModes.Edition);
        }

        /// <summary>
        /// Commit the edition
        /// </summary>
        private void ValidateEdition()
        {
            // Copy to avoid same instance manipulation
            var copy = PasswordEntry.Copy();
            databaseRepository.UpdatePasswordEntry(copy);
            iconsService.DownloadIcon(copy.Website);
            Messenger.Default.Send(new EntryEditedMessage(this, copy));
            RaisePropertyChanged(nameof(PasswordEntry));
            SetElementsVisibility(ViewModes.View);
        }

        /// <summary>
        /// Reset the password model to the backup one and set the view mode to view
        /// </summary>
        private void CancelEdition()
        {
            PasswordEntry = backupPasswordEntry.Copy();
            PasswordEntry.PropertyChanged -= PasswordPropertyChanged;
            SetElementsVisibility(ViewModes.View);
        }

        /// <summary>
        /// Set the view mode to edition
        /// </summary>
        /// <param name="obj"></param>
        private void StartEntryCreation(ShowNewEntryViewMessage obj)
        {
            AddCategories();
            PasswordEntry = new PasswordEntryModel();
            PasswordEntry.PropertyChanged += PasswordPropertyChanged;
            PasswordEntry.Password = passwordService.GeneratePassword(PasswordTypes.Full, 20);
            UserControlVisibility = true;
            SetElementsVisibility(ViewModes.Creation);
        }

        /// <summary>
        /// Commit password entry creation
        /// </summary>
        private void CreateEntry()
        {
            var copy = PasswordEntry.Copy();
            databaseRepository.AddPasswordEntry(copy);
            iconsService.DownloadIcon(copy.Website);
            Messenger.Default.Send(new EntryAddedMessage(this, copy));
        }

        /// <summary>
        /// Hide the view
        /// </summary>
        private void CancelCreation()
        {
            UserControlVisibility = false;
        }

        /// <summary>
        /// Set view mode
        /// </summary>
        /// <param name="mode"></param>
        private void SetElementsVisibility(ViewModes mode)
        {
            ShowPassword = false;

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

        /// <summary>
        /// Fill categories combobox if this wasn't already done
        /// </summary>
        private void AddCategories()
        {
            if (Categories.Any())
            {
                return;
            }

            // string.Empty for no category
            Categories.Add(string.Empty);
            foreach (var category in databaseRepository.GetDatabase().Categories)
            {
                Categories.Add(category);
            }
        }

        /// <summary>
        /// Add new category in the categories combobox
        /// </summary>
        /// <param name="obj"></param>
        private void HandleNewCategory(CategoryAddedMessage obj)
        {
            if (Categories.Any())
            {
                Categories.Add(obj.NewCategory);
            }
        }

        /// <summary>
        /// Edit the category in the categories combobox
        /// </summary>
        /// <param name="obj"></param>
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

        /// <summary>
        /// Delete the category in the categories combobox
        /// </summary>
        /// <param name="obj"></param>
        private void CategoryDeletedHandler(CategoryDeletedMessage obj)
        {
            Categories.Remove(obj.Category);
        }

        /// <summary>
        /// Call the CopyDataToClipboard extension method
        /// </summary>
        /// <param name="property"></param>
        private void CopyToClipboard(string property)
        {
            PasswordEntry.CopyDataToClipboard(property);
        }

        /// <summary>
        /// Call the OpenWebsite extension method
        /// </summary>
        /// <param name="obj"></param>
        private void OpenWebsite()
        {
            PasswordEntry.OpenWebsite();
        }

        /// <summary>
        /// Setup and open the password generation dialog
        /// </summary>
        private void OpenPasswordGenerationDialog()
        {
            PasswordGenerationDialogVisibility = true;
            PasswordLength = "20";
            SelectedPasswordType = PasswordTypes.Full;
        }

        /// <summary>
        /// Close the password generation dialog
        /// </summary>
        private void CancelPasswordGeneration()
        {
            PasswordGenerationDialogVisibility = false;
        }

        /// <summary>
        /// Generate a password with the selected length and composition
        /// </summary>
        private void GeneratePassword()
        {
            if (string.IsNullOrWhiteSpace(PasswordLength)) return;

            PasswordEntry.Password = passwordService.GeneratePassword(SelectedPasswordType, int.Parse(PasswordLength));
            PasswordGenerationDialogVisibility = false;
        }

        /// <summary>
        /// PasswordEntry.Password property changed event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PasswordPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PasswordEntry.PasswordStrength = passwordService.CheckPasswordStrength(PasswordEntry.Password);
            RaisePropertyChanged(nameof(PasswordEntry));
        }

        /// <summary>
        /// When an icon is loaded, update the view to display it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IconDownloadedEventHandler(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(PasswordEntry));
        }
    }
}
