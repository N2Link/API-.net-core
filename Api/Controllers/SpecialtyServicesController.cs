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
    public class SpecialtyServicesController : ControllerBase
    {
        private readonly FreeLancerVNContext _context;

        public SpecialtyServicesController(FreeLancerVNContext context)
        {
            _context = context;
        }

        // GET: api/SpecialtyServices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SS>>> GetSpecialtyServices()
        {
            return await _context.SpecialtyServices.Include(p=>p.Service).Include(p=>p.Specialty).
                Select(p=> new SS 
                {
                    ServiceID = p.ServiceId,
                    SpecialtyID = p.SpecialtyId,
                    ServiceName = p.Service.Name,
                    SpecialtyName = p.Specialty.Name,
                }).ToListAsync();
        }

        [HttpGet("getservices/{specialtyId}")]
        public async Task<ActionResult<IEnumerable<Api.Models.Service>>> GetServices(int specialtyId)
        {
            return await _context.SpecialtyServices.Include(p => p.Service)
                .Where(p => p.SpecialtyId == specialtyId).Select(p => p.Service).ToListAsync();
        }   
        [HttpGet("getspecialtys/{serviceId}")]
        public async Task<ActionResult<IEnumerable<Specialty>>> GetSpecialtys(int serviceId)
        {
            return await _context.SpecialtyServices.Include(p => p.Specialty)
                .Where(p => p.ServiceId == serviceId).Select(p => p.Specialty).ToListAsync();
        }

        // GET: api/SpecialtyServices/5
        [HttpGet("getone")]
        public async Task<ActionResult<SS>> GetSpecialtyService(int serviceID, int specialtyID)
        {
            var specialtyService = await _context.SpecialtyServices.Include(p=>p.Service)
                .Include(p=>p.Specialty)
                .SingleOrDefaultAsync(p=>p.ServiceId==serviceID&&p.SpecialtyId==specialtyID);

            if (specialtyService == null)
            {
                return NotFound();
            }
            var ss = new SS()
            {
                ServiceID = specialtyService.ServiceId,
                SpecialtyID = specialtyService.SpecialtyId,
                ServiceName = specialtyService.Service.Name,
                SpecialtyName = specialtyService.Specialty.Name,
            };
            return ss;
        }

        // PUT: api/SpecialtyServices/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSpecialtyService(int id, SpecialtyService specialtyService)
        {
            if (id != specialtyService.SpecialtyId)
            {
                return BadRequest();
            }

            _context.Entry(specialtyService).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpecialtyServiceExists(id))
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

        // POST: api/SpecialtyServices
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<SpecialtyService>> PostSpecialtyService(SpecialtyService specialtyService)
        {
            _context.SpecialtyServices.Add(specialtyService);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SpecialtyServiceExists(specialtyService.SpecialtyId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetSpecialtyService", new { id = specialtyService.SpecialtyId }, specialtyService);
        }

        // DELETE: api/SpecialtyServices/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<SpecialtyService>> DeleteSpecialtyService(int id)
        {
            var specialtyService = await _context.SpecialtyServices.FindAsync(id);
            if (specialtyService == null)
            {
                return NotFound();
            }

            _context.SpecialtyServices.Remove(specialtyService);
            await _context.SaveChangesAsync();

            return specialtyService;
        }

        private bool SpecialtyServiceExists(int id)
        {
            return _context.SpecialtyServices.Any(e => e.SpecialtyId == id);
        }
    }
}
