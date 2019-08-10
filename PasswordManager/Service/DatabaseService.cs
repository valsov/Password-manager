using PasswordManager.Model;
using PasswordManager.Repository.Interfaces;
using PasswordManager.Service.Interfaces;
using System.Collections.Generic;

namespace PasswordManager.Service
{
    public class DatabaseService : IDatabaseService
    {
        IDatabaseRepository databaseRepository;

        public DatabaseService(IDatabaseRepository databaseRepository)
        {
            this.databaseRepository = databaseRepository;
        }

        public DatabaseModel ReadDatabase(string path, string password)
        {
            return databaseRepository.LoadDatabase(path, password);
        }

        public bool WriteDatabase(DatabaseModel database)
        {
            return databaseRepository.WriteDatabase(database);
        }

        public DatabaseModel CreateDatabase(string path, string name, string password)
        {
            var firstCat = "First category";
            var secondCat = "Second category";
            var database = new DatabaseModel
            {
                Name = name,
                Path = path,
                Categories = new List<string>()
                {
                    firstCat,
                    secondCat,
                    "Third category",
                    "Last category"
                },
                PasswordEntries = new List<PasswordEntryModel>()
                {
                    new PasswordEntryModel()
                    {
                        Id = System.Guid.NewGuid().ToString(),
                        Name = "First entry",
                        Notes = "Is in first cat",
                        Password = "Password01!",
                        PasswordStrength = PasswordStrength.VeryStrong,
                        Username = "aze",
                        Category = firstCat
                    },
                    new PasswordEntryModel()
                    {
                        Id = System.Guid.NewGuid().ToString(),
                        Name = "Seonc entry",
                        Notes = "Is in first cat",
                        Password = "",
                        PasswordStrength = PasswordStrength.Blank,
                        Username = "ert",
                        Category = firstCat
                    },
                    new PasswordEntryModel()
                    {
                        Id = System.Guid.NewGuid().ToString(),
                        Name = "Third one here",
                        Notes = "Is in second cat",
                        Password = "Password03!",
                        PasswordStrength = PasswordStrength.VeryWeak,
                        Username = "azerazer",
                        Category = secondCat
                    },
                    new PasswordEntryModel()
                    {
                        Id = System.Guid.NewGuid().ToString(),
                        Name = "new entry !!",
                        Notes = "Is in second cat",
                        Password = "Password04!",
                        PasswordStrength = PasswordStrength.Weak,
                        Username = "fsdfsdfsd",
                        Category = secondCat
                    },
                    new PasswordEntryModel()
                    {
                        Id = System.Guid.NewGuid().ToString(),
                        Name = "Beforethelast",
                        Notes = "Is in second cat",
                        Password = "Password05!",
                        PasswordStrength = PasswordStrength.Medium,
                        Username = "rthrth",
                        Category = secondCat
                    },
                    new PasswordEntryModel()
                    {
                        Id = System.Guid.NewGuid().ToString(),
                        Name = "Last one :)",
                        Notes = "Is in NO cat",
                        Password = "Password06!",
                        PasswordStrength = PasswordStrength.Strong,
                        Username = "toto",
                        Category = null
                    }
                },
                MainPassword = password
            };

            var result = WriteDatabase(database);

            return result ? database : null;
        }
    }
}
