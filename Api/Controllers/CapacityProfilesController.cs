using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Models;
using Api.Enities;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CapacityProfilesController : ControllerBase
    {
        private readonly FreeLancerVNContext _context;
        private IWebHostEnvironment _webHostEnvironment;

        public CapacityProfilesController(FreeLancerVNContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            this._webHostEnvironment = webHostEnvironment;
        }

        // GET: api/CapacityProfiles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CapacityProfileResponse>>> GetCapacityProfiles()
        {
            return await _context.CapacityProfiles
                .Include(p => p.ProfileServices).ThenInclude(p => p.Service)
                .Select(p=> new CapacityProfileResponse(p))
                .ToListAsync();
        }

/*        [HttpGet("freelancer/{freelancerID}")]
        public async Task<ActionResult<IEnumerable<CapacityProfileResponse>>> GetCPListByUserId(int freelancerID)
        {
            return await _context.CapacityProfiles
                        .Include(p => p.ProfileServices).ThenInclude(p => p.Service)
                        .Where(p=>p.FreelancerId==freelancerID)
                        .Select(p => new CapacityProfileResponse(p))
                        .ToListAsync();
        }*/

        // GET: api/CapacityProfiles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CapacityProfileResponse>> GetCapacityProfile(int id)
        {
            var capacityProfile = await _context.CapacityProfiles
                .Include(p => p.ProfileServices).ThenInclude(p => p.Service)
                .SingleOrDefaultAsync(p => p.Id == id);


            if (capacityProfile == null)
            {
                return NotFound();
            }

            return new CapacityProfileResponse(capacityProfile);
        }

        // PUT: api/CapacityProfiles/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("id")]
        public async Task<IActionResult> PutCapacityProfile(int id, CProfilePostModel cpEditModel)
        {
            CapacityProfile capacityProfile = _context.CapacityProfiles
                        .Include(p => p.ProfileServices)
                        .SingleOrDefault(p => p.Id == id);
            if (capacityProfile == null)
            {
                return NotFound();
            }
            String jwt = Request.Headers["Authorization"];
            jwt = jwt.Substring(7);
            //Decode jwt and get payload
            var stream = jwt;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var tokenS = jsonToken as JwtSecurityToken;
            //I can get Claims using:
            var email = tokenS.Claims.First(claim => claim.Type == "email").Value;
            var account = _context.Accounts.SingleOrDefaultAsync(p => p.Email == email);
            if (account == null||account.Id!=capacityProfile.FreelancerId)
            {
                return BadRequest();
            }
            //create image
            string newname = "";
            if (cpEditModel.ImageBase64 != "")
            {
                string rootpath = _webHostEnvironment.WebRootPath;

                newname = cpEditModel.ImageName+"_"+capacityProfile.Id;

                using (FileStream fs = System.IO.File.Create(rootpath +"\\Images"+ newname))
                {
                    fs.Close();
                    System.IO.File.WriteAllBytes(rootpath + "\\Images" + newname, Convert.FromBase64String(cpEditModel.ImageBase64));
                }
                if (capacityProfile.ImageUrl != null)
                {
                    var nameDelete = capacityProfile.ImageUrl
                        .Substring(capacityProfile.ImageUrl.LastIndexOf("/") + 1);
                    try
                    {
                        System.IO.File.Delete(rootpath + "\\Images\\" + nameDelete);
                    }
                    catch (Exception) {}
                }
            }

            capacityProfile.Name = cpEditModel.Name;
            capacityProfile.Description = cpEditModel.Description;
            capacityProfile.Urlweb = cpEditModel.Urlweb;
            capacityProfile.ImageUrl = newname ==""?capacityProfile.ImageUrl:
                "freelancervn.somee.com/api/images/images/" + newname;
            _context.ProfileServices.RemoveRange(capacityProfile.ProfileServices.ToArray());
            await _context.SaveChangesAsync();

            foreach (var item in cpEditModel.Services)
            {
                _context.ProfileServices.Add(new ProfileService
                {
                    Cpid = id,
                    ServiceId = item.Id,
                });
            }

            _context.Entry(capacityProfile).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CapacityProfileExists(id))
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

        // POST: api/CapacityProfiles
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<CapacityProfileResponse>> PostCapacityProfile(CProfilePostModel cProfilePostModel)
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
            Account account = await _context.Accounts.SingleOrDefaultAsync(p=>p.Email == email);
            if (account == null)
            {
                return BadRequest();
            }

            CapacityProfile capacityProfile = new CapacityProfile()
            {
                FreelancerId = account.Id,
                Name = cProfilePostModel.Name,
                Description = cProfilePostModel.Description,
                Urlweb = cProfilePostModel.Urlweb,
            };
            _context.CapacityProfiles.Add(capacityProfile);

            await _context.SaveChangesAsync();


            foreach (var item in cProfilePostModel.Services)
            {
                _context.ProfileServices.Add(new ProfileService
                {
                    Cpid = capacityProfile.Id,
                    ServiceId = item.Id,
                });
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CapacityProfileExists(capacityProfile.FreelancerId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            capacityProfile = _context.CapacityProfiles
                .Include(p => p.ProfileServices).ThenInclude(p => p.Service)
                .SingleOrDefault(p => p.Id == capacityProfile.Id);
            //create image
            string rootpath = _webHostEnvironment.WebRootPath;
            var newname = cProfilePostModel.ImageName +"_"+ capacityProfile.Id;
            using (FileStream fs = System.IO.File.Create(rootpath + "\\Images" + newname))
            {
                fs.Close();
                System.IO.File.WriteAllBytes(rootpath + "\\Images" + newname, Convert.FromBase64String(cProfilePostModel.ImageBase64));
            }
            capacityProfile.ImageUrl = "freelancervn.somee.com/api/images/images/" + newname;
            _context.SaveChanges();
            return Ok(new CapacityProfileResponse(capacityProfile));
        }

        // DELETE: api/CapacityProfiles/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<CapacityProfile>> DeleteCapacityProfile(int id)
        {
            var capacityProfile = await _context.CapacityProfiles.Include(p=>p.ProfileServices)
                .SingleOrDefaultAsync(p=>p.Id == id);
            if (capacityProfile == null)
            {
                return NotFound();
            }
            _context.ProfileServices.RemoveRange(capacityProfile.ProfileServices);
            await _context.SaveChangesAsync();
            _context.CapacityProfiles.Remove(capacityProfile);
            await _context.SaveChangesAsync();
            return capacityProfile;
        }

        private bool CapacityProfileExists(int id)
        {
            return _context.CapacityProfiles.Any(e => e.Id == id);
        }
    }
}
