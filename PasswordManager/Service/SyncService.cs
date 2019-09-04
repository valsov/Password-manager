using CG.Web.MegaApiClient;
using PasswordManager.Events;
using PasswordManager.Service.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManager.Service
{
    /// <summary>
    /// Service handling the password database synchronisation
    /// </summary>
    public class SyncService : ISyncService
    {
        /// <summary>
        /// Event published when a database download operation ended
        /// </summary>
        public event EventHandler<DatabaseDownloadEndedEventArgs> DatabaseDownloadEnded;

        /// <summary>
        /// Download the newest file .crypt file from the given url, async
        /// </summary>
        /// <param name="url"></param>
        /// <param name="path"></param>
        public void DownloadDatabaseFromUrl(string url, string path)
        {
            Task.Run(() => InternalDownloadDatabseFromUrl(url, path));
        }

        /// <summary>
        /// Download a database file from the given Mega folder to the given path
        /// </summary>
        /// <param name="url"></param>
        /// <param name="path"></param>
        private void InternalDownloadDatabseFromUrl(string url, string path)
        {
            string error = null;
            var client = new MegaApiClient();
            try
            {
                client.LoginAnonymous();

                var database = client.GetNodesFromLink(new Uri(url)).Where(x => x.Name.EndsWith(".crypt"))
                                                                    .OrderByDescending(x => x.CreationDate)
                                                                    .FirstOrDefault();
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
            finally
            {
                client.Logout();
            }

            DatabaseDownloadEnded?.Invoke(this, new DatabaseDownloadEndedEventArgs(string.IsNullOrEmpty(error), path, error));
        }
    }
}
