namespace PasswordManager.Service.Interfaces
{
    /// <summary>
    /// Service handling the data encryption and decryption
    /// </summary>
    public interface IEncryptionService
    {
        /// <summary>
        /// Convert the key to a byte array and store it into the repository
        /// </summary>
        /// <param name="key"></param>
        void Initialize(string key);

        /// <summary>
        /// Unload the key from the repository
        /// </summary>
        void Clear();

        /// <summary>
        /// Encrypt the given string
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        string Encrypt(string plainText);

        /// <summary>
        /// Encrypt the given byte array
        /// </summary>
        /// <param name="plainBytes"></param>
        /// <returns></returns>
        byte[] Encrypt(byte[] plainBytes);
        
        /// <summary>
        /// Decrypt the given string
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        string Decrypt(string cipherText);

        /// <summary>
        /// Decrypt the given byte array
        /// </summary>
        /// <param name="cypherBytes"></param>
        /// <returns></returns>
        byte[] Decrypt(byte[] cypherBytes);
    }
}
