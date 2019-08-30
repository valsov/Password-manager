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

        private string formError;
        /// <summary>
        /// Error label for category edition and creation
        /// </summary>
        public string FormError
        {
            get
            {
                return formError;
            }
            set
            {
                formError = value;
                RaisePropertyChanged(nameof(FormError));
            }
        }

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

        public RelayCommand<string> SelectCategoryCommand
        {
            get
            {
                return new RelayCommand<string>(SelectCategory);
            }
        }

        public RelayCommand ShowNewCategoryFormCommand
        {
            get
            {
                return new RelayCommand(() => NewCategoryFormVisibility = true);
            }
        }

        public RelayCommand CancelAddCategoryCommand
        {
            get
            {
                return new RelayCommand(StopAddCategory);
            }
        }

        public RelayCommand AddCategoryCommand
        {
            get
            {
                return new RelayCommand(AddCategory);
            }
        }

        public RelayCommand<string> EditCategoryCommand
        {
            get
            {
                return new RelayCommand<string>(EditCategory);
            }
        }

        public RelayCommand<string> DeleteCategoryCommand
        {
            get
            {
                return new RelayCommand<string>(DeleteCategory);
            }
        }

        public RelayCommand CancelCategoryEditionCommand
        {
            get
            {
                return new RelayCommand(CancelCategoryEdition);
            }
        }

        public RelayCommand ValidateCategoryEditionCommand
        {
            get
            {
                return new RelayCommand(ValidateCategoryEdition);
            }
        }

        public RelayCommand CancelCategoryDeletionCommand
        {
            get
            {
                return new RelayCommand(CancelCategoryDeletion);
            }
        }

        public RelayCommand ValidateCategoryDeletionCommand
        {
            get
            {
                return new RelayCommand(ValidateCategoryDeletion);
            }
        }

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
                FormError = "Category name is empty";
                return;
            }
            if(CategoryList.FirstOrDefault(x => x == NewCategoryName) != null)
            {
                FormError = "Category exists";
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
            FormError = string.Empty;
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
            if (string.IsNullOrWhiteSpace(CategoryInEdition))
            {
                FormError = "Category name is empty";
                return;
            }
            if (CategoryList.FirstOrDefault(x => x == CategoryInEdition) != null && CategoryInEdition != originalCategoryInEdition)
            {
                FormError = "Category exists";
                return;
            }

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
            FormError = string.Empty;
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
