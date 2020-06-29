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

            _logger.LogDebug("FindAllAsync: find {0} elements", items.Count);
            return items;
        }

        public async Task<Contact> FindAsync(uint id)
        {
            _logger.LogDebug("FindAsync: start search for element with id <{0}>", id);

            var item = await _contactDataModel.FindAsync(id);
            return item;
        }

        public async Task<uint> InsertAsync(Contact item)
        {
            _logger.LogDebug("InsertAsync: insert element <{0}>", item.ToString());
            return await _contactDataModel.InsertAsync(item);
        }

        public async Task<uint> UpdateAsync(Contact item)
        {
            _logger.LogDebug("UpdateAsync: update element with id <{0}>", item.Id);
            return await _contactDataModel.UpdateAsync(item);
        }

        public async Task DeleteAsync(uint id)
        {
            _logger.LogDebug("DeleteAsync: delete element with id <{0}>", id);
            await _contactDataModel.DeleteAsync(id);
        }
    }
}
