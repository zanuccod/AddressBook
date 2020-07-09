using System.Threading.Tasks;
using AddressBook.API.Controllers;
using AddressBook.API.Domains;
using AddressBook.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace AddressBookAPI.Test.Controller
{
    [TestFixture]
    public class AddressBookControllerTest
    {
        private Mock<IContactService> _mockContactService;
        private AddressBookController _controller;

        [SetUp]
        public void BeforeEachTest()
        {
            _mockContactService = new Mock<IContactService>();
            _controller = new AddressBookController(_mockContactService.Object, new NullLogger<AddressBookController>());
        }

        [Test]
        public async Task Find_ContactExists_ShouldReturnOk()
        {
            // Act
            var item = new Contact
            {
                Id = 1,
                Name = "testName",
                Surname = "testSurname",
                PhoneNumber = "123124"
            };

            _mockContactService.Setup(x => x.FindAsync(It.IsAny<int>())).Returns(Task.FromResult(item));
            var result = await _controller.Find(1);

            // Assert
            Assert.AreEqual(typeof(OkObjectResult), result.Result.GetType());

            var objectResult = (OkObjectResult)result.Result;
            Assert.AreEqual(item, objectResult.Value);
        }

        [Test]
        public async Task Find_NotValidId_ShouldReturnBadRequest()
        {
            // Act
            var result = await _controller.Find(0);

            // Assert
            Assert.AreEqual(typeof(BadRequestObjectResult), result.Result.GetType());
        }

        [Test]
        public async Task Find_ContactsNotExists_ShouldReturnNotFound()
        {
            // Act
            var result = await _controller.Find(1);

            // Assert
            Assert.AreEqual(typeof(NotFoundObjectResult), result.Result.GetType());
        }

        [Test]
        public async Task Insert_ItemNull_ShouldReturnBadRequest()
        {
            // Act
            var result = await _controller.Insert(null);

            // Assert
            Assert.AreEqual(typeof(BadRequestObjectResult), result.Result.GetType());
        }

        [Test]
        public async Task Insert_ItemNotNull_ShouldReturnCreated()
        {
            // Act
            var result = await _controller.Insert(new Contact());

            // Assert
            Assert.AreEqual(typeof(CreatedAtActionResult), result.Result.GetType());
        }

        [Test]
        public async Task Update_ItemNull_ShouldReturnBadRequest()
        {
            // Act
            var result = await _controller.Update(null);

            // Assert
            Assert.AreEqual(typeof(BadRequestObjectResult), result.Result.GetType());
        }

        [Test]
        public async Task Update_ItemNotFound_ShouldReturnNotFound()
        {
            // Act
            var result = await _controller.Update(new Contact());

            // Assert
            Assert.AreEqual(typeof(NotFoundResult), result.Result.GetType());
        }

        [Test]
        public async Task Update_ItemNotNull_ShouldReturnOk()
        {
            // Assert
            var item = new Contact
            {
                Id = 1
            };
            _mockContactService.Setup(x => x.FindAsync(item.Id)).Returns(Task.FromResult(item));

            // Act
            var result = await _controller.Update(item);

            // Assert
            Assert.AreEqual(typeof(OkObjectResult), result.Result.GetType());
        }

        [Test]
        public async Task Delete_NotValidId_ShouldReturnBadRequest()
        {
            // Act
            var result = await _controller.Delete(0);

            // Assert
            Assert.AreEqual(typeof(BadRequestObjectResult), result.Result.GetType());
        }
    }
}
