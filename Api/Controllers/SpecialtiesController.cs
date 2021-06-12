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
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SpecialtiesController : ControllerBase
    {
        private readonly FreeLancerVNContext _context;
        private IWebHostEnvironment _webHostEnvironment;
        private string rootpath;

        public SpecialtiesController(FreeLancerVNContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            rootpath = _webHostEnvironment.WebRootPath;
        }

        // GET: api/Specialties
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Specialty>>> GetSpecialties()
        {
            return await _context.Specialties
                .Where(p => p.IsActive == true)
                .Select(p => new Specialty()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Image = p.Image,
                    IsActive = true
                })
                .ToListAsync();
        }

        [HttpGet("adminmode")]
        public async Task<ActionResult<IEnumerable<Specialty>>> GetSpecialtiesadmin()
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
            if (admin == null) { return BadRequest(); }
            return await _context.Specialties
                .Select(p => new Specialty()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Image = p.Image
                }).ToListAsync();
        }
        //GET Specialtys
        [HttpGet("{id}/services")]
        public async Task<ActionResult<List<ResponseIdName>>> GetSpecialyu(int id)
        {
            var specialty = await _context.Specialties
                .Include(p => p.SpecialtyServices)
                .ThenInclude(p => p.Service).SingleOrDefaultAsync(p => p.Id == id);

            if (specialty == null)
            {
                return NotFound();
            }
            var services = specialty.SpecialtyServices
                .Select(p => p.Service).Where(p => p.IsActive == true)
                .Select(p => new ResponseIdName(p)).ToList();

            return services;
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
            specialty.Accounts = null;
            specialty.SpecialtyServices = null;
            return specialty;
        }

        // PUT: api/Specialties/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSpecialty(int id, SpecialtyPModel specialtyPutModel)
        {
            var specialty = await _context.Specialties.Include(p=>p.SpecialtyServices)
                .SingleOrDefaultAsync(p=>p.Id == id);
            if (specialty == null)
            {
                return NotFound();
            }
            //create image
            string newname = "";
            if (specialtyPutModel.ImageBase64 != "")
            {
                string rootpath = _webHostEnvironment.WebRootPath;

                var nameDelete = specialty.Image
                    .Substring(specialty.Image.LastIndexOf("/") + 1);
                try
                {
                    System.IO.File.Delete(rootpath + "\\Images\\" + nameDelete);
                }
                catch (Exception) { }

                newname = specialtyPutModel.ImageName + "_"+id;

                using (FileStream fs = System.IO.File.Create(rootpath + "\\Assets\\" + newname))
                {
                    fs.Close();
                    System.IO.File.WriteAllBytes(rootpath + "\\Images" + newname, Convert.FromBase64String(specialtyPutModel.ImageBase64));
                }
                specialty.Image = "freelancervn.somee.com/api/images/assets/" + newname;
            }


            specialty.Name = specialtyPutModel.Name;

            List<int> check = new List<int>();
            check = specialtyPutModel.Services.Select(p => p.Id).ToList();
            //unActive 
            foreach (var item in specialty.SpecialtyServices.Where(p => p.IsActive == true).ToList())
            {
                if (!check.Contains(item.SpecialtyId))
                {
                    item.IsActive = false;
                }
            }
            check = specialty.SpecialtyServices.Where(p => p.IsActive == true)
                    .Select(p => p.ServiceId).ToList();
            //active new
            foreach (var item in specialtyPutModel.Services.ToList())
            {
                if (!check.Contains(item.Id))
                {
                    _context.SpecialtyServices.Add(new SpecialtyService()
                    {
                        SpecialtyId = id,
                        ServiceId = item.Id,
                        IsActive = true
                    });
                }
            }
            _context.Entry(specialtyPutModel).State = EntityState.Modified;

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

            return Ok();
        }

        // POST: api/Specialties
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Specialty>> PostSpecialty(SpecialtyPModel specialtyPostModel)
        {
            var specialty = new Specialty() { Name = specialtyPostModel.Name };
            _context.Specialties.Add(specialty);
            await _context.SaveChangesAsync();
            string newname = specialtyPostModel.Name + "_" + specialty.Id;
            using (FileStream fs = System.IO.File.Create(rootpath + newname))
            {
                fs.Close();
                System.IO.File.WriteAllBytes(rootpath + "\\Images\\" +newname,
                    Convert.FromBase64String(specialtyPostModel.ImageBase64));
            }
            specialty.Name = specialtyPostModel.Name;
            specialty.Image = "freelancervn.somee.com/api/images/assets/" + newname;
            await _context.SaveChangesAsync();
            foreach (var item in specialtyPostModel.Services.ToList())
            {
                _context.SpecialtyServices.Add(new SpecialtyService()
                {
                    SpecialtyId = specialty.Id,
                    ServiceId = item.Id,
                    IsActive = true
                });
            }
            await _context.SaveChangesAsync();
            return specialty;
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

            specialty.IsActive = false;
            await _context.SaveChangesAsync();

            return specialty;
        }

        private bool SpecialtyExists(int id)
        {
            return _context.Specialties.Any(e => e.Id == id);
        }
    }
}
