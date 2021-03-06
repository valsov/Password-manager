﻿using PasswordManager.Repository.Interfaces;
using PasswordManager.Service.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PasswordManager.Service
{
    /// <summary>
    /// Service handling the data encryption and decryption
    /// </summary>
    public class EncryptionService : IEncryptionService
    {
        private ISecurityRepository securityRepository;

        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        const int KEYSIZE = 256;

        // This constant determines the number of iterations for the password bytes generation function.
        const int ITERATIONS = 1000;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="securityRepository"></param>
        public EncryptionService(ISecurityRepository securityRepository)
        {
            this.securityRepository = securityRepository;
        }

        /// <summary>
        /// Convert the key to a byte array and store it into the repository
        /// </summary>
        /// <param name="key"></param>
        public void Initialize(string key)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            securityRepository.LoadKey(keyBytes);
        }

        /// <summary>
        /// Unload the key from the repository
        /// </summary>
        public void Clear()
        {
            securityRepository.UnloadKey();
        }

        /// <summary>
        /// Encrypt the given string
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public string Encrypt(string plainText)
        {
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var cipherBytes = Encrypt(plainBytes);
            return Convert.ToBase64String(cipherBytes);
        }

        /// <summary>
        /// Encrypt the given byte array
        /// </summary>
        /// <param name="plainBytes"></param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] plainBytes)
        {
            var key = securityRepository.GetKey();

            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();

            using (var password = new Rfc2898DeriveBytes(key, saltStringBytes, ITERATIONS))
            {
                var keyBytes = password.GetBytes(KEYSIZE / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return cipherTextBytes;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Decrypt the given string
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public string Decrypt(string cipherText)
        {
            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            var plainBytes = Decrypt(cipherTextBytesWithSaltAndIv);
            return Encoding.UTF8.GetString(plainBytes);
        }

        /// <summary>
        /// Decrypt the given byte array
        /// </summary>
        /// <param name="cipherBytes"></param>
        /// <returns></returns>
        public byte[] Decrypt(byte[] cipherBytes)
        {
            var key = securityRepository.GetKey();

            // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
            var saltStringBytes = cipherBytes.Take(KEYSIZE / 8).ToArray();
            // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            var ivStringBytes = cipherBytes.Skip(KEYSIZE / 8).Take(KEYSIZE / 8).ToArray();
            // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            var cipherTextBytes = cipherBytes.Skip((KEYSIZE / 8) * 2).Take(cipherBytes.Length - ((KEYSIZE / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(key, saltStringBytes, ITERATIONS))
            {
                var keyBytes = password.GetBytes(KEYSIZE / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return plainTextBytes.Take(decryptedByteCount).ToArray();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generate a randomly filled byte[] of 256 bit
        /// </summary>
        /// <returns></returns>
        byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
    }
}
