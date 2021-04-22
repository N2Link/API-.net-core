using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Models;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecialtiesController : ControllerBase
    {
        private readonly FreeLancerVNContext _context;

        public SpecialtiesController(FreeLancerVNContext context)
        {
            _context = context;
        }

        // GET: api/Specialties
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Specialty>>> GetSpecialties()
        {
            return await _context.Specialties.ToListAsync();
        }

        // GET: api/Specialties/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Specialty>> GetSpecialty(int id)
        {
            var specialty = await _context.Specialties.FindAsync(id);

            if (specialty == null)
            {
                return NotFound();
            }

            return specialty;
        }

        // PUT: api/Specialties/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSpecialty(int id, Specialty specialty)
        {
            if (id != specialty.Id)
            {
                return BadRequest();
            }

            _context.Entry(specialty).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpecialtyExists(id))
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

        // POST: api/Specialties
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Specialty>> PostSpecialty(Specialty specialty)
        {
            _context.Specialties.Add(specialty);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSpecialty", new { id = specialty.Id }, specialty);
        }

        // DELETE: api/Specialties/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Specialty>> DeleteSpecialty(int id)
        {
            var specialty = await _context.Specialties.FindAsync(id);
            if (specialty == null)
            {
                return NotFound();
            }

            _context.Specialties.Remove(specialty);
            await _context.SaveChangesAsync();

            return specialty;
        }

        private bool SpecialtyExists(int id)
        {
            return _context.Specialties.Any(e => e.Id == id);
        }
    }
}
