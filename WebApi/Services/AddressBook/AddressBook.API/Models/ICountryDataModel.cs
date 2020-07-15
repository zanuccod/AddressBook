using System.Threading.Tasks;
using AddressBook.API.Domains;

namespace AddressBook.API.Models
{
    public interface ICountryDataModel : IDataModel<Country>
    {
        Task<Country> FindByISOCodeAsync(string code);
    }
}
