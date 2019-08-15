using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PasswordManager.Messengers;
using PasswordManager.Repository.Interfaces;
using System;
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

        private string originalCategoryInEdition;
        public string CategoryInEdition { get; set; }

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

        private Visibility categoryEditionFormVisibility;
        public Visibility CategoryEditionFormVisibility
        {
            get
            {
                return categoryEditionFormVisibility;
            }
            set
            {
                categoryEditionFormVisibility = value;
                RaisePropertyChanged(nameof(CategoryEditionFormVisibility));
            }
        }

        /// <summary>
        /// Command to select a category and filter password entries by this criteria
        /// </summary>
        public RelayCommand<string> SelectCategoryCommand { get; private set; }

        public RelayCommand ShowNewCategoryFormCommand { get; private set; }

        public RelayCommand CancelAddCategoryCommand { get; private set; }

        public RelayCommand AddCategoryCommand { get; private set; }

        public RelayCommand<string> EditCategoryCommand { get; private set; }

        public RelayCommand<string> DeleteCategoryCommand { get; private set; }

        public RelayCommand CancelCategoryEditionCommand { get; private set; }

        public RelayCommand ValidateCategoryEditionCommand { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public CategoryListViewModel(IDatabaseRepository databaseRepository)
        {
            this.databaseRepository = databaseRepository;
            Messenger.Default.Register<DatabaseLoadedMessage>(this, DatabaseLoadedHandler);
            Messenger.Default.Register<DatabaseUnloadedMessage>(this, DatabaseUnloadedHandler);
            CategoryList = new ObservableCollection<string>();
            SelectCategoryCommand = new RelayCommand<string>(SelectCategory);
            ShowNewCategoryFormCommand = new RelayCommand(() => ToggleNewCategoryFormVisibility(true));
            CancelAddCategoryCommand = new RelayCommand(StopAddCategory);
            AddCategoryCommand = new RelayCommand(AddCategory);
            EditCategoryCommand = new RelayCommand<string>(EditCategory);
            DeleteCategoryCommand = new RelayCommand<string>(DeleteCategory);
            CancelCategoryEditionCommand = new RelayCommand(CancelCategoryEdition);
            ValidateCategoryEditionCommand = new RelayCommand(ValidateCategoryEdition);
            SelectedCategory = null;
            NewCategoryButtonVisibility = Visibility.Visible;
            NewCategoryFormVisibility = Visibility.Hidden;
            CategoryEditionFormVisibility = Visibility.Hidden;
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

        private void DatabaseUnloadedHandler(DatabaseUnloadedMessage obj)
        {
            CategoryList.Clear();
            SelectedCategory = null;
            CategoryInEdition = null;
            originalCategoryInEdition = null;
            ToggleNewCategoryFormVisibility(false);
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

        private void DeleteCategory(string category)
        {
            if(SelectedCategory == category)
            {
                SelectCategory(null);
            }
            CategoryList.Remove(category);
            databaseRepository.DeleteCategory(category);
            Messenger.Default.Send(new CategoryDeletedMessage(this, category));
        }

        private void EditCategory(string category)
        {
            originalCategoryInEdition = category;
            CategoryInEdition = category;
            RaisePropertyChanged(nameof(CategoryInEdition));
            CategoryEditionFormVisibility = Visibility.Visible;
        }

        private void ValidateCategoryEdition()
        {
            var index = CategoryList.IndexOf(originalCategoryInEdition);
            CategoryList[index] = CategoryInEdition;
            databaseRepository.UpdateCategory(originalCategoryInEdition, CategoryInEdition);
            Messenger.Default.Send(new CategoryEditedMessage(this, originalCategoryInEdition, CategoryInEdition));
            if (SelectedCategory == originalCategoryInEdition)
            {
                SelectedCategory = CategoryInEdition;
            }
            CategoryEditionFormVisibility = Visibility.Hidden;
        }

        private void CancelCategoryEdition()
        {
            CategoryEditionFormVisibility = Visibility.Hidden;
        }
    }
}
