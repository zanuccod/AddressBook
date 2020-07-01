using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AddressBook.API.Domains;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AddressBook.API.Models
{
    public class ContactDataModelSQLIte : IContactDataModel
    {
        private readonly string _dbConnection;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ContactDataModelSQLIte> _logger;

        public ContactDataModelSQLIte(IConfiguration configuration, ILogger<ContactDataModelSQLIte> logger)
        {
            _configuration = configuration;
            _logger = logger;

            _dbConnection = _configuration.GetConnectionString("SQLiteDbConnection");
            CreateDatabaseIfNotExists();
        }

        public ContactDataModelSQLIte(string dbConnection, ILogger<ContactDataModelSQLIte> logger)
        {
            _dbConnection = dbConnection;
            _logger = logger;

            CreateDatabaseIfNotExists();
        }

        public async Task<ICollection<Contact>> FindAllAsync()
        {
            using var conn = GetDbConnection();
            await conn.OpenAsync();

            var result = await conn.QueryAsync<Contact>("SELECT * FROM Contacts");
            return result.ToList();
        }

        public async Task<Contact> FindAsync(uint id)
        {
            using var conn = GetDbConnection();
            await conn.OpenAsync();

            return await conn.QueryFirstOrDefaultAsync<Contact>("SELECT * FROM Contacts WHERE id = @contactId", new { contactId = id });
        }

        public async Task<uint> InsertAsync(Contact item)
        {
            using var conn = GetDbConnection();
            await conn.OpenAsync();

            var sqlCmd = "INSERT INTO Contacts (Name, Surname, Nickname, PhoneNumber) VALUES (@Name, @Surname, @Nickname, @PhoneNumber);";
            sqlCmd += "SELECT MAX(Id) FROM Contacts;";

            return await conn.ExecuteScalarAsync<uint>(sqlCmd, item);
        }

        public async Task<uint> UpdateAsync(Contact item)
        {
            using var conn = GetDbConnection();
            await conn.OpenAsync();

            var sqlCmd = "UPDATE Contacts SET Name = @Name, Surname = @Surname, Nickname = @Nickname, PhoneNumber = @PhoneNumber WHERE id = @id";
            return (uint) await conn.ExecuteAsync(sqlCmd, item);
        }

        public async Task DeleteAsync(uint id)
        {
            using var conn = GetDbConnection();
            await conn.OpenAsync();

            await conn.ExecuteAsync("DELETE FROM Contacts WHERE id = @contactId", new { contactId = id });
        }

        private SQLiteConnection GetDbConnection()
        {
            return new SQLiteConnection($"Data Source={_dbConnection}");
        }

        private void CreateDatabaseIfNotExists()
        {
            _logger.LogInformation("Source database <{0}>", _dbConnection);
            if (File.Exists(_dbConnection))
                return;

            _logger.LogInformation("Database <{0}> not exists, creating it", _dbConnection);

            Directory.CreateDirectory("Data");
            SQLiteConnection.CreateFile(_dbConnection);

            using var conn = GetDbConnection();
            conn.Open();

            var contactsTable = "CREATE TABLE IF NOT EXISTS Contacts (";
            contactsTable += "Id integer PRIMARY KEY AUTOINCREMENT, ";
            contactsTable += "Name text NOT NULL, ";
            contactsTable += "Surname text NOT NULL, ";
            contactsTable += "Nickname text NULL, ";
            contactsTable += "PhoneNumber text NOT NULL)";

            conn.Execute(contactsTable);

            _logger.LogInformation("Database created");

        }
    }
}
