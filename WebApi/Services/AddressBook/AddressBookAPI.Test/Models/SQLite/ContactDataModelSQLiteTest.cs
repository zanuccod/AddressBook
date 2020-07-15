using System.IO;
using System.Threading.Tasks;
using AddressBook.API.Domains;
using AddressBook.API.Models;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace AddressBookAPI.Test.Models.SQLite
{
    [TestFixture]
    public class ContactDataModelSQLiteTest
    {
        private ContactDataModelSQLite _dataModel;
        private const string dbPath = "dbSqLiteTest";

        [SetUp]
        public void BeforeEachTest()
        {
            // create new db file for test
            _dataModel = new ContactDataModelSQLite(dbPath, new NullLogger<ContactDataModelSQLite>());
        }

        [TearDown]
        public void AfterEachTest()
        {
            // delete all database files generated for test
            var files = Directory.GetFiles(Path.GetDirectoryName(Path.GetFullPath(dbPath)), dbPath + ".*");
            foreach (var file in files)
                File.Delete(file);
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
    }
}
