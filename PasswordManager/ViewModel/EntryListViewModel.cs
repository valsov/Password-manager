using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PasswordManager.Messengers;
using PasswordManager.Model;
using PasswordManager.Repository.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PasswordManager.ViewModel
{
    public class EntryListViewModel : ViewModelBase
    {
        private IDatabaseRepository databaseRepository;

        /// <summary>
        /// Complete password entries list
        /// </summary>
        private List<PasswordEntryModel> basePasswordEntries;

        /// <summary>
        /// Password entries to be displayed, regarding the currently selected category
        /// </summary>
        public IEnumerable<PasswordEntryModel> PasswordEntryList
        {
            get
            {
                return basePasswordEntries.Where(x => (string.IsNullOrWhiteSpace(SelectedCategory) || x.Category == SelectedCategory)
                                                   && (string.IsNullOrWhiteSpace(SearchValue) || x.Name.ToLower().Contains(SearchValue.ToLower())));
            }
        }

        private PasswordEntryModel selectedPasswordEntry;
        /// <summary>
        /// Currently selected password entry
        /// </summary>
        public PasswordEntryModel SelectedPasswordEntry
        {
            get
            {
                return selectedPasswordEntry;
            }
            set
            {
                selectedPasswordEntry = value;
                RaisePropertyChanged(nameof(SelectedPasswordEntry));
            }
        }

        /// <summary>
        /// List of categories to be displayed
        /// </summary>
        public ObservableCollection<string> CategoryList { get; set; }

        private string selectedCategory;
        /// <summary>
        /// Currently selected category
        /// </summary>
        public string SelectedCategory
        {
            get
            {
                return selectedCategory;
            }
            set
            {
                selectedCategory = value;
                RaisePropertyChanged(nameof(SelectedCategory));
            }
        }

        private string searchValue;
        public string SearchValue
        {
            get
            {
                return searchValue;
            }
            set
            {
                searchValue = value;
                RaisePropertyChanged(nameof(PasswordEntryList));
            }
        }

        private string newCategoryName;
        public string NewCategoryName
        {
            get
            {
                return newCategoryName;
            }
            set
            {
                newCategoryName = value;
                RaisePropertyChanged(nameof(NewCategoryName));
            }
        }

        private Visibility newCategoryButtonVisibility;
        public Visibility NewCategoryButtonVisibility
        {
            get
            {
                return newCategoryButtonVisibility;
            }
            set
            {
                newCategoryButtonVisibility = value;
                RaisePropertyChanged(nameof(NewCategoryButtonVisibility));
            }
        }

        private Visibility newCategoryFormVisibility;
        public Visibility NewCategoryFormVisibility
        {
            get
            {
                return newCategoryFormVisibility;
            }
            set
            {
                newCategoryFormVisibility = value;
                RaisePropertyChanged(nameof(NewCategoryFormVisibility));
            }
        }

        /// <summary>
        /// Command to select a category and filter password entries by this criteria
        /// </summary>
        public RelayCommand<string> SelectCategoryCommand { get; private set; }

        /// <summary>
        /// Command to select an entry
        /// </summary>
        public RelayCommand<PasswordEntryModel> SelectEntryCommand { get; private set; }

        /// <summary>
        /// Command to open the entry creation view
        /// </summary>
        public RelayCommand AddEntryCommand { get; private set; }

        public RelayCommand ShowNewCategoryFormCommand { get; private set; }

        public RelayCommand CancelAddCategoryCommand { get; private set; }

        public RelayCommand AddCategoryCommand { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public EntryListViewModel(IDatabaseRepository databaseRepository)
        {
            this.databaseRepository = databaseRepository;
            Messenger.Default.Register<DatabaseLoadedMessage>(this, DatabaseLoadedHandler);
            Messenger.Default.Register<EntryEditedMessage>(this, EntryEditedHandler);
            Messenger.Default.Register<EntryAddedMessage>(this, EntryAddedHandler);
            basePasswordEntries = new List<PasswordEntryModel>();
            CategoryList = new ObservableCollection<string>();
            SelectCategoryCommand = new RelayCommand<string>(SelectCategory);
            SelectEntryCommand = new RelayCommand<PasswordEntryModel>(SelectEntry);
            AddEntryCommand = new RelayCommand(AddEntry);
            ShowNewCategoryFormCommand = new RelayCommand(() => ToggleNewCategoryFormVisibility(true));
            CancelAddCategoryCommand = new RelayCommand(StopAddCategory);
            AddCategoryCommand = new RelayCommand(AddCategory);
            SelectedCategory = null;
            NewCategoryButtonVisibility = Visibility.Visible;
            NewCategoryFormVisibility = Visibility.Hidden;
        }

        /// <summary>
        /// Database loaded message handler, load the database content
        /// </summary>
        /// <param name="obj"></param>
        void DatabaseLoadedHandler(DatabaseLoadedMessage obj)
        {
            foreach(var entry in obj.DatabaseModel.PasswordEntries)
            {
                basePasswordEntries.Add(entry);
            }
            RaisePropertyChanged(nameof(PasswordEntryList));

            foreach (var cat in obj.DatabaseModel.Categories)
            {
                CategoryList.Add(cat);
            }
        }

        /// <summary>
        /// Update the edited entry
        /// </summary>
        /// <param name="obj"></param>
        void EntryEditedHandler(EntryEditedMessage obj)
        {
            for (int i = 0; i < basePasswordEntries.Count; i++)
            {
                if(basePasswordEntries[i].Id == obj.Entry.Id)
                {
                    basePasswordEntries[i] = obj.Entry;
                    break;
                }
            }
            RaisePropertyChanged(nameof(PasswordEntryList));
        }

        /// <summary>
        /// Add the newly created entry to the entries list and select it
        /// </summary>
        /// <param name="obj"></param>
        void EntryAddedHandler(EntryAddedMessage obj)
        {
            basePasswordEntries.Add(obj.Entry);
            RaisePropertyChanged(nameof(PasswordEntryList));
            SelectEntry(obj.Entry);
        }

        /// <summary>
        /// Set SelectedCategory
        /// </summary>
        /// <param name="selectedCategory"></param>
        private void SelectCategory(string selectedCategory)
        {
            SelectedCategory = CategoryList.FirstOrDefault(x => x == selectedCategory);
            RaisePropertyChanged(nameof(PasswordEntryList));
        }

        /// <summary>
        /// Set SelectedPasswordEntry and send a new EntrySelectedMessage
        /// </summary>
        /// <param name="selectedPasswordEntry"></param>
        private void SelectEntry(PasswordEntryModel selectedPasswordEntry)
        {
            SelectedPasswordEntry = selectedPasswordEntry;
            Messenger.Default.Send(new EntrySelectedMessage(this, selectedPasswordEntry.Copy()));
        }

        /// <summary>
        /// Send a message to open the new entry view
        /// </summary>
        private void AddEntry()
        {
            Messenger.Default.Send(new ShowNewEntryViewMessage(this));
        }

        private void AddCategory()
        {
            if (string.IsNullOrWhiteSpace(NewCategoryName))
            {
                return;
            }
            databaseRepository.AddCategory(NewCategoryName);
            Messenger.Default.Send(new CategoryAddedMessage(this, NewCategoryName));
            CategoryList.Add(NewCategoryName);
            StopAddCategory();
        }

        private void StopAddCategory()
        {
            NewCategoryName = string.Empty;
            ToggleNewCategoryFormVisibility(false);
        }

        private void ToggleNewCategoryFormVisibility(bool editionMode)
        {
            if (editionMode)
            {
                NewCategoryButtonVisibility = Visibility.Hidden;
                NewCategoryFormVisibility = Visibility.Visible;
            }
            else
            {
                NewCategoryButtonVisibility = Visibility.Visible;
                NewCategoryFormVisibility = Visibility.Hidden;
            }
        }
    }
}
