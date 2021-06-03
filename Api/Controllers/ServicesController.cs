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
using System.IdentityModel.Tokens.Jwt;

namespace Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly FreeLancerVNContext _context;

        public ServicesController(FreeLancerVNContext context)
        {
            _context = context;
        }

        // GET:
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResponseIdName>>> GetServices()
        {
            return await _context.Services
                .Where(p=>p.IsActive==true)
                .Select(p=> new ResponseIdName(p)).ToListAsync();
        }    
        [HttpGet("adminmode")]
        public async Task<ActionResult<IEnumerable<Api.Models.Service>>> GetServicesadmin()
        {
            String jwt = Request.Headers["Authorization"];
            jwt = jwt.Substring(7);
            //Decode jwt and get payload
            var stream = jwt;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var tokenS = jsonToken as JwtSecurityToken;
            //I can get Claims using:
            var email = tokenS.Claims.First(claim => claim.Type == "email").Value;
            var admin = await _context.Accounts.SingleOrDefaultAsync(p => p.Email == email && p.RoleId == 1);
            if(admin == null) { return BadRequest(); }
            return await _context.Services
                .Select(p=> new Api.Models.Service() 
                {
                    Id = p.Id, Name = p.Name, IsActive = p.IsActive 
                }).ToListAsync();
        }
        //GET Specialtys
        [HttpGet("{id}/specialtys")]
        public async Task<ActionResult<List<ResponseIdName>>> GetSpecialyu(int id)
        {
            var service = await _context.Services
                .Include(p=>p.SpecialtyServices)
                .ThenInclude(p=>p.Specialty).SingleOrDefaultAsync(p=>p.Id ==id);

            if (service == null)
            {
                return NotFound();
            }
            var specialtys = service.SpecialtyServices
                .Select(p => p.Specialty).Where(p=>p.IsActive == true)
                .Select(p => new ResponseIdName(p)).ToList();

            return specialtys;
        }
        // GET: api/Services/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseIdName>> GetService(int id)
        {
            var service = await _context.Services.FindAsync(id);

            if (service == null)
            {
                return NotFound();
            }

            return new ResponseIdName(service);
        }

        // PUT: api/Services/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutService(int id, [FromBody]string name)
        {
            var service = _context.Services.Find(id);
            service.Name = name;
            _context.Entry(service).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceExists(id))
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

        // POST: api/Services
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Api.Models.Service>> PostService([FromBody]string name)
        {
            _context.Services.Add(new Models.Service() {Name = name, IsActive = true });
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/Services/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Api.Models.Service>> DeleteService(int id)
        {
            String jwt = Request.Headers["Authorization"];
            jwt = jwt.Substring(7);
            //Decode jwt and get payload
            var stream = jwt;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var tokenS = jsonToken as JwtSecurityToken;
            //I can get Claims using:
            var email = tokenS.Claims.First(claim => claim.Type == "email").Value;
            var admin = await _context.Accounts
                .SingleOrDefaultAsync(p => p.Email == email && p.RoleId == 1);
            if (admin == null) { return BadRequest(); }

            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            service.IsActive = false;
            await _context.SaveChangesAsync();

            return service;
        }

        private bool ServiceExists(int id)
        {
            return _context.Services.Any(e => e.Id == id);
        }
    }
}
