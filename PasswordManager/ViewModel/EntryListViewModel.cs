using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PasswordManager.Extensions;
using PasswordManager.Messengers;
using PasswordManager.Model;
using PasswordManager.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PasswordManager.ViewModel
{
    public class EntryListViewModel : ViewModelBase
    {
        private IIconsService iconsService;

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
        /// <summary>
        /// Value to search filter the password entries
        /// </summary>
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

        public RelayCommand<PasswordEntryModel> SelectEntryCommand
        {
            get
            {
                return new RelayCommand<PasswordEntryModel>(SelectEntry);
            }
        }

        public RelayCommand AddEntryCommand
        {
            get
            {
                return new RelayCommand(AddEntry);
            }
        }

        public RelayCommand<PasswordEntryModel> CopyPasswordCommand
        {
            get
            {
                return new RelayCommand<PasswordEntryModel>((obj) => CopyToClipboard(obj, nameof(PasswordEntryModel.Password)));
            }
        }

        public RelayCommand<PasswordEntryModel> CopyUsernameCommand
        {
            get
            {
                return new RelayCommand<PasswordEntryModel>((obj) => CopyToClipboard(obj, nameof(PasswordEntryModel.Username)));
            }
        }

        public RelayCommand<PasswordEntryModel> OpenWebsiteCommand
        {
            get
            {
                return new RelayCommand<PasswordEntryModel>(OpenWebsite);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public EntryListViewModel(IIconsService iconsService)
        {
            this.iconsService = iconsService;

            iconsService.IconDownloadedEvent += IconDownloadedEventHandler;

            basePasswordEntries = new List<PasswordEntryModel>();

            Messenger.Default.Register<DatabaseLoadedMessage>(this, DatabaseLoadedHandler);
            Messenger.Default.Register<DatabaseUnloadedMessage>(this, DatabaseUnloadedHandler);
            Messenger.Default.Register<EntryEditedMessage>(this, EntryEditedHandler);
            Messenger.Default.Register<EntryAddedMessage>(this, EntryAddedHandler);
            Messenger.Default.Register<EntryDeletedMessage>(this, EntryDeletedHandler);
            Messenger.Default.Register<CategorySelectedMessage>(this, CategorySelectedHandler);
            Messenger.Default.Register<CategoryDeletedMessage>(this, CategoryDeletedHandler);
            Messenger.Default.Register<CategoryEditedMessage>(this, CategoryEditedHandler);
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
        /// Empty all database related ressources
        /// </summary>
        /// <param name="obj"></param>
        private void DatabaseUnloadedHandler(DatabaseUnloadedMessage obj)
        {
            basePasswordEntries.Clear();
            selectedCategory = null;
            SearchValue = null;
            SelectedPasswordEntry = null;
        }

        /// <summary>
        /// Update the edited entry
        /// </summary>
        /// <param name="obj"></param>
        void EntryEditedHandler(EntryEditedMessage obj)
        {
            var index = 0;
            for (int i = 0; i < basePasswordEntries.Count; i++)
            {
                if (basePasswordEntries[i].Id == obj.Entry.Id)
                {
                    basePasswordEntries[i] = obj.Entry;
                    index = i;
                    break;
                }
            }
            RaisePropertyChanged(nameof(PasswordEntryList));
            SelectedPasswordEntry = basePasswordEntries[index];
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
        /// Remove the given entry from the list
        /// </summary>
        /// <param name="obj"></param>
        private void EntryDeletedHandler(EntryDeletedMessage obj)
        {
            basePasswordEntries.RemoveAll(x => x.Id == obj.Entry.Id);
            RaisePropertyChanged(nameof(PasswordEntryList));
        }

        /// <summary>
        /// Set SelectedCategory to filter the entry list
        /// </summary>
        /// <param name="selectedCategory"></param>
        private void CategorySelectedHandler(CategorySelectedMessage obj)
        {
            selectedCategory = obj.SelectedCategory;
            RaisePropertyChanged(nameof(PasswordEntryList));
        }

        /// <summary>
        /// Update the given category in the entry list objects
        /// </summary>
        /// <param name="obj"></param>
        private void CategoryEditedHandler(CategoryEditedMessage obj)
        {
            foreach (var entry in basePasswordEntries.Where(x => x.Category == obj.BaseCategory))
            {
                entry.Category = obj.NewCategory;
            }
        }

        /// <summary>
        /// Delete the category in the entry list objects
        /// </summary>
        /// <param name="obj"></param>
        private void CategoryDeletedHandler(CategoryDeletedMessage obj)
        {
            foreach(var entry in basePasswordEntries.Where(x => x.Category == obj.Category))
            {
                entry.Category = null;
            }
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
            SelectedPasswordEntry = null;
            Messenger.Default.Send(new ShowNewEntryViewMessage(this));
        }

        /// <summary>
        /// Call the CopyDataToClipboard extension method
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="property"></param>
        private void CopyToClipboard(PasswordEntryModel obj, string property)
        {
            obj.CopyDataToClipboard(property);
        }

        /// <summary>
        /// Call the OpenWebsite extension method
        /// </summary>
        /// <param name="obj"></param>
        private void OpenWebsite(PasswordEntryModel obj)
        {
            obj.OpenWebsite();
        }

        /// <summary>
        /// When an icon is loaded, update the view to display it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IconDownloadedEventHandler(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(PasswordEntryList));
        }
    }
}
