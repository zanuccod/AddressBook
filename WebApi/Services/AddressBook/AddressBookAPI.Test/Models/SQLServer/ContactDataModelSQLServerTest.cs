using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AddressBook.API.Domains;
using AddressBook.API.Models;
using Dapper;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace AddressBookAPI.Test.Models.SQLServer
{
    [TestFixture]
    public class ContactDataModelSQLServerTest : BaseTest
    {
        private ContactDataModelSQLServer _dataModel;
        private CountryDataModelSQServer _countriesDataModel;

        private SqlConnectionStringBuilder builder;

        const string connString = "Server=localhost;Database=TutorialDbTest;user id=sa;password=reallyStrongPwd#123;Application Name=AddressBook.API;";

        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            builder = new SqlConnectionStringBuilder(connString);

            // force to create TutorialDbTest database with needed tables for tests
            _dataModel = new ContactDataModelSQLServer(builder.ConnectionString, new NullLogger<ContactDataModelSQLServer>());
            _countriesDataModel = new CountryDataModelSQServer(builder.ConnectionString, new NullLogger<CountryDataModelSQServer>());
        }

        [OneTimeTearDown]
        public void AfterAllTests()
        {
            // delete test database
            builder.InitialCatalog = "master";
            using var conn = new SqlConnection(builder.ConnectionString);
            conn.OpenAsync();

            conn.ExecuteAsync($"DROP DATABASE TutorialDbTest").ConfigureAwait(true);
        }

        [TearDown]
        public void AfterEachTest()
        {
            using var conn = new SqlConnection(builder.ConnectionString);
            conn.OpenAsync();

            // delete all data on Contacts table to reset conditions for tests
            conn.Execute($"DELETE FROM Contacts");
            conn.Execute($"DELETE FROM Countries");
        }

        [Test]
        public async Task TestCRUD()
        {
            // Arrange
            var item = new Contact
            {
                Name = "testName",
                Surname = "testSurname",
                PhoneNumber = "123124"
            };

            /****** INSERT *******/
            // Act
            var insertedId = await _dataModel.InsertAsync(item).ConfigureAwait(true);

            // Assert
            var insertedItem = await _dataModel.FindAsync(insertedId).ConfigureAwait(true);

            Assert.AreEqual(item.Name, insertedItem.Name);
            Assert.AreEqual(item.Surname, insertedItem.Surname);
            Assert.AreEqual(item.PhoneNumber, insertedItem.PhoneNumber);

            /****** UPDATE *******/
            // Act
            insertedItem.Name = "testName_1";
            var result = await _dataModel.UpdateAsync(insertedItem).ConfigureAwait(true);

            // Assert
            item = await _dataModel.FindAsync(insertedItem.Id).ConfigureAwait(true);
            Assert.AreEqual(1, result);

            Assert.AreEqual(item.Name, "testName_1");
            Assert.AreEqual(item.Surname, insertedItem.Surname);
            Assert.AreEqual(item.PhoneNumber, insertedItem.PhoneNumber);

            /****** DELETE *******/
            // Act
            await _dataModel.DeleteAsync(insertedItem.Id).ConfigureAwait(true);

            // Assert
            var items = await _dataModel.FindAllAsync().ConfigureAwait(true);

            Assert.AreEqual(0, items.Count);
        }

        [Test]
        public async Task FindAllAsync()
        {
            // Arrange
            var itemCount = 10;
            for (uint i = 1; i <= itemCount; i++)
            {
                await _dataModel.InsertAsync(new Contact
                {
                    Name = $"testName_{i}",
                    Surname = $"testSurname_{i}",
                    PhoneNumber = $"123124{i}"
                }).ConfigureAwait(true);
            }

            // Act
            var items = await _dataModel.FindAllAsync().ConfigureAwait(true);

            // Assert
            Assert.AreEqual(itemCount, items.Count);
        }

        [Test]
        public async Task TestCRUDWithCountriesTable()
        {
            // Arrange
            var countryITA = new Country { Name = "Italy", ISOCode = "ITA" };

            // to get autoincrement id
            await _countriesDataModel.InsertAsync(countryITA);
            countryITA = (await _countriesDataModel.FindAllAsync()).First();

            var item = new Contact
            {
                Name = "testName",
                Surname = "testSurname",
                PhoneNumber = "123124",
                Country = countryITA
            };

            /****** INSERT *******/
            // Act
            var insertedId = await _dataModel.InsertAsync(item).ConfigureAwait(true);

            // Assert
            var insertedItem = await _dataModel.FindAsync(insertedId).ConfigureAwait(true);

            Assert.AreEqual(item.Name, insertedItem.Name);
            Assert.AreEqual(item.Surname, insertedItem.Surname);
            Assert.AreEqual(item.PhoneNumber, insertedItem.PhoneNumber);
            Assert.AreEqual(item.Country, insertedItem.Country);

            ///****** UPDATE *******/
            // Act
            await _countriesDataModel.InsertAsync(new Country { Name = "United States of America", ISOCode = "USA" });
            var countryUSA = (await _countriesDataModel.FindAllAsync()).First(x => x.ISOCode.Equals("USA"));

            insertedItem.Name = "testName_1";
            insertedItem.Country = countryUSA;

            var result = await _dataModel.UpdateAsync(insertedItem).ConfigureAwait(true);

            // Assert
            item = await _dataModel.FindAsync(insertedItem.Id).ConfigureAwait(true);
            Assert.AreEqual(1, result);

            Assert.AreEqual(item.Name, "testName_1");
            Assert.AreEqual(item.Surname, insertedItem.Surname);
            Assert.AreEqual(item.Country, insertedItem.Country);
            Assert.AreEqual(item.PhoneNumber, insertedItem.PhoneNumber);

            /****** DELETE *******/
            // Act
            await _dataModel.DeleteAsync(insertedItem.Id).ConfigureAwait(true);

            // Assert
            var items = await _dataModel.FindAllAsync().ConfigureAwait(true);

            Assert.AreEqual(0, items.Count);
        }

        [Test]
        public async Task FindAllAsyncWithCountriesTable()
        {
            // Arrange
            var countryITA = new Country { Name = "Italy", ISOCode = "ITA" };
            await _countriesDataModel.InsertAsync(countryITA);
            countryITA = (await _countriesDataModel.FindAllAsync()).First();

            var itemCount = 10;
            for (uint i = 1; i <= itemCount; i++)
            {
                await _dataModel.InsertAsync(new Contact
                {
                    Name = $"testName_{i}",
                    Surname = $"testSurname_{i}",
                    PhoneNumber = $"123124{i}",
                    Country = countryITA
                }).ConfigureAwait(true);
            }

            // Act
            var items = await _dataModel.FindAllAsync().ConfigureAwait(true);

            // Assert
            Assert.AreEqual(itemCount, items.Count);
            Assert.AreEqual(items.First().Country, countryITA);
        }
    }
}
