using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AddressBook.API.Domains;
using AddressBook.API.Models.BaseModels;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AddressBook.API.Models
{
    public class CountryDataModelSQLite : SQLiteDataModelBase, ICountryDataModel
    {
        #region Constructors

        public CountryDataModelSQLite(IConfiguration configuration, ILogger<CountryDataModelSQLite> logger)
            : base(configuration, logger)
        { }

        public CountryDataModelSQLite(string _connectionString, ILogger<CountryDataModelSQLite> logger)
            : base(_connectionString, logger)
        { }

        #endregion

        #region Public Methods

        public async Task<ICollection<Country>> FindAllAsync()
        {
            using var conn = GetDbConnection();
            await conn.OpenAsync();

            var result = await conn.QueryAsync<Country>("SELECT * FROM Countries");
            return result.ToList();
        }

        public async Task<Country> FindAsync(int id)
        {
            using var conn = GetDbConnection();
            await conn.OpenAsync();

            return await conn.QueryFirstOrDefaultAsync<Country>("SELECT * FROM Countries WHERE id = @countryId", new { countryId = id });
        }

        public async Task<Country> FindByISOCodeAsync(string code)
        {
            using var conn = GetDbConnection();
            await conn.OpenAsync();

            return await conn.QueryFirstOrDefaultAsync<Country>("SELECT * FROM Countries WHERE ISOCode = @isoCode", new { isoCode = code });
        }

        public async Task<int> InsertAsync(Country item)
        {
            using var conn = GetDbConnection();
            await conn.OpenAsync();

            var sqlCmd = "INSERT INTO Countries (Name, ISOCode) VALUES (@Name, @isoCode);";
            sqlCmd += "SELECT MAX(Id) FROM Countries;";

            return await conn.ExecuteScalarAsync<int>(sqlCmd, item);
        }

        public async Task<int> UpdateAsync(Country item)
        {
            using var conn = GetDbConnection();
            await conn.OpenAsync();

            var sqlCmd = "UPDATE Countries SET Name = @Name, ISOCode = @isoCode WHERE id = @id";
            return await conn.ExecuteAsync(sqlCmd, item);
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = GetDbConnection();
            await conn.OpenAsync();

            await conn.ExecuteAsync("DELETE FROM Countries WHERE id = @countryId", new { countryId = id });
        }

        #endregion
    }
}
