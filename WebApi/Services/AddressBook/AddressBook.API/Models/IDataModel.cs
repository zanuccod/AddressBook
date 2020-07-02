using System.Collections.Generic;
using System.Threading.Tasks;

namespace AddressBook.API.Models
{
    public interface IDataModel<T>
    {
        Task<ICollection<T>> FindAllAsync();
        Task<T> FindAsync(int id);
        Task<int> InsertAsync(T item);
        Task<int> UpdateAsync(T item);
        Task DeleteAsync(int id);
    }
}
