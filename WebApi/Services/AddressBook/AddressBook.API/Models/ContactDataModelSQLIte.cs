using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AddressBook.API.Domains;
using Dapper;

namespace AddressBook.API.Models
{
    public class ContactDataModelSQLIte : IContactDataModel
    {
        private readonly string _dbConnection;

        public ContactDataModelSQLIte()
        {
            _dbConnection = "Data/AddressBook.db";
            CreateDatabaseIfNotExists();
        }

        public ContactDataModelSQLIte(string dbConnection)
        {
            _dbConnection = dbConnection;
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

            var sqlCmd = "INSERT INTO Contacts (Name, Surname, Nickname, PhoneNumber) VALUES (@Name, @Surname, @Nickname, @PhoneNumber)";
            await conn.ExecuteAsync(sqlCmd, item);

            // because sqlLite dosn't support "SELECT CAST(SCOPE_IDENTITY() as int)" to get last inserted id
            return await conn.ExecuteScalarAsync<uint>("SELECT MAX(Id) FROM Contacts");
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
            if (File.Exists(_dbConnection))
                return;

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
        }
    }
}
