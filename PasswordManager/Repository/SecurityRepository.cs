using PasswordManager.Repository.Interfaces;

namespace PasswordManager.Repository
{
    /// <summary>
    /// Implementation of ISecurityRepository interface, provides a storage for the current encryption/decryption key
    /// </summary>
    public class SecurityRepository : ISecurityRepository
    {
        byte[] key;

        /// <summary>
        /// Store the key into the cache
        /// </summary>
        /// <param name="key"></param>
        public void LoadKey(byte[] key)
        {
            this.key = key;
        }

        /// <summary>
        /// Destruct the key
        /// </summary>
        public void UnloadKey()
        {
            for(int i = 0; i < key.Length; i++)
            {
                key[i] = 0;
            }
        }

        /// <summary>
        /// Get the key from the cache
        /// </summary>
        /// <returns></returns>
        public byte[] GetKey()
        {
            return key;
        }
    }
}
