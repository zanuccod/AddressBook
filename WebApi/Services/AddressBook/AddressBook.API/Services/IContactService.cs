using System.Collections.Generic;
using System.Threading.Tasks;
using AddressBook.API.Domains;

namespace AddressBook.API.Services
{
    public interface IContactService
    {
        Task<ICollection<Contact>> FindAllAsync();
        Task<Contact> FindAsync(int id);
        Task<int> InsertAsync(Contact item);
        Task<Contact> UpdateAsync(Contact item);
        Task DeleteAsync(int id);
    }
}
