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
    public class ContactDataModelSQLServer : SQLServerDataModelBase, IContactDataModel
    {
        #region Constructors

        public ContactDataModelSQLServer(IConfiguration configuration, ILogger<ContactDataModelSQLServer> logger)
            : base(configuration, logger)
        { }

        public ContactDataModelSQLServer(string dbconnection, ILogger<ContactDataModelSQLServer> logger)
            : base(dbconnection, logger)
        { }

        #endregion

        #region Public Methods

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

            var sqlCmd = "INSERT INTO Contacts (Name, Surname, Nickname, PhoneNumber) VALUES (@Name, @Surname, @Nickname, @PhoneNumber);";
            sqlCmd += "SELECT MAX(Id) FROM Contacts;";

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
    }
}
