namespace PasswordManager.Repository.Interfaces
{
    /// <summary>
    /// Provides a storage for the current encryption/decryption key
    /// </summary>
    public interface ISecurityRepository
    {
        /// <summary>
        /// Store the key into the cache
        /// </summary>
        /// <param name="key"></param>
        void LoadKey(byte[] key);

        /// <summary>
        /// Destruct the key
        /// </summary>
        void UnloadKey();

        /// <summary>
        /// Get the key from the cache
        /// </summary>
        /// <returns></returns>
        byte[] GetKey();
    }
}
