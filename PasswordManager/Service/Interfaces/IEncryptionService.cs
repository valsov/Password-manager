namespace PasswordManager.Service.Interfaces
{
    /// <summary>
    /// Service handling the data encryption and decryption
    /// </summary>
    public interface IEncryptionService
    {
        /// <summary>
        /// Encrypt the given string with the given password
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="passPhrase"></param>
        /// <returns></returns>
        string Encrypt(string plainText, string passPhrase);
        
        /// <summary>
        /// Decrypt the given string with the given password
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="passPhrase"></param>
        /// <returns></returns>
        string Decrypt(string cipherText, string passPhrase);
    }
}
