using AddressBook.API;

namespace AddressBookAPI.Test.Models
{
    public abstract class BaseTest
    {
        public BaseTest()
        {
            Program.InitDapperColumnsMapping();
        }
    }
}
