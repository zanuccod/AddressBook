using System.Data.SQLite;
using System.IO;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AddressBook.API.Models.BaseModels
{
    public class SQLiteDataModelBase
    {
        protected readonly IConfiguration _configuration;
        protected readonly ILogger<SQLiteDataModelBase> _logger;
        protected readonly string _connectionString;

        #region Constructors

        public SQLiteDataModelBase(IConfiguration configuration, ILogger<SQLiteDataModelBase> logger)
        {
            _configuration = configuration;
            _logger = logger;

            _connectionString = _configuration.GetConnectionString("SQLiteDbConnection");
            CreateDatabaseIfNotExists();
        }

        public SQLiteDataModelBase(string connectionString, ILogger<SQLiteDataModelBase> logger)
        {
            _connectionString = connectionString;
            _logger = logger;

            CreateDatabaseIfNotExists();
        }

        #endregion

        #region Private Methods

        private void CreateDatabaseIfNotExists()
        {
            _logger.LogInformation("Source database <{connectionString}>", _connectionString);
            if (File.Exists(_connectionString))
                return;

            _logger.LogInformation("Database <{connectionString}> not exists, creating it", _connectionString);

            Directory.CreateDirectory("Data");
            SQLiteConnection.CreateFile(_connectionString);

            using var conn = GetDbConnection();
            conn.Open();

            var table = "CREATE TABLE IF NOT EXISTS Countries (";
            table += "Id integer PRIMARY KEY AUTOINCREMENT, ";
            table += "Name text NOT NULL, ";
            table += "ISOCode text NOT NULL)";
            conn.Execute(table);

            table = "CREATE TABLE IF NOT EXISTS Contacts (";
            table += "Id integer PRIMARY KEY AUTOINCREMENT, ";
            table += "Name text NOT NULL, ";
            table += "Surname text NOT NULL, ";
            table += "Nickname text NULL, ";
            table += "CountryId integer NULL, ";
            table += "PhoneNumber text NOT NULL)";
            conn.Execute(table);

            _logger.LogInformation("Database created");
        }

        #endregion

        #region Protected Methods

        protected SQLiteConnection GetDbConnection()
        {
            return new SQLiteConnection($"Data Source={_connectionString}");
        }

        #endregion
    }
}
