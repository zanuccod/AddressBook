using System;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AddressBook.API.Models.BaseModels
{
    public abstract class SQLServerDataModelBase
    {
        protected readonly IConfiguration _configuration;
        protected readonly ILogger<SQLServerDataModelBase> _logger;
        protected readonly SqlConnectionStringBuilder _connectionString;

        #region Constructors

        public SQLServerDataModelBase(IConfiguration configuration, ILogger<SQLServerDataModelBase> logger)
        {
            _configuration = configuration;
            _logger = logger;

            _connectionString = new SqlConnectionStringBuilder(_configuration.GetConnectionString("SQLServerDbConnection"));
            CreateDatabaseIfNotExists();
        }

        public SQLServerDataModelBase(string dbconnection, ILogger<SQLServerDataModelBase> logger)
        {
            _connectionString = new SqlConnectionStringBuilder(dbconnection);
            _logger = logger;

            CreateDatabaseIfNotExists();
        }

        #endregion

        #region Private Methods

        private void CreateDatabaseIfNotExists()
        {
            _logger.LogInformation("Source database <{connectionString}>", _connectionString.ConnectionString);

            var targetDatabase = _connectionString.InitialCatalog;

            // change to master database to check if targetDatabase exsits and if not to create it
            _connectionString.InitialCatalog = "master";
            using var conn = GetDbConnection();
            conn.Open();

            var isTargetDatabase = conn.ExecuteScalar<string>($"SELECT * FROM sys.databases WHERE name = '{targetDatabase}'");
            if (!string.IsNullOrEmpty(isTargetDatabase))
            {
                _logger.LogDebug("Database <{targetDatabase}> alredy exists, not need to create it", targetDatabase);
                _connectionString.InitialCatalog = targetDatabase;
                return;
            }

            CreateDatabase(conn, targetDatabase);

            conn.ChangeDatabase(targetDatabase);

            CreateTables(conn);

            _connectionString.InitialCatalog = targetDatabase;
            _logger.LogInformation("Database <{targetDatabase}> created", targetDatabase);
        }

        private void CreateDatabase(SqlConnection conn, string targetDatabase)
        {
            _logger.LogInformation("Database <{targetDatabase}> not exists, creating it", targetDatabase);
            conn.Execute($"CREATE DATABASE {targetDatabase}");
        }

        private void CreateTables(SqlConnection conn)
        {
            var table = "CREATE TABLE Countries (";
            table += "Id integer IDENTITY(1,1), ";
            table += "Name text NOT NULL, ";
            table += "ISOCode text NOT NULL)";
            conn.Execute(table);

            table = "CREATE TABLE Contacts (";
            table += "Id integer IDENTITY(1,1), ";
            table += "Name text NOT NULL, ";
            table += "Surname text NOT NULL, ";
            table += "Nickname text NULL, ";
            table += "CountryId integer NULL, ";
            table += "PhoneNumber text NOT NULL,";
            table += "FOREIGN KEY (CountryId) REFERENCES Countries(Id))";
            conn.Execute(table);
        }

        #endregion

        #region Protected Methods

        protected SqlConnection GetDbConnection()
        {
            return new SqlConnection(_connectionString.ConnectionString);
        }

        #endregion
    }
}
