using PasswordManager.Model;
using PasswordManager.Service.Interfaces;
using System;
using System.Linq;

namespace PasswordManager.Service
{
    /// <summary>
    /// Implementation of IPasswordService interface, service handling password generation and management
    /// </summary>
    public class PasswordService : IPasswordService
    {
        const string SPECIAL_CHARS = "&'(-_)=^$£+°µ%§/.?,;:!€@¨²<>";

        const string NUMBER_CHARS = "1234567890";

        const string ALPHA_CHARS = "azertyuiopqsdfghjklmwxcvbn";

        /// <summary>
        /// Generate a password with the given characteristics
        /// </summary>
        /// <param name="type"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public string GeneratePassword(PasswordTypes type, int length)
        {
            var chars = string.Empty;

            // goto keyword is mandatory to fall from one case to another
            switch (type)
            {
                case PasswordTypes.Full:
                    chars += SPECIAL_CHARS;
                    goto case PasswordTypes.AlphaAndNum;
                case PasswordTypes.AlphaAndNum:
                    chars += NUMBER_CHARS;
                    goto case PasswordTypes.Alpha;
                case PasswordTypes.Alpha:
                    chars += ALPHA_CHARS;
                    break;
            }

            return GenerateRandomSequence(length, chars);
        }

        /// <summary>
        /// Compute the strength level of the given string as a password
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public PasswordStrength CheckPasswordStrength(string password)
        {
            int score = 0;

            if (password is null) return PasswordStrength.Blank;

            if (password.Length < 1)
                return PasswordStrength.Blank;
            if (password.Length < 8)
                return PasswordStrength.VeryWeak;

            if (password.Length >= 8)
                score++;
            if (password.Length >= 12)
                score++;
            if (password.Any(x => char.IsDigit(x)))
                score++;
            if (password.Any(x => char.IsLower(x))
             && password.Any(x => char.IsUpper(x)))
                score++;
            if (password.Any(x => !char.IsLetterOrDigit(x)))
                score++;

            return (PasswordStrength)score;
        }

        /// <summary>
        /// Generate a string of the rgiven length based on a character list
        /// </summary>
        /// <param name="length"></param>
        /// <param name="chars"></param>
        /// <returns></returns>
        string GenerateRandomSequence(int length, string chars)
        {
            var password = "";
            var random = new Random();

            for (int i = 0; i < length; i++)
            {
                var character = chars[random.Next(0, chars.Length)];

                // If the random char is an alphabet character
                if (ALPHA_CHARS.Contains(character))
                {
                    if (random.Next(0, 2) == 0)
                    {
                        // Random uppercase transform
                        character = char.ToUpper(character);
                    }
                }

                password += character;
            }

            return password;
        }
    }
}
