using PasswordManager.Repository.Interfaces;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace PasswordManager.Repository
{
    /// <summary>
    /// Implementation of IIconsRepository interface, provides an access to the user's websites icons
    /// </summary>
    public class IconsRepository : IIconsRepository
    {
        private Dictionary<string, BitmapSource> cache;

        public IconsRepository()
        {
            cache = new Dictionary<string, BitmapSource>();
        }

        /// <summary>
        /// Associate a BitmapSource to an hostname and store them in memory
        /// </summary>
        /// <param name="source"></param>
        /// <param name="hostname"></param>
        public void AddIcon(BitmapSource source, string hostname)
        {
            if (cache.ContainsKey(hostname))
            {
                cache[hostname] = source;
            }
            else
            {
                cache.Add(hostname, source);
            }
        }

        /// <summary>
        /// Get the BitmapSource associated with the given hostname
        /// </summary>
        /// <param name="hostname"></param>
        /// <returns></returns>
        public BitmapSource GetIcon(string hostname)
        {
            if(cache.TryGetValue(hostname, out BitmapSource source))
            {
                return source;
            }

            return null;
        }

        /// <summary>
        /// Verify the an entry with the given hostname exists in the repository
        /// </summary>
        /// <param name="hostname"></param>
        /// <returns></returns>
        public bool CheckIconExists(string hostname)
        {
            return cache.ContainsKey(hostname);
        }

        /// <summary>
        /// Empty the repository
        /// </summary>
        public void UnloadIcons()
        {
            cache.Clear();
        }
    }
}
