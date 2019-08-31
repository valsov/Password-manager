using System.Collections.Generic;

namespace PasswordManager.Model
{
    public class TranslationModel
    {
        public Languages Language { get; set; }

        public Dictionary<string, string> TranslationBase { get; set; }
    }
}
