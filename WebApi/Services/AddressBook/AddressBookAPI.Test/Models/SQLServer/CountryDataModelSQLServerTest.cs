using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using AddressBook.API.Domains;
using AddressBook.API.Models;
using Dapper;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace AddressBookAPI.Test.Models.SQLServer
{
    public class CountryDataModelSQLServerTest
    {
        private CountryDataModelSQServer _dataModel;
        private SqlConnectionStringBuilder builder;

        const string connString = "Server=localhost;Database=TutorialDbTest;user id=sa;password=reallyStrongPwd#123;Application Name=AddressBook.API;";

        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            builder = new SqlConnectionStringBuilder(connString);

            // force to create TutorialDbTest database with needed tables for tests
            _dataModel = new CountryDataModelSQServer(builder.ConnectionString, new NullLogger<CountryDataModelSQServer>());
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
            conn.Execute($"DELETE FROM Countries");
        }

        [Test]
        public async Task TestCRUD()
        {
            // Arrange
            var item = new Country
            {
                Name = "United States of America",
                ISOCode = "USA"
            };

            /****** INSERT *******/
            // Act
            var insertedId = await _dataModel.InsertAsync(item).ConfigureAwait(true);

            // Assert
            var insertedItem = await _dataModel.FindAsync(insertedId).ConfigureAwait(true);

            Assert.AreEqual(item.Name, insertedItem.Name);
            Assert.AreEqual(item.ISOCode, insertedItem.ISOCode);

            /****** UPDATE *******/
            // Act
            insertedItem.Name = "testName_1";
            var result = await _dataModel.UpdateAsync(insertedItem).ConfigureAwait(true);

            // Assert
            item = await _dataModel.FindAsync(insertedItem.Id).ConfigureAwait(true);
            Assert.AreEqual(1, result);

            Assert.AreEqual(item.Name, "testName_1");
            Assert.AreEqual(item.ISOCode, insertedItem.ISOCode);

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
                await _dataModel.InsertAsync(new Country
                {
                    Name = "United States of America",
                    ISOCode = "USA"
                }).ConfigureAwait(true);
            }

            // Act
            var items = await _dataModel.FindAllAsync().ConfigureAwait(true);

            // Assert
            Assert.AreEqual(itemCount, items.Count);
        }
    }
}
