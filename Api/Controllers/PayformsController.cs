using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Api.Enities;
using Microsoft.AspNetCore.Cors;

namespace Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]

    public class PayformsController : ControllerBase
    {
        private readonly FreeLancerVNContext _context;

        public PayformsController(FreeLancerVNContext context)
        {
            _context = context;
        }

        // GET: api/Payforms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResponseIdName>>> GetPayforms()
        {
            return await _context.Payforms.Select(p=>new ResponseIdName(p)).ToListAsync();
        }

        // GET: api/Payforms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseIdName>> GetPayform(int id)
        {
            var payform = await _context.Payforms.FindAsync(id);

            if (payform == null)
            {
                return NotFound();
            }

            return new ResponseIdName(payform);
        }

        // PUT: api/Payforms/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPayform(int id, Payform payform)
        {
            if (id != payform.Id)
            {
                return BadRequest();
            }

            _context.Entry(payform).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PayformExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }

        // POST: api/Payforms
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Payform>> PostPayform(Payform payform)
        {
            _context.Payforms.Add(payform);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPayform", new { id = payform.Id }, payform);
        }

        // DELETE: api/Payforms/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Payform>> DeletePayform(int id)
        {
            var payform = await _context.Payforms.FindAsync(id);
            if (payform == null)
            {
                return NotFound();
            }

            _context.Payforms.Remove(payform);
            await _context.SaveChangesAsync();

            return payform;
        }

        private bool PayformExists(int id)
        {
            return _context.Payforms.Any(e => e.Id == id);
        }
    }
}
