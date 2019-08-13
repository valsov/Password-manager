using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PasswordManager.Extensions;
using PasswordManager.Messengers;
using PasswordManager.Model;
using System.Collections.Generic;
using System.Linq;

namespace PasswordManager.ViewModel
{
    public class EntryListViewModel : ViewModelBase
    {
        private string selectedCategory;

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
                return basePasswordEntries.Where(x => (string.IsNullOrWhiteSpace(selectedCategory) || x.Category == selectedCategory)
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

        /// <summary>
        /// Command to select an entry
        /// </summary>
        public RelayCommand<PasswordEntryModel> SelectEntryCommand { get; private set; }

        /// <summary>
        /// Command to open the entry creation view
        /// </summary>
        public RelayCommand AddEntryCommand { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public EntryListViewModel()
        {
            Messenger.Default.Register<DatabaseLoadedMessage>(this, DatabaseLoadedHandler);
            Messenger.Default.Register<EntryEditedMessage>(this, EntryEditedHandler);
            Messenger.Default.Register<EntryAddedMessage>(this, EntryAddedHandler);
            Messenger.Default.Register<CategorySelectedMessage>(this, CategorySelectedHandler);
            basePasswordEntries = new List<PasswordEntryModel>();
            SelectEntryCommand = new RelayCommand<PasswordEntryModel>(SelectEntry);
            AddEntryCommand = new RelayCommand(AddEntry);
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
        private void CategorySelectedHandler(CategorySelectedMessage obj)
        {
            selectedCategory = obj.SelectedCategory;
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
    }
}
