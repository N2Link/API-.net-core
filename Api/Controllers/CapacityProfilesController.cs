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
        public async Task<ActionResult<IEnumerable<CapacityProfile>>> GetCapacityProfiles()
        {
            return await _context.CapacityProfiles.ToListAsync();
        }

        // GET: api/CapacityProfiles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CapacityProfile>> GetCapacityProfile(int id)
        {
            var capacityProfile = await _context.CapacityProfiles.FindAsync(id);

            if (capacityProfile == null)
            {
                return NotFound();
            }

            return capacityProfile;
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
            string imageUrl = _webHostEnvironment.WebRootPath;
            string newURL = "\\Images\\" + capacityProfile.Id +"_"+ cpEditModel.ImageName;
            using (FileStream fs = System.IO.File.Create(imageUrl + newURL))
            {
                System.IO.File.WriteAllBytes(imageUrl + newURL, Convert.FromBase64String(cpEditModel.ImageBase64));
            }
            if (capacityProfile.ImageUrl != null)
            {
                try
                {
                    System.IO.File.Delete(imageUrl + capacityProfile.ImageUrl);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            capacityProfile.Name = cpEditModel.Name;
            capacityProfile.Description = cpEditModel.Description;
            capacityProfile.Urlweb = cpEditModel.Urlweb;
            capacityProfile.ImageUrl = newURL;
            _context.ProfileServices.RemoveRange(capacityProfile.ProfileServices.ToArray());
            await _context.SaveChangesAsync();

            foreach (var item in cpEditModel.ServiceIds)
            {
                _context.ProfileServices.Add(new ProfileService
                {
                    Cpid = id,
                    ServiceId = item,
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
        public async Task<ActionResult<CProfilePostModel>> PostCapacityProfile(CProfilePostModel cProfilePostModel)
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
            var account = _context.Accounts.SingleOrDefaultAsync(p=>p.Email == email);
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

            //create image
            string imageUrl = _webHostEnvironment.WebRootPath;
            string newURL = "\\Images\\"+ capacityProfile.Id  + "_" + cProfilePostModel.ImageName;
            using (FileStream fs = System.IO.File.Create(imageUrl + newURL))
            {
                fs.Close();
                System.IO.File.WriteAllBytes(imageUrl + newURL, Convert.FromBase64String(cProfilePostModel.ImageBase64));
            }
            capacityProfile.ImageUrl = newURL;

            foreach (var item in cProfilePostModel.ServiceIds)
            {
                _context.ProfileServices.Add(new ProfileService
                {
                    Cpid = capacityProfile.Id,
                    ServiceId = item,
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

            return Ok(capacityProfile);
        }

        // DELETE: api/CapacityProfiles/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<CapacityProfile>> DeleteCapacityProfile(int id)
        {
            var capacityProfile = await _context.CapacityProfiles.FindAsync(id);
            if (capacityProfile == null)
            {
                return NotFound();
            }

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
