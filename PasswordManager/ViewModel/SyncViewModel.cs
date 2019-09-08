using System;
using System.IO;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PasswordManager.Events;
using PasswordManager.Messengers;
using PasswordManager.Model;
using PasswordManager.Repository.Interfaces;
using PasswordManager.Service.Interfaces;

namespace PasswordManager.ViewModel
{
    public class SyncViewModel : BaseViewModel
    {
        private ISyncService syncService;

        private IDatabaseRepository databaseRepository;

        private string username;
        /// <summary>
        /// Username of the storage account
        /// </summary>
        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                username = value;
                RaisePropertyChanged(nameof(Username));
                RaisePropertyChanged(nameof(IsSyncEnabled));
            }
        }

        private string password;
        /// <summary>
        /// Password of the storage account
        /// </summary>
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
                RaisePropertyChanged(nameof(Password));
                RaisePropertyChanged(nameof(IsSyncEnabled));
            }
        }

        private bool showPassword;
        /// <summary>
        /// Should display password as plaintext or confidentiality dots
        /// </summary>
        public bool ShowPassword
        {
            get
            {
                return showPassword;
            }
            set
            {
                showPassword = value;
                RaisePropertyChanged(nameof(ShowPassword));
            }
        }

        private string error;
        /// <summary>
        /// Sync errors
        /// </summary>
        public string Error
        {
            get
            {
                return error;
            }
            set
            {
                error = value;
                RaisePropertyChanged(nameof(Error));
            }
        }

        /// <summary>
        /// Indicates wether the sync can be performed
        /// </summary>
        public bool IsSyncEnabled
        {
            get
            {
                return !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password) && !SyncInProgress;
            }
        }

        private bool syncInProgress;
        /// <summary>
        /// Indicates wether a sync operation is in progress
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
                RaisePropertyChanged(nameof(IsSyncEnabled));
            }
        }

        private bool showSyncSteps;
        /// <summary>
        /// Show the sync steps section
        /// </summary>
        public bool ShowSyncSteps
        {
            get
            {
                return showSyncSteps;
            }
            set
            {
                showSyncSteps = value;
                RaisePropertyChanged(nameof(ShowSyncSteps));
            }
        }

        private SyncStepStates downloadSyncStepState;
        /// <summary>
        /// First sync step
        /// </summary>
        public SyncStepStates DownloadSyncStepState
        {
            get
            {
                return downloadSyncStepState;
            }
            set
            {
                downloadSyncStepState = value;
                RaisePropertyChanged(nameof(DownloadSyncStepState));
            }
        }

        private SyncStepStates mergeSyncStepState;
        /// <summary>
        /// Second sync step
        /// </summary>
        public SyncStepStates MergeSyncStepState
        {
            get
            {
                return mergeSyncStepState;
            }
            set
            {
                mergeSyncStepState = value;
                RaisePropertyChanged(nameof(MergeSyncStepState));
            }
        }

        private SyncStepStates uploadSyncStepState;
        /// <summary>
        /// Third and last sync step
        /// </summary>
        public SyncStepStates UploadSyncStepState
        {
            get
            {
                return uploadSyncStepState;
            }
            set
            {
                uploadSyncStepState = value;
                RaisePropertyChanged(nameof(UploadSyncStepState));
            }
        }

        public RelayCommand CloseCommand
        {
            get
            {
                return new RelayCommand(() => Messenger.Default.Send(new ToggleSyncViewMessage(this, false)));
            }
        }

        public RelayCommand StartSyncCommand
        {
            get
            {
                return new RelayCommand(StartSync);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="translationService"></param>
        /// <param name="syncService"></param>
        /// <param name="databaseRepository"></param>
        public SyncViewModel(ITranslationService translationService,
                             ISyncService syncService,
                             IDatabaseRepository databaseRepository)
            : base(translationService)
        {
            this.syncService = syncService;
            this.databaseRepository = databaseRepository;

            Messenger.Default.Register<DatabaseLoadedMessage>(this, InitUserControl);
            Messenger.Default.Register<ToggleSyncViewMessage>(this, ToggleSyncView);
        }

        /// <summary>
        /// Initiate the SyncView with stored values
        /// </summary>
        /// <param name="obj"></param>
        private void InitUserControl(DatabaseLoadedMessage obj)
        {
            Username = obj.DatabaseModel.SyncData.Username;
            Password = obj.DatabaseModel.SyncData.Password;
            ShowPassword = false;
            Error = string.Empty;
            if(!(obj.Sender is SyncViewModel))
            {
                // Don't reset if the opening was sent from the sync process
                ShowSyncSteps = false;
                DownloadSyncStepState = SyncStepStates.Inactive;
                MergeSyncStepState = SyncStepStates.Inactive;
                UploadSyncStepState = SyncStepStates.Inactive;
            }
        }

        /// <summary>
        /// Hide the sync steps
        /// </summary>
        /// <param name="obj"></param>
        private void ToggleSyncView(ToggleSyncViewMessage obj)
        {
            ShowPassword = false;
            Error = string.Empty;
            ShowSyncSteps = false;
        }

        /// <summary>
        /// Start the sync process
        /// </summary>
        private void StartSync()
        {
            ShowSyncSteps = true;
            SyncInProgress = true;
            DownloadSyncStepState = SyncStepStates.InProgress;
            MergeSyncStepState = SyncStepStates.Inactive;
            UploadSyncStepState = SyncStepStates.Inactive;
            Error = string.Empty;
            syncService.DatabaseDownloadEnded += DatabaseDownloadEndedHandler;
            syncService.DownloadDatabase(Username, Password, Constants.TempDatabaseFileName);
        }

        /// <summary>
        /// Database download ended event handler, move to the next phase if it was a success
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void DatabaseDownloadEndedHandler(object sender, DatabaseDownloadEndedEventArgs args)
        {
            syncService.DatabaseDownloadEnded -= DatabaseDownloadEndedHandler;

            Error = args.Error;
            if (!args.Result)
            {
                SyncInProgress = false;
                DownloadSyncStepState = SyncStepStates.Failed;
                return;
            }

            var incomingDatabase = databaseRepository.GetDatabase(args.Path);
            if(incomingDatabase is null)
            {
                Error = "CorruptedSyncDatabase";
                SyncInProgress = false;
                DownloadSyncStepState = SyncStepStates.Failed;
                return;
            }

            DownloadSyncStepState = SyncStepStates.Done;

            if (databaseRepository.GetDatabase().SyncData.LastSync == DateTime.MinValue || incomingDatabase.SyncData.LastSync > databaseRepository.GetDatabase().SyncData.LastSync)
            {
                syncService.DatabasesMerged += DatabasesMergedHandler;
                syncService.MergeDatabases(databaseRepository.GetDatabase(), incomingDatabase);
                MergeSyncStepState = SyncStepStates.InProgress;
            }
            else if (databaseRepository.GetDatabase().SyncData.LastSync != incomingDatabase.SyncData.LastSync)
            {
                // Just upload because only local changes
                syncService.DatabaseUploadEnded += DatabaseUploadEndedHandler;
                syncService.UploadDatabase(Username, Password, databaseRepository.GetDatabase().Path);
                MergeSyncStepState = SyncStepStates.Skipped;
                UploadSyncStepState = SyncStepStates.InProgress;
            }
            else
            {
                Error = "DatabaseAlreadyUpToDate";
                SyncInProgress = false;
                MergeSyncStepState = SyncStepStates.Skipped;
                UploadSyncStepState = SyncStepStates.Skipped;
            }

            File.Delete(args.Path);
        }

        /// <summary>
        /// Database merged event handler, move to the next phase if it was a success
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void DatabasesMergedHandler(object sender, DatabasesMergedEventArgs args)
        {
            syncService.DatabasesMerged -= DatabasesMergedHandler;

            args.MergedDatabase.SyncData = new SyncData()
            {
                Username = Username,
                Password = Password,
                LastSync = DateTime.Now
            };
            databaseRepository.LoadDatabase(args.MergedDatabase);
            databaseRepository.WriteDatabase();

            Application.Current.Dispatcher.Invoke(() => Messenger.Default.Send(new DatabaseLoadedMessage(this, args.MergedDatabase)));

            syncService.DatabaseUploadEnded += DatabaseUploadEndedHandler;
            syncService.UploadDatabase(Username, Password, args.MergedDatabase.Path);
            MergeSyncStepState = SyncStepStates.Done;
            UploadSyncStepState = SyncStepStates.InProgress;
        }

        /// <summary>
        /// Database upload ended event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void DatabaseUploadEndedHandler(object sender, DatabaseUploadEndedEventArgs args)
        {
            syncService.DatabaseUploadEnded -= DatabaseUploadEndedHandler;

            UploadSyncStepState = args.Result ? SyncStepStates.Done : SyncStepStates.Failed;
            Error = args.Error;
            SyncInProgress = false;
        }
    }
}
