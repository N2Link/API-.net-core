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
    public class LevelsController : ControllerBase
    {
        private readonly FreeLancerVNContext _context;

        public LevelsController(FreeLancerVNContext context)
        {
            _context = context;
        }

        // GET: api/Levels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResponseIdName>>> GetLevels()
        {
            return await _context.Levels.Select(p=> new ResponseIdName(p)).ToListAsync();
        }

        // GET: api/Levels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseIdName>> GetLevel(int id)
        {
            var level = await _context.Levels.FindAsync(id);

            if (level == null)
            {
                return NotFound();
            }

            return new ResponseIdName(level);
        }

        // PUT: api/Levels/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLevel(int id, Level level)
        {
            if (id != level.Id)
            {
                return BadRequest();
            }

            _context.Entry(level).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LevelExists(id))
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

        // POST: api/Levels
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Level>> PostLevel(Level level)
        {
            _context.Levels.Add(level);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLevel", new { id = level.Id }, level);
        }

        // DELETE: api/Levels/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Level>> DeleteLevel(int id)
        {
            var level = await _context.Levels.FindAsync(id);
            if (level == null)
            {
                return NotFound();
            }

            _context.Levels.Remove(level);
            await _context.SaveChangesAsync();

            return level;
        }

        private bool LevelExists(int id)
        {
            return _context.Levels.Any(e => e.Id == id);
        }
    }
}
