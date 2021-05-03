using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Models;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FreelancerServicesController : ControllerBase
    {
        private readonly FreeLancerVNContext _context;

        public FreelancerServicesController(FreeLancerVNContext context)
        {
            _context = context;
        }

        // GET: api/FreelancerServices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FreelancerService>>> GetFreelancerServices()
        {
            return await _context.FreelancerServices.ToListAsync();
        }

        // GET: api/FreelancerServices/5
        [HttpGet("getone")]
        public async Task<ActionResult<FreelancerService>> GetFreelancerService(int freelancerID, int serviceID)
        {
            var freelancerService = await _context.FreelancerServices
                .SingleOrDefaultAsync(p=>p.FreelancerId == freelancerID && p.ServiceId == serviceID);

            if (freelancerService == null)
            {
                return NotFound();
            }
            return freelancerService;
        }

        // PUT: api/FreelancerServices/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFreelancerService(int id, FreelancerService freelancerService)
        {
            if (id != freelancerService.FreelancerId)
            {
                return BadRequest();
            }

            _context.Entry(freelancerService).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FreelancerServiceExists(id))
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

        // POST: api/FreelancerServices
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<FreelancerService>> PostFreelancerService(FreelancerService freelancerService)
        {
            _context.FreelancerServices.Add(freelancerService);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FreelancerServiceExists(freelancerService.FreelancerId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetFreelancerService", new { id = freelancerService.FreelancerId }, freelancerService);
        }

        // DELETE: api/FreelancerServices/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<FreelancerService>> DeleteFreelancerService(int id)
        {
            var freelancerService = await _context.FreelancerServices.FindAsync(id);
            if (freelancerService == null)
            {
                return NotFound();
            }

            _context.FreelancerServices.Remove(freelancerService);
            await _context.SaveChangesAsync();

            return freelancerService;
        }

        private bool FreelancerServiceExists(int id)
        {
            return _context.FreelancerServices.Any(e => e.FreelancerId == id);
        }
    }
}
