﻿using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf.Transitions;
using Microsoft.Win32;
using PasswordManager.Events;
using PasswordManager.Messengers;
using PasswordManager.Service.Interfaces;
using System.IO;
using System.Windows;

namespace PasswordManager.ViewModel
{
    public class SyncOpeningViewModel : BaseViewModel
    {
        private ISyncService syncService;

        private string databasePath;
        /// <summary>
        /// Database file location, returns the filename only
        /// </summary>
        public string DatabasePath
        {
            get
            {
                return Path.GetFileName(databasePath);
            }
            set
            {
                databasePath = value;
                RaisePropertyChanged(nameof(DatabasePath));
                RaisePropertyChanged(nameof(IsSyncEnabled));
            }
        }

        private string url;
        /// <summary>
        /// Sync url
        /// </summary>
        public string Url
        {
            get
            {
                return url;
            }
            set
            {
                url = value;
                RaisePropertyChanged(nameof(Url));
                RaisePropertyChanged(nameof(IsSyncEnabled));
            }
        }

        /// <summary>
        /// Indicates wether the databasePath and the url properties is filled
        /// </summary>
        public bool IsSyncEnabled
        {
            get
            {
                return !string.IsNullOrEmpty(databasePath) && !string.IsNullOrEmpty(Url);
            }
        }

        private bool syncInProgress;
        /// <summary>
        /// Indicates if the file download is in progress
        /// </summary>
        public bool SyncInProgress
        {
            get
            {
                return syncInProgress;
            }
            set
            {
                syncInProgress = value;
                RaisePropertyChanged(nameof(SyncInProgress));
            }
        }

        private string error;
        /// <summary>
        /// Error message
        /// </summary>
        public string Error
        {
            get
            {
                return this[error];
            }
            set
            {
                error = value;
                RaisePropertyChanged(nameof(Error));
            }
        }

        public RelayCommand SelectDatabaseFileCommand
        {
            get
            {
                return new RelayCommand(SelectDatabaseFile);
            }
        }

        public RelayCommand SyncDatabaseCommand
        {
            get
            {
                return new RelayCommand(DownloadDatabase);
            }
        }

        public RelayCommand OpenDatabaseCreationViewCommand
        {
            get
            {
                return new RelayCommand(OpenDatabaseCreationView);
            }
        }

        public RelayCommand OpenDatabaseSelectionViewCommand
        {
            get
            {
                return new RelayCommand(() => OpenDatabaseSelectionView());
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="translationService"></param>
        /// <param name="syncService"></param>
        public SyncOpeningViewModel(ITranslationService translationService,
                                    ISyncService syncService)
            : base(translationService)
        {
            this.syncService = syncService;

            Messenger.Default.Register<ShowSyncOpeningViewMessage>(this, InitUserControl);
        }

        /// <summary>
        /// Setup the UserControl properties
        /// </summary>
        /// <param name="obj"></param>
        private void InitUserControl(ShowSyncOpeningViewMessage obj)
        {
            DatabasePath = string.Empty;
            Url = string.Empty;
            Error = string.Empty;
        }

        /// <summary>
        /// Open a file dialog to select a database file
        /// </summary>
        private void SelectDatabaseFile()
        {
            var fileDialog = new SaveFileDialog()
            {
                Title = this["ChooseDatabaseFile"],
                Filter = "Password Databases|*.crypt"
            };

            var result = fileDialog.ShowDialog();
            if (result == true)
            {
                DatabasePath = fileDialog.FileName;
            }
        }

        /// <summary>
        /// Call the syncService to download a file
        /// </summary>
        private void DownloadDatabase()
        {
            if (!IsSyncEnabled)
            {
                return;
            }

            SyncInProgress = true;
            syncService.DatabaseDownloadEnded += DatabaseDownloadEndedHandler;
            syncService.DownloadDatabaseFromUrl(Url, databasePath);
        }

        /// <summary>
        /// Display the eventual database download errors or switch to the database selection view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void DatabaseDownloadEndedHandler(object sender,  DatabaseDownloadEndedEventArgs args)
        {
            syncService.DatabaseDownloadEnded -= DatabaseDownloadEndedHandler;
            SyncInProgress = false;
            Error = args.Error;
            if (args.Result)
            {
                Application.Current.Dispatcher.Invoke(() => OpenDatabaseSelectionView(databasePath));
            }
        }

        /// <summary>
        /// Switch to the database creation view
        /// </summary>
        private void OpenDatabaseCreationView()
        {
            Transitioner.MovePreviousCommand.Execute(null, null);
            Messenger.Default.Send(new ShowDatabaseCreationViewMessage(this));
        }

        /// <summary>
        /// Switch to the database selection view
        /// </summary>
        /// <param name="path">Default value is empty string</param>
        private void OpenDatabaseSelectionView(string path = "")
        {
            Transitioner.MoveFirstCommand.Execute(null, null);
            Messenger.Default.Send(new ShowDatabaseSelectionViewMessage(this, path));
        }
    }
}
