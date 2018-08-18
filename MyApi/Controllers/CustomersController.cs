using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Models;
using Shared;
using System.IO;

namespace MyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly MyContext _context;

        public CustomersController(MyContext context)
        {
            _context = context;
        }
        [HttpPost("profileImage")]
        public async Task<IActionResult> PostImageAsync()
        {
            List<string> files = new List<string>();

            if (Request.Form?.Files?.Count > 0)
            {
                foreach (var item in Request.Form.Files)
                {
                    var fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(item.FileName);
                    using (var fileStream = System.IO.File.Create(Path.Combine(Directory.GetCurrentDirectory(), "Uploads", fileName)))
                    {
                        await item.CopyToAsync(fileStream);
                    }
                    files.Add(fileName);
                }
            }

            return Ok(files);
        }
        // GET: api/Customers
        [HttpGet]
        public IEnumerable<Customer> GetCustomer()
        {
            return _context.Customers;
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        // PUT: api/Customers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer([FromRoute] string id, [FromBody] Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != customer.Id)
            {
                return BadRequest();
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Customers
        [HttpPost]
        public async Task<IActionResult> PostCustomer([FromBody] Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCustomer", new { id = customer.Id }, customer);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return Ok(customer);
        }

        private bool CustomerExists(string id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }

        [HttpPut("/sync")]
        public async Task<IActionResult> Sync([FromBody] List<Customer> changed, [FromQuery] DateTime? since = null)
        {
            using (var trans = _context.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                foreach (var update in changed)
                {
                    if (!CustomerExists(update.Id))
                    {
                        _context.InsertCustomer(update);
                    }
                    else
                    {
                        _context.UpdateCustomer(update);
                    }
                }
                trans.Commit();
            }

            if (since == null)
            {
                return Ok(await _context.Customers.ToListAsync());
            }
            else
            {
                return Ok(await _context.Customers.Where(x => x.LastUpdated >= since.Value || (x.Deleted != null && x.Deleted > since.Value)).ToListAsync());
            }
        }
    }
}