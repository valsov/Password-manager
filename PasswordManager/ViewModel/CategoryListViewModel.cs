using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PasswordManager.Messengers;
using PasswordManager.Repository.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;

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
        /// <summary>
        /// Name of the category to be added
        /// </summary>
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

        /// <summary>
        /// Copy of the category name before edition
        /// </summary>
        private string originalCategoryInEdition;

        /// <summary>
        /// Name of the category currently being edited
        /// </summary>
        public string CategoryInEdition { get; set; }

        private bool newCategoryFormVisibility;
        /// <summary>
        /// Visibility of the form to create a new category
        /// </summary>
        public bool NewCategoryFormVisibility
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

        private bool categoryEditionFormVisibility;
        /// <summary>
        /// Visibility of the form to edit a category
        /// </summary>
        public bool CategoryEditionFormVisibility
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

        private string categoryToDelete;
        private bool categoryDeletionConfimationVisibility;
        /// <summary>
        /// Visibility of the category delete confirmation dialog
        /// </summary>
        public bool CategoryDeletionConfimationVisibility
        {
            get
            {
                return categoryDeletionConfimationVisibility;
            }
            set
            {
                categoryDeletionConfimationVisibility = value;
                RaisePropertyChanged(nameof(CategoryDeletionConfimationVisibility));
            }
        }

        public RelayCommand<string> SelectCategoryCommand { get; private set; }

        public RelayCommand ShowNewCategoryFormCommand { get; private set; }

        public RelayCommand CancelAddCategoryCommand { get; private set; }

        public RelayCommand AddCategoryCommand { get; private set; }

        public RelayCommand<string> EditCategoryCommand { get; private set; }

        public RelayCommand<string> DeleteCategoryCommand { get; private set; }

        public RelayCommand CancelCategoryEditionCommand { get; private set; }

        public RelayCommand ValidateCategoryEditionCommand { get; private set; }

        public RelayCommand CancelCategoryDeletionCommand { get; private set; }

        public RelayCommand ValidateCategoryDeletionCommand { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public CategoryListViewModel(IDatabaseRepository databaseRepository)
        {
            this.databaseRepository = databaseRepository;

            CategoryList = new ObservableCollection<string>();
            SelectedCategory = null;
            NewCategoryFormVisibility = false;
            CategoryEditionFormVisibility = false;
            CategoryDeletionConfimationVisibility = false;

            Messenger.Default.Register<DatabaseLoadedMessage>(this, DatabaseLoadedHandler);
            Messenger.Default.Register<DatabaseUnloadedMessage>(this, DatabaseUnloadedHandler);

            SelectCategoryCommand = new RelayCommand<string>(SelectCategory);
            ShowNewCategoryFormCommand = new RelayCommand(() => NewCategoryFormVisibility = true);
            CancelAddCategoryCommand = new RelayCommand(StopAddCategory);
            AddCategoryCommand = new RelayCommand(AddCategory);
            EditCategoryCommand = new RelayCommand<string>(EditCategory);
            CancelCategoryEditionCommand = new RelayCommand(CancelCategoryEdition);
            ValidateCategoryEditionCommand = new RelayCommand(ValidateCategoryEdition);
            DeleteCategoryCommand = new RelayCommand<string>(DeleteCategory);
            CancelCategoryDeletionCommand = new RelayCommand(CancelCategoryDeletion);
            ValidateCategoryDeletionCommand = new RelayCommand(ValidateCategoryDeletion);
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
        /// Empty all database related ressources
        /// </summary>
        /// <param name="obj"></param>
        private void DatabaseUnloadedHandler(DatabaseUnloadedMessage obj)
        {
            CategoryList.Clear();
            SelectedCategory = null;
            CategoryInEdition = null;
            originalCategoryInEdition = null;
            NewCategoryFormVisibility = false;
            CategoryDeletionConfimationVisibility = false;
            categoryToDelete = null;
        }

        /// <summary>
        /// Send a CategorySelectedMessage
        /// </summary>
        /// <param name="selectedCategory"></param>
        private void SelectCategory(string selectedCategory)
        {
            SelectedCategory = CategoryList.FirstOrDefault(x => x == selectedCategory);
            Messenger.Default.Send(new CategorySelectedMessage(this, SelectedCategory));
        }

        /// <summary>
        /// Add a new category
        /// </summary>
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

        /// <summary>
        /// Cancel the new category procedure
        /// </summary>
        private void StopAddCategory()
        {
            NewCategoryName = string.Empty;
            NewCategoryFormVisibility = false;
        }

        /// <summary>
        /// Setup the category edition form
        /// </summary>
        /// <param name="category"></param>
        private void EditCategory(string category)
        {
            originalCategoryInEdition = category;
            CategoryInEdition = category;
            RaisePropertyChanged(nameof(CategoryInEdition));
            CategoryEditionFormVisibility = true;
        }

        /// <summary>
        /// Commit the category edition
        /// </summary>
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
            CategoryEditionFormVisibility = false;
        }

        /// <summary>
        /// Hide the category edition form
        /// </summary>
        private void CancelCategoryEdition()
        {
            CategoryEditionFormVisibility = false;
        }

        /// <summary>
        /// Show the category delete confirmation dialog
        /// </summary>
        /// <param name="category"></param>
        private void DeleteCategory(string category)
        {
            categoryToDelete = category;
            CategoryDeletionConfimationVisibility = true;
        }

        /// <summary>
        /// Commit the category deletion
        /// </summary>
        private void ValidateCategoryDeletion()
        {
            if (SelectedCategory == categoryToDelete)
            {
                SelectCategory(null);
            }
            CategoryList.Remove(categoryToDelete);
            databaseRepository.DeleteCategory(categoryToDelete);
            Messenger.Default.Send(new CategoryDeletedMessage(this, categoryToDelete));
            CategoryDeletionConfimationVisibility = false;
        }

        /// <summary>
        /// Hide the category delete confirmation dialog
        /// </summary>
        private void CancelCategoryDeletion()
        {
            CategoryDeletionConfimationVisibility = false;
        }
    }
}
