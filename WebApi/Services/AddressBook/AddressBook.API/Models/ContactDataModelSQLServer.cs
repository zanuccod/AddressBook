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

            var sqlCmd = "SELECT Contacts.Id as contactId, Contacts.Name as contactName, Contacts.Surname as contactSurname, ";
            sqlCmd += "Contacts.Nickname as contactNickname, Contacts.PhoneNumber as contactPhoneNumber, Contacts.CountryId, ";
            sqlCmd += "Countries.Id as countryId, Countries.Name as countryName, Countries.ISOCode as countryISOCode ";
            sqlCmd += "FROM Contacts ";
            sqlCmd += "LEFT OUTER JOIN Countries ON Countries.Id = Contacts.CountryId ";

            var result = await conn.QueryAsync<Contact, Country, Contact>(
                sqlCmd,
                (contact, country) => { contact.Country = country; return contact; },
                splitOn: "countryId");
            return result.ToList();
        }

        public async Task<Contact> FindAsync(int id)
        {
            using var conn = GetDbConnection();
            await conn.OpenAsync();

            var sqlCmd = "SELECT Contacts.Id as contactId, Contacts.Name as contactName, Contacts.Surname as contactSurname, ";
            sqlCmd += "Contacts.Nickname as contactNickname, Contacts.PhoneNumber as contactPhoneNumber, Contacts.CountryId, ";
            sqlCmd += "Countries.Id as countryId, Countries.Name as countryName, Countries.ISOCode as countryISOCode ";
            sqlCmd += "FROM Contacts ";
            sqlCmd += "LEFT OUTER JOIN Countries ON Countries.Id = Contacts.CountryId ";
            sqlCmd += "WHERE Contacts.id = @contactId";

            var result = await conn.QueryAsync<Contact, Country, Contact>(
                sqlCmd,
                (contact, country) => { contact.Country = country; return contact; },
                splitOn: "countryId",
                param: new { contactId = id });

            return result.FirstOrDefault();
        }

        public async Task<int> InsertAsync(Contact item)
        {
            using var conn = GetDbConnection();
            await conn.OpenAsync();

            var sqlCmd = "INSERT INTO Contacts (Name, Surname, Nickname, CountryId, PhoneNumber) VALUES (@Name, @Surname, @Nickname, @CountryId, @PhoneNumber);";
            sqlCmd += "SELECT MAX(Id) FROM Contacts;";

            return await conn.ExecuteScalarAsync<int>(sqlCmd, new { item.Name, item.Surname, item.Nickname, CountryId = item.Country?.Id, item.PhoneNumber });
        }

        public async Task<int> UpdateAsync(Contact item)
        {
            using var conn = GetDbConnection();
            await conn.OpenAsync();

            var sqlCmd = "UPDATE Contacts SET Name = @Name, Surname = @Surname, Nickname = @Nickname, CountryId = @CountryId, PhoneNumber = @PhoneNumber WHERE id = @id";
            return await conn.ExecuteAsync(sqlCmd, new { item.Name, item.Surname, item.Nickname, CountryId = item.Country?.Id, item.PhoneNumber, id = item.Id });
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
