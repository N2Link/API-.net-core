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

namespace Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TypeOfWorksController : ControllerBase
    {
        private readonly FreeLancerVNContext _context;

        public TypeOfWorksController(FreeLancerVNContext context)
        {
            _context = context;
        }

        // GET: api/TypeOfWorks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResponseIdName>>> GetTypeOfWorks()
        {
            return await _context.TypeOfWorks.Select(p=> new ResponseIdName(p)).ToListAsync();
        }

        // GET: api/TypeOfWorks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseIdName>> GetTypeOfWork(int id)
        {
            var typeOfWork = await _context.TypeOfWorks.FindAsync(id);

            if (typeOfWork == null)
            {
                return NotFound();
            }

            return new ResponseIdName(typeOfWork) ;
        }

        // PUT: api/TypeOfWorks/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTypeOfWork(int id, TypeOfWork typeOfWork)
        {
            if (id != typeOfWork.Id)
            {
                return BadRequest();
            }

            _context.Entry(typeOfWork).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TypeOfWorkExists(id))
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

        // POST: api/TypeOfWorks
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<TypeOfWork>> PostTypeOfWork(TypeOfWork typeOfWork)
        {
            _context.TypeOfWorks.Add(typeOfWork);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTypeOfWork", new { id = typeOfWork.Id }, typeOfWork);
        }

        // DELETE: api/TypeOfWorks/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TypeOfWork>> DeleteTypeOfWork(int id)
        {
            var typeOfWork = await _context.TypeOfWorks.FindAsync(id);
            if (typeOfWork == null)
            {
                return NotFound();
            }

            _context.TypeOfWorks.Remove(typeOfWork);
            await _context.SaveChangesAsync();

            return typeOfWork;
        }

        private bool TypeOfWorkExists(int id)
        {
            return _context.TypeOfWorks.Any(e => e.Id == id);
        }
    }
}
