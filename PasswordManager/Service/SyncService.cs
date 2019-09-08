using CG.Web.MegaApiClient;
using PasswordManager.Events;
using PasswordManager.Model;
using PasswordManager.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManager.Service
{
    /// <summary>
    /// Service handling the password database synchronisation, each function is async
    /// </summary>
    public class SyncService : ISyncService
    {
        /// <summary>
        /// Event published when a database download operation ended
        /// </summary>
        public event EventHandler<DatabaseDownloadEndedEventArgs> DatabaseDownloadEnded;

        /// <summary>
        /// Event published when a datases merge operation ended
        /// </summary>
        public event EventHandler<DatabasesMergedEventArgs> DatabasesMerged;

        /// <summary>
        /// Event published when a database upload operation ended
        /// </summary>
        public event EventHandler<DatabaseUploadEndedEventArgs> DatabaseUploadEnded;

        /// <summary>
        /// Download a database from the given url to the specified path
        /// </summary>
        /// <param name="url"></param>
        /// <param name="path"></param>
        public void DownloadDatabaseFromUrl(string url, string path)
        {
            Task.Run(() => InternalDownloadDatabase(url, path));
        }

        /// <summary>
        /// Download the last edited database from the given account
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="path"></param>
        public void DownloadDatabase(string username, string password, string path)
        {
            Task.Run(() => InternalDownloadDatabase(null, path, username, password));
        }

        /// <summary>
        /// Merge the given databases into one
        /// </summary>
        /// <param name="baseModel"></param>
        /// <param name="incomingModel"></param>
        public void MergeDatabases(DatabaseModel baseModel, DatabaseModel incomingModel)
        {
            Task.Run(() => InternalMergeDatabases(baseModel, incomingModel));
        }

        /// <summary>
        /// Upload the file at the given path to the given account
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="path"></param>
        public void UploadDatabase(string username, string password, string path)
        {
            Task.Run(() => InternalUploadDatabase(username, password, path));
        }

        /// <summary>
        /// Perform the database download
        /// </summary>
        /// <param name="url"></param>
        /// <param name="path"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        private void InternalDownloadDatabase(string url, string path, string username = null, string password = null)
        {
            string error = null;
            var client = new MegaApiClient();
            try
            {
                INode database = null;
                if(username is null)
                {
                    client.LoginAnonymous();
                    database = client.GetNodesFromLink(new Uri(url)).Where(x => x.Name.EndsWith(".crypt"))
                                                                    .OrderByDescending(x => x.CreationDate)
                                                                    .FirstOrDefault();
                }
                else
                {
                    client.Login(username, password);
                    database = client.GetNodes().Where(x => x.Name != null && x.Name.EndsWith(".crypt"))
                                                .OrderByDescending(x => x.CreationDate)
                                                .FirstOrDefault();
                }
                
                if (database is null)
                {
                    error = "CantFindFile";
                }
                else
                {
                    using (var stream = client.Download(database))
                    {
                        using (var fileStream = File.Create(path))
                        {
                            stream.CopyTo(fileStream);
                        }
                    }
                }
            }
            catch (ApiException)
            {
                error = "ApiError";
            }
            catch (UriFormatException)
            {
                error = "InvalidUrl";
            }
            catch (ArgumentException)
            {
                error = "InvalidUrl";
            }
            catch (IOException)
            {
                error = "CantDownloadFile";
            }
            catch (Exception)
            {
                error = "ApiError";
            }
            finally
            {
                if (client.IsLoggedIn) client.Logout();
            }

            DatabaseDownloadEnded?.Invoke(this, new DatabaseDownloadEndedEventArgs(string.IsNullOrEmpty(error), path, error));
        }
        
        /// <summary>
        /// Perform the database merge
        /// </summary>
        /// <param name="baseModel"></param>
        /// <param name="incomingModel"></param>
        private void InternalMergeDatabases(DatabaseModel baseModel, DatabaseModel incomingModel)
        {
            var mergedDatabase = new DatabaseModel()
            {
                PasswordEntries = new List<PasswordEntryModel>(),
                Categories = new List<CategoryModel>(),
                DeletedEntries = new List<string>(),
                DeletedCategories = new List<string>()
            };

            ///
            /// Password Entries
            ///
            mergedDatabase.PasswordEntries.AddRange(MergeSyncEntries(baseModel.PasswordEntries.Cast<ISyncEntry>(),
                                                                     baseModel.DeletedEntries,
                                                                     incomingModel.PasswordEntries.Cast<ISyncEntry>(),
                                                                     incomingModel.DeletedEntries)
                                                                     .Cast<PasswordEntryModel>().ToList());

            ///
            /// Categories
            ///
            mergedDatabase.Categories.AddRange(MergeSyncEntries(baseModel.Categories.Cast<ISyncEntry>(),
                                                                baseModel.DeletedCategories,
                                                                incomingModel.Categories.Cast<ISyncEntry>(),
                                                                incomingModel.DeletedCategories)
                                                                .Cast<CategoryModel>().ToList());

            ///
            /// Deleted Entries
            ///
            mergedDatabase.DeletedEntries.AddRange(baseModel.DeletedEntries);
            mergedDatabase.DeletedEntries.AddRange(incomingModel.DeletedEntries);
            mergedDatabase.DeletedEntries = mergedDatabase.DeletedEntries.Distinct().ToList();

            ///
            /// Deleted categories
            ///
            mergedDatabase.DeletedCategories.AddRange(baseModel.DeletedCategories);
            mergedDatabase.DeletedCategories.AddRange(incomingModel.DeletedCategories);
            mergedDatabase.DeletedCategories = mergedDatabase.DeletedCategories.Distinct().ToList();

            ///
            /// Misc data
            ///
            mergedDatabase.Name = baseModel.Name;
            mergedDatabase.Path = baseModel.Path;

            DatabasesMerged?.Invoke(this, new DatabasesMergedEventArgs(mergedDatabase));
        }

        /// <summary>
        /// Merge two sync entries lists into one
        /// </summary>
        /// <param name="baseEntries"></param>
        /// <param name="deletedBaseEntries"></param>
        /// <param name="incomingEntries"></param>
        /// <param name="deletedIncomingEntries"></param>
        /// <returns></returns>
        private static List<ISyncEntry> MergeSyncEntries(IEnumerable<ISyncEntry> baseEntries, List<string> deletedBaseEntries, IEnumerable<ISyncEntry> incomingEntries, List<string> deletedIncomingEntries)
        {
            var mergedData = new List<ISyncEntry>();

            foreach (var baseEntry in baseEntries)
            {
                var correspondingIncomingEntry = incomingEntries.FirstOrDefault(x => x.Id == baseEntry.Id);
                if (correspondingIncomingEntry is null)
                {
                    if (deletedIncomingEntries.FirstOrDefault(x => x == baseEntry.Id) is null)
                    {
                        // added in baseDb
                        mergedData.Add(baseEntry);
                    }
                    continue;
                }

                var comparaison = DateTime.Compare(baseEntry.LastEditionDate, correspondingIncomingEntry.LastEditionDate);
                if (comparaison >= 0)
                {
                    mergedData.Add(baseEntry);
                }
                else
                {
                    mergedData.Add(correspondingIncomingEntry);
                }
            }

            foreach (var incomingEntry in incomingEntries)
            {
                var correspondingBaseEntry = baseEntries.Any(x => x.Id == incomingEntry.Id);
                if (!baseEntries.Any(x => x.Id == incomingEntry.Id) && !deletedBaseEntries.Any(x => x == incomingEntry.Id))
                {
                    // added in incomingDb
                    mergedData.Add(incomingEntry);
                }
            }

            return mergedData;
        }

        /// <summary>
        /// Perform the file upload to an account
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="path"></param>
        public void InternalUploadDatabase(string username, string password, string path)
        {
            string error = null;

            if (!File.Exists(path))
            {
                error = "LocalDatabaseFileNotFound";
                DatabaseUploadEnded?.Invoke(this, new DatabaseUploadEndedEventArgs(false, error));
                return;
            }

            var client = new MegaApiClient();
            try
            {
                client.Login(username, password);
                var nodes = client.GetNodes();
                // Delete previous sync files
                foreach (var node in nodes.Where(x => x.Name != null && x.Name.EndsWith(".crypt")))
                {
                    client.Delete(node, false);
                }

                // Upload new sync file
                var parentNode = nodes.FirstOrDefault(x => x.Type == NodeType.Root);
                if (parentNode is null)
                {
                    error = "InvalidUrl";
                }
                else
                {
                    client.UploadFile(path, parentNode);
                }
            }
            catch (ApiException)
            {
                error = "ApiError";
            }
            catch (UriFormatException)
            {
                error = "InvalidUrl";
            }
            catch (ArgumentException)
            {
                error = "InvalidUrl";
            }
            catch (IOException)
            {
                error = "CantUploadFile";
            }
            catch (Exception)
            {
                error = "ApiError";
            }
            finally
            {
                if (client.IsLoggedIn) client.Logout();
            }

            DatabaseUploadEnded?.Invoke(this, new DatabaseUploadEndedEventArgs(string.IsNullOrEmpty(error), error));
        }
    }
}
