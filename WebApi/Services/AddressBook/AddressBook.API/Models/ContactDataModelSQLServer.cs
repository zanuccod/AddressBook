using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AddressBook.API.Domains;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AddressBook.API.Models
{
    public class ContactDataModelSQLServer : IContactDataModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ContactDataModelSQLServer> _logger;
        private readonly SqlConnectionStringBuilder _connectionString;

        #region Constructors

        public ContactDataModelSQLServer(IConfiguration configuration, ILogger<ContactDataModelSQLServer> logger)
        {
            _configuration = configuration;
            _logger = logger;

            _connectionString = new SqlConnectionStringBuilder(_configuration.GetConnectionString("SQLServerDbConnection"));
            CreateDatabaseIfNotExists();
        }

        public ContactDataModelSQLServer(string dbconnection, ILogger<ContactDataModelSQLServer> logger)
        {
            _connectionString = new SqlConnectionStringBuilder(dbconnection);
            _logger = logger;

            CreateDatabaseIfNotExists();
        }

        #endregion

        #region Public Methods

        public void CreateDatabaseIfNotExists()
        {
            _logger.LogInformation("Source database <{0}>", _connectionString.ConnectionString);

            var targetDatabase = _connectionString.InitialCatalog;

            // change to master database to check if targetDatabase exsits and if not to create it
            _connectionString.InitialCatalog = "master";
            using var conn = GetDbConnection();
            conn.Open();

            var isTargetDatabase = conn.ExecuteScalar<string>($"SELECT * FROM sys.databases WHERE name = '{targetDatabase}'");
            if (!string.IsNullOrEmpty(isTargetDatabase))
            {
                _logger.LogDebug($"Database <{targetDatabase}> alredy exists, not need to create it");
                _connectionString.InitialCatalog = targetDatabase;
                return;
            }

            CreateDatabase(conn, targetDatabase);

            conn.ChangeDatabase(targetDatabase);

            CreateTables(conn);

            _connectionString.InitialCatalog = targetDatabase;
            _logger.LogInformation($"Database <{targetDatabase}> created");
        }

        public async Task<ICollection<Contact>> FindAllAsync()
        {
            using var conn = GetDbConnection();
            await conn.OpenAsync();

            var result = await conn.QueryAsync<Contact>("SELECT * FROM Contacts");
            return result.ToList();
        }

        public async Task<Contact> FindAsync(int id)
        {
            using var conn = GetDbConnection();
            await conn.OpenAsync();

            return await conn.QueryFirstOrDefaultAsync<Contact>("SELECT * FROM Contacts WHERE id = @contactId", new { contactId = id });
        }

        public async Task<int> InsertAsync(Contact item)
        {
            using var conn = GetDbConnection();
            await conn.OpenAsync();

            var sqlCmd = "INSERT INTO dbo.Contacts (Name, Surname, Nickname, PhoneNumber) VALUES (@Name, @Surname, @Nickname, @PhoneNumber);";
            sqlCmd += "SELECT MAX(Id) FROM dbo.Contacts;";

            return await conn.ExecuteScalarAsync<int>(sqlCmd, item);
        }

        public async Task<int> UpdateAsync(Contact item)
        {
            using var conn = GetDbConnection();
            await conn.OpenAsync();

            var sqlCmd = "UPDATE Contacts SET Name = @Name, Surname = @Surname, Nickname = @Nickname, PhoneNumber = @PhoneNumber WHERE id = @id";
            return await conn.ExecuteAsync(sqlCmd, item);
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = GetDbConnection();
            await conn.OpenAsync();

            await conn.ExecuteAsync("DELETE FROM Contacts WHERE id = @contactId", new { contactId = id });
        }

        #endregion

        #region Private Methods

        private SqlConnection GetDbConnection()
        {
            return new SqlConnection(_connectionString.ConnectionString);
        }

        private void CreateDatabase(SqlConnection conn, string targetDatabase)
        {
            _logger.LogInformation("Database <{0}> not exists, creating it", targetDatabase);
            conn.Execute($"CREATE DATABASE {targetDatabase}");
        }

        private void CreateTables(SqlConnection conn)
        {
            var contactsTable = "CREATE TABLE Contacts (";
            contactsTable += "Id integer IDENTITY(1,1), ";
            contactsTable += "Name text NOT NULL, ";
            contactsTable += "Surname text NOT NULL, ";
            contactsTable += "Nickname text NULL, ";
            contactsTable += "PhoneNumber text NOT NULL)";
            conn.Execute(contactsTable);
        }

        #endregion
    }
}
