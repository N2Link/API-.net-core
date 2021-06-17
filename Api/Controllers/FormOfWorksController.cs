using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

namespace Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]

    public class FormOfWorksController : ControllerBase
    {
        private readonly FreeLancerVNContext _context;

        public FormOfWorksController(FreeLancerVNContext context)
        {
            _context = context;
        }

        // GET: api/FormOfWorks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FormOfWork>>> GetFormOfWorks()
        {
            return await _context.FormOfWorks.ToListAsync();
        }

        // GET: api/FormOfWorks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FormOfWork>> GetFormOfWork(int id)
        {
            var formOfWork = await _context.FormOfWorks.FindAsync(id);

            if (formOfWork == null)
            {
                return NotFound();
            }

            return formOfWork;
        }

        // PUT: api/FormOfWorks/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFormOfWork(int id, FormOfWork formOfWork)
        {
            if (id != formOfWork.Id)
            {
                return BadRequest();
            }

            _context.Entry(formOfWork).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FormOfWorkExists(id))
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

        // POST: api/FormOfWorks
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<FormOfWork>> PostFormOfWork(FormOfWork formOfWork)
        {
            _context.FormOfWorks.Add(formOfWork);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFormOfWork", new { id = formOfWork.Id }, formOfWork);
        }

        // DELETE: api/FormOfWorks/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<FormOfWork>> DeleteFormOfWork(int id)
        {
            var formOfWork = await _context.FormOfWorks.FindAsync(id);
            if (formOfWork == null)
            {
                return NotFound();
            }

            _context.FormOfWorks.Remove(formOfWork);
            await _context.SaveChangesAsync();

            return formOfWork;
        }

        private bool FormOfWorkExists(int id)
        {
            return _context.FormOfWorks.Any(e => e.Id == id);
        }
    }
}
