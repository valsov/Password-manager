using Newtonsoft.Json;
using System.Collections.Generic;

namespace PasswordManager.Model
{
    public class DatabaseModel
    {
        public string Path { get; set; }

        public string Name { get; set; }

        public List<PasswordEntryModel> PasswordEntries { get; set; }

        public List<string> Categories { get; set; }

        [JsonIgnore]
        public string MainPassword { get; set; }
    }
}
