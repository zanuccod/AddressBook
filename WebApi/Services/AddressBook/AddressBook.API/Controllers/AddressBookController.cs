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
    [Produces("application/json")]
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
                _logger.LogDebug("Response <{Response}>, found <{count}> contacts", nameof(Ok), items.Count);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GetType().Name);
                throw;
            }
        }

        [HttpGet("find/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Contact>> Find(int id)
        {
            try
            {
                if (id < 1)
                {
                    _logger.LogDebug("Response <{Response}>, given id is not valid", nameof(BadRequest), id);
                    return BadRequest();
                }

                var item = await _contactService.FindAsync(id);
                if (item != null)
                {
                    _logger.LogDebug("Response <{Response}>, contact with id <{id}> found", nameof(Ok), item.Id);
                    return Ok(item);
                }
                else
                {
                    _logger.LogDebug("Response <{Response}>, contact with id <{id}> not found", nameof(NotFound), id);
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
                    _logger.LogDebug("Response <{Response}>, given contact is null", nameof(BadRequest));
                    return BadRequest("contact not specified");
                }

                if (ModelState.IsValid)
                {
                    var result = await _contactService.InsertAsync(item);

                    item.Id = result;
                    _logger.LogDebug("Response <{Response}>, contact <{item}> inserted", nameof(Ok), item);

                    return CreatedAtAction(nameof(Find), new { id = result }, item);
                }
                else
                {
                    return BadRequest(ModelState);
                }
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
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Contact>> Update(Contact item)
        {
            try
            {
                if (item == null)
                {
                    _logger.LogDebug("Response <{Response}>, given contact is null", nameof(BadRequest));
                    return BadRequest("contact not specified");
                }

                var found = await _contactService.FindAsync(item.Id);
                if (found == null)
                {
                    _logger.LogDebug("Response <{Response}>, given contact with id <{id}> not found", nameof(NotFound), item.Id);
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var result = await _contactService.UpdateAsync(item);
                    _logger.LogDebug("Response <{Response}>, contact <{item}> updated", nameof(Ok), item);

                    return Ok(item);
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GetType().Name);
                throw;
            }
        }

        [HttpDelete("delete/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Contact>> Delete(int id)
        {
            try
            {
                if (id < 1)
                {
                    _logger.LogDebug("Response <{Response}>, given id is not valid", nameof(BadRequest), id);
                    return BadRequest();
                }

                var item = await _contactService.FindAsync(id);
                if (item == null)
                {
                    _logger.LogDebug("Response <{Response}>, given contact with id <{id}> not found", nameof(NotFound), id);
                    return NotFound();
                }

                await _contactService.DeleteAsync(id);
                _logger.LogDebug("Response <{Response}>>, contact with <{id}> deleted", nameof(Ok), id);

                return Ok(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GetType().Name);
                throw;
            }
        }
    }
}
