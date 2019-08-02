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
                        Description = "Is in first cat",
                        Password = "Password01!",
                        Username = "aze",
                        Category = firstCat
                    },
                    new PasswordEntryModel()
                    {
                        Id = System.Guid.NewGuid().ToString(),
                        Name = "Seonc entry",
                        Description = "Is in first cat",
                        Password = "Password02!",
                        Username = "ert",
                        Category = firstCat
                    },
                    new PasswordEntryModel()
                    {
                        Id = System.Guid.NewGuid().ToString(),
                        Name = "Third one here",
                        Description = "Is in second cat",
                        Password = "Password03!",
                        Username = "azerazer",
                        Category = secondCat
                    },
                    new PasswordEntryModel()
                    {
                        Id = System.Guid.NewGuid().ToString(),
                        Name = "new entry !!",
                        Description = "Is in second cat",
                        Password = "Password04!",
                        Username = "fsdfsdfsd",
                        Category = secondCat
                    },
                    new PasswordEntryModel()
                    {
                        Id = System.Guid.NewGuid().ToString(),
                        Name = "Beforethelast",
                        Description = "Is in second cat",
                        Password = "Password05!",
                        Username = "rthrth",
                        Category = secondCat
                    },
                    new PasswordEntryModel()
                    {
                        Id = System.Guid.NewGuid().ToString(),
                        Name = "Last one :)",
                        Description = "Is in NO cat",
                        Password = "Password06!",
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
