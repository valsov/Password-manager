namespace PasswordManager.Service.Interfaces
{
    public interface IEncryptionService
    {
        string Encrypt(string plainText, string passPhrase);
        
        string Decrypt(string cipherText, string passPhrase);
    }
}
