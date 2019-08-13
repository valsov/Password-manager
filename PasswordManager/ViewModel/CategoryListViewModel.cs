using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PasswordManager.Messengers;
using PasswordManager.Repository.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PasswordManager.ViewModel
{
    public class CategoryListViewModel : ViewModelBase
    {
        private IDatabaseRepository databaseRepository;

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

        public RelayCommand ShowNewCategoryFormCommand { get; private set; }

        public RelayCommand CancelAddCategoryCommand { get; private set; }

        public RelayCommand AddCategoryCommand { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public CategoryListViewModel(IDatabaseRepository databaseRepository)
        {
            this.databaseRepository = databaseRepository;
            Messenger.Default.Register<DatabaseLoadedMessage>(this, DatabaseLoadedHandler);
            CategoryList = new ObservableCollection<string>();
            SelectCategoryCommand = new RelayCommand<string>(SelectCategory);
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
            foreach (var cat in obj.DatabaseModel.Categories)
            {
                CategoryList.Add(cat);
            }
        }

        /// <summary>
        /// Set SelectedCategory
        /// </summary>
        /// <param name="selectedCategory"></param>
        private void SelectCategory(string selectedCategory)
        {
            SelectedCategory = CategoryList.FirstOrDefault(x => x == selectedCategory);
            Messenger.Default.Send(new CategorySelectedMessage(this, SelectedCategory));
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
