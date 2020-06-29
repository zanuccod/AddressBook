using System.Collections.Generic;
using System.Threading.Tasks;

namespace AddressBook.API.Models
{
    public interface IDataModel<T>
    {
        Task<ICollection<T>> FindAllAsync();
        Task<T> FindAsync(uint id);
        Task<uint> InsertAsync(T item);
        Task<uint> UpdateAsync(T item);
        Task DeleteAsync(uint id);
    }
}
