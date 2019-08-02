/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:PasswordManager"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

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
            ///
            /// Reference : https://stackoverflow.com/questions/13795596/how-to-use-mvvmlight-simpleioc
            ///


            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            // Register Repositories
            SimpleIoc.Default.Register<ISettingsRepository, SettingsRepository>();
            SimpleIoc.Default.Register<IDatabaseRepository, DatabaseRepository>();

            // Register Services
            SimpleIoc.Default.Register<IDatabaseService, DatabaseService>();
            SimpleIoc.Default.Register<IEncryptionService, EncryptionService>();
            SimpleIoc.Default.Register<IPasswordService, PasswordService>();
            SimpleIoc.Default.Register<ISettingsService, SettingsService>();
            SimpleIoc.Default.Register<ISyncService, SyncService>();

            // Register ViewModels
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<DatabaseChallengeViewModel>();
            SimpleIoc.Default.Register<DatabaseCreationViewModel>();
            SimpleIoc.Default.Register<EntryListViewModel>();
            SimpleIoc.Default.Register<EntryViewModel>();
            SimpleIoc.Default.Register<NewEntryViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public DatabaseChallengeViewModel DatabaseChallenge
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DatabaseChallengeViewModel>();
            }
        }

        public DatabaseCreationViewModel DatabaseCreation
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DatabaseCreationViewModel>();
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

        public NewEntryViewModel NewEntry
        {
            get
            {
                return ServiceLocator.Current.GetInstance<NewEntryViewModel>();
            }
        }

        public SettingsViewModel Settings
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SettingsViewModel>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}
