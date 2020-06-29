using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AddressBook.API.Domains;
using AddressBook.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AddressBook.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressBookController : ControllerBase
    {
        private readonly IContactService _contactService;
        private readonly ILogger<AddressBookController> _logger;

        public AddressBookController(IContactService contactService, ILogger<AddressBookController> logger)
        {
            _contactService = contactService;
            _logger = logger;
        }

        [HttpGet("find-all")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<ICollection<Contact>>> FindAll()
        {
            try
            {
                var items = await _contactService.FindAllAsync();
                _logger.LogDebug($"Response <{nameof(Ok)}>, found <{items.Count}> contacts");
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GetType().Name);
                throw;
            }
        }

        [HttpGet("find")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Contact>> Find(uint id)
        {
            try
            {
                var item = await _contactService.FindAsync(id);
                if (item != null)
                {
                    _logger.LogDebug($"Response <{nameof(Ok)}>>, contact with id <{item.Id}> found");
                    return Ok(item);
                }
                else
                {
                    _logger.LogDebug($"Response <{nameof(NotFound)}>>, contact with id <{id}> not found");
                    return NotFound(id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GetType().Name);
                throw;
            }
        }

        [HttpPost("insert")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Contact>> Insert(Contact item)
        {
            try
            {
                if (item == null)
                {
                    _logger.LogDebug($"Response <{nameof(BadRequest)}>, given contact is null");
                    return BadRequest("contact not specified");
                }

                var result = await _contactService.InsertAsync(item);
                _logger.LogDebug($"Response <{nameof(Ok)}>>, contact <{item}> inserted");

                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GetType().Name);
                throw;
            }
        }

        [HttpPut("update")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Contact>> Update(Contact item)
        {
            try
            {
                if (item == null)
                {
                    _logger.LogDebug($"Response <{nameof(BadRequest)}>, given contact is null");
                    return BadRequest("contact not specified");
                }

                var result = await _contactService.UpdateAsync(item);
                _logger.LogDebug($"Response <{nameof(Ok)}>>, contact <{item}> updated");

                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GetType().Name);
                throw;
            }
        }

        [HttpDelete("delete")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Contact>> Delete(Contact item)
        {
            try
            {
                if (item == null)
                {
                    _logger.LogDebug($"Response <{nameof(BadRequest)}>, given contact is null");
                    return BadRequest("contact not specified");
                }

                await _contactService.DeleteAsync(item.Id);
                _logger.LogDebug($"Response <{nameof(Ok)}>>, contact <{item}> deleted");

                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GetType().Name);
                throw;
            }
        }
    }
}
