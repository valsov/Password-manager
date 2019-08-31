using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using PasswordManager.Repository;
using PasswordManager.Repository.Interfaces;
using PasswordManager.Service;
using PasswordManager.Service.Interfaces;

namespace PasswordManager.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            // Register Repositories
            SimpleIoc.Default.Register<ISettingsRepository, SettingsRepository>();
            SimpleIoc.Default.Register<IDatabaseRepository, DatabaseRepository>();
            SimpleIoc.Default.Register<IIconsRepository, IconsRepository>();
            SimpleIoc.Default.Register<ISecurityRepository, SecurityRepository>();

            // Register Services
            SimpleIoc.Default.Register<IEncryptionService, EncryptionService>();
            SimpleIoc.Default.Register<IPasswordService, PasswordService>();
            SimpleIoc.Default.Register<ISettingsService, SettingsService>();
            SimpleIoc.Default.Register<ISyncService, SyncService>();
            SimpleIoc.Default.Register<IIconsService, IconsService>();
            SimpleIoc.Default.Register<IClipboardService, ClipboardService>();

            // Register ViewModels
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<DatabaseSelectionViewModel>();
            SimpleIoc.Default.Register<DatabaseCreationViewModel>();
            SimpleIoc.Default.Register<CategoryListViewModel>();
            SimpleIoc.Default.Register<EntryListViewModel>();
            SimpleIoc.Default.Register<EntryViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<SyncOpeningViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public DatabaseSelectionViewModel DatabaseSelection
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DatabaseSelectionViewModel>();
            }
        }

        public DatabaseCreationViewModel DatabaseCreation
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DatabaseCreationViewModel>();
            }
        }

        public CategoryListViewModel CategoryList
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CategoryListViewModel>();
            }
        }

        public EntryListViewModel EntryList
        {
            get
            {
                return ServiceLocator.Current.GetInstance<EntryListViewModel>();
            }
        }

        public EntryViewModel Entry
        {
            get
            {
                return ServiceLocator.Current.GetInstance<EntryViewModel>();
            }
        }

        public SettingsViewModel Settings
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SettingsViewModel>();
            }
        }

        public SyncOpeningViewModel SyncOpening
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SyncOpeningViewModel>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}
