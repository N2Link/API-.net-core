using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Api.Enities;
using Microsoft.AspNetCore.Cors;

namespace Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]

    public class AnnouncementsController : ControllerBase
    {
        private readonly FreeLancerVNContext _context;

        public AnnouncementsController(FreeLancerVNContext context)
        {
            _context = context;
        }

        // GET: api/Announcements
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Announcement>>> GetAnnouncements()
        {
            return await _context.Announcements.ToListAsync();
        }

        // GET: api/Announcements/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Announcement>> GetAnnouncement(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);

            if (announcement == null)
            {
                return NotFound();
            }

            return announcement;
        }

        [HttpPost]
        public async Task<ActionResult<Announcement>> PostAnnouncement(AnnouncementPostModel announcementPostModel)
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
            var account = _context.Accounts.SingleOrDefault(p=>p.Email == email
            &&p.RoleId == 1);
            if (account == null)
            {
                return BadRequest();
            }
            Announcement announcement = new Announcement()
            {
                Title = announcementPostModel.Title,
                Detail = announcementPostModel.Detail,
            };
            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();
            foreach (var AccountID in announcementPostModel.AccountIDs)
            {
                _context.AnnouncementAccounts.Add(new AnnouncementAccount()
                {
                    AccountId = AccountID,
                    AnnouncementId = announcement.Id
                });
            }
            await _context.SaveChangesAsync();
            return Ok();
        }
        private bool AnnouncementExists(int id)
        {
            return _context.Announcements.Any(e => e.Id == id);
        }
    }
}
