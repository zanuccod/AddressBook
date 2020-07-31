using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AddressBook.API.Domains;
using AddressBook.API.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace AddressBook.API.Services
{
    public class CountryServiceMemoryCache : ICountryService
    {
        private readonly ICountryDataModel _countryDataModel;
        private readonly IMemoryCache _cache;
        private readonly ILogger<CountryServiceMemoryCache> _logger;

        public CountryServiceMemoryCache(ICountryDataModel countryDataModel, IMemoryCache cache, ILogger<CountryServiceMemoryCache> logger)
        {
            _countryDataModel = countryDataModel;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ICollection<Country>> FindAllAsync()
        {
            var items = await _cache.GetOrCreateAsync(CacheKeys.CountriesKey, entry =>
            {
                _logger.LogDebug("FindAllAsync: elements not be found at the cache, add it");

                entry.SlidingExpiration = TimeSpan.FromHours(1);
                return _countryDataModel.FindAllAsync();
            });

            _logger.LogDebug("FindAllAsync: find {0} elements", items.Count);
            return items;
        }

        public async Task<Country> FindByIdAsync(int id)
        {
            _logger.LogDebug("FindByIdAsync: start search for element with id <{id}>", id);

            var items = await FindAllAsync();
            return items.FirstOrDefault(x => x.Id == id);
        }

        public async Task<Country> FindByISOCodeAsync(string code)
        {
            _logger.LogDebug("FindByISOCodeAsync: start search for element with ISOCode <{isoCode}>", code);

            var items = await FindAllAsync();
            return items.FirstOrDefault(x => x.ISOCode.Equals(code));
        }
    }
}