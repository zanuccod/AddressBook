using System.Collections.Generic;
using System.Threading.Tasks;
using AddressBook.API.Domains;

namespace AddressBook.API.Services
{
    public interface ICountryService
    {
        Task<ICollection<Country>> FindAllAsync();
        Task<Country> FindByIdAsync(int id);
        Task<Country> FindByISOCodeAsync(string code);
    }
}
