using System.Collections.Generic;
using System.Threading.Tasks;
using AddressBook.API.Domains;

namespace AddressBook.API.Services
{
    public interface IContactService
    {
        Task<ICollection<Contact>> FindAllAsync();
        Task<Contact> FindAsync(uint id);
        Task<uint> InsertAsync(Contact item);
        Task<uint> UpdateAsync(Contact item);
        Task DeleteAsync(uint id);
    }
}
