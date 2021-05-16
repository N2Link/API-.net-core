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
    public class FreelancerSkillsController : ControllerBase
    {
        private readonly FreeLancerVNContext _context;

        public FreelancerSkillsController(FreeLancerVNContext context)
        {
            _context = context;
        }

        // GET: api/FreelancerSkills
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FreelancerSkill>>> GetFreelancerSkills()
        {
            return await _context.FreelancerSkills.ToListAsync();
        }

        // GET: api/FreelancerSkills/5
        [HttpGet("{freelancerId}")]
        public async Task<ActionResult> GetFreelancerSkill(int freelancerId)
        {
            var freelancerSkills = await _context.FreelancerSkills.Include(p=>p.Skill).Where(p=>p.FreelancerId == freelancerId).ToListAsync();

            if (freelancerSkills == null)
            {
                return NotFound();
            }

            return Ok(freelancerSkills.Select(p=>new FreelancerSkill() 
                {
                    FreelancerId = freelancerId,
                    Skill = new Skill() {Id = p.Skill.Id, Name = p.Skill.Name } 
                 }));
        }

        // PUT: api/FreelancerSkills/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFreelancerSkill(int id, FreelancerSkill freelancerSkill)
        {
            if (id != freelancerSkill.FreelancerId)
            {
                return BadRequest();
            }

            _context.Entry(freelancerSkill).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FreelancerSkillExists(id))
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

        // POST: api/FreelancerSkills
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<FreelancerSkill>> PostFreelancerSkill(FreelancerSkill freelancerSkill)
        {
            _context.FreelancerSkills.Add(freelancerSkill);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FreelancerSkillExists(freelancerSkill.FreelancerId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetFreelancerSkill", new { id = freelancerSkill.FreelancerId }, freelancerSkill);
        }

        // DELETE: api/FreelancerSkills/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<FreelancerSkill>> DeleteFreelancerSkill(int id)
        {
            var freelancerSkill = await _context.FreelancerSkills.FindAsync(id);
            if (freelancerSkill == null)
            {
                return NotFound();
            }

            _context.FreelancerSkills.Remove(freelancerSkill);
            await _context.SaveChangesAsync();

            return freelancerSkill;
        }

        private bool FreelancerSkillExists(int id)
        {
            return _context.FreelancerSkills.Any(e => e.FreelancerId == id);
        }
    }
}
