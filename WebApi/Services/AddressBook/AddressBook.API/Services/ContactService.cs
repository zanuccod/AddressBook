using System.Collections.Generic;
using System.Threading.Tasks;
using AddressBook.API.Domains;
using AddressBook.API.Models;
using Microsoft.Extensions.Logging;

namespace AddressBook.API.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactDataModel _contactDataModel;
        private readonly ILogger<ContactService> _logger;

        public ContactService(IContactDataModel contactDataModel, ILogger<ContactService> logger)
        {
            _contactDataModel = contactDataModel;
            _logger = logger;
        }

        public async Task<ICollection<Contact>> FindAllAsync()
        {
            var items = await _contactDataModel.FindAllAsync();

            _logger.LogDebug("FindAllAsync: find {count} elements", items.Count);
            return items;
        }

        public async Task<Contact> FindAsync(int id)
        {
            _logger.LogDebug("FindAsync: start search for element with id <{id}>", id);

            var item = await _contactDataModel.FindAsync(id);
            return item;
        }

        public async Task<int> InsertAsync(Contact item)
        {
            _logger.LogDebug("InsertAsync: insert element <{item}>", item);
            return await _contactDataModel.InsertAsync(item);
        }

        public async Task<Contact> UpdateAsync(Contact item)
        {
            _logger.LogDebug("UpdateAsync: update element with id <{id}>", item.Id);
            await _contactDataModel.UpdateAsync(item);

            return await _contactDataModel.FindAsync(item.Id);
        }

        public async Task DeleteAsync(int id)
        {
            _logger.LogDebug("DeleteAsync: delete element with id <{id}>", id);
            await _contactDataModel.DeleteAsync(id);
        }
    }
}
