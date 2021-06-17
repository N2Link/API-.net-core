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
using Microsoft.AspNetCore.Cors;
using System.IdentityModel.Tokens.Jwt;

namespace Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]

    public class MessagesController : ControllerBase
    {
        private readonly FreeLancerVNContext _context;

        public MessagesController(FreeLancerVNContext context)
        {
            _context = context;
        }

        [HttpGet("pagination/{id}")]
        public async Task<ActionResult<IEnumerable<MessageResponse>>> 
            GetMessage( int id,int jobId, int freelancerId)
        {
            var lastMessage = _context.Messages.Find(id);

            return await _context.Messages.Where(p => p.JobId == jobId
            && p.FreelancerId == freelancerId && p.Time<lastMessage.Time
            && p.Id != lastMessage.Id)
            .OrderByDescending(p=>p.Time)
            .Take(20)
            .Select(p => new MessageResponse(p))
            .ToListAsync();
        }     
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageResponse>>> 
            GetMessage(int jobId, int freelancerId)
        {
            return await _context.Messages.Where(p => p.JobId == jobId
            && p.FreelancerId == freelancerId)
            .OrderByDescending(p=>p.Time)
            .Take(20)
            .Select(p => new MessageResponse(p))
            .ToListAsync();
        }
        [HttpPut("seen")]
        public async Task<ActionResult> SeenMessage( int jobId, int freelancerId)
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
            var account = await _context.Accounts.Include(p=>p.JobRenters)
                .SingleOrDefaultAsync(p => p.Email == email);
            if(account.Id!=freelancerId
                &&(account.JobRenters.Count == 0|| !account.JobRenters
                .Select(p => p.Id).Contains(jobId)))
            {
                return BadRequest();
            }

            foreach (var msg in _context.Messages.Where(p=>p.JobId == jobId 
            && p.FreelancerId ==freelancerId && p.Status == "Unseen"
            && p.ReceiveId == account.Id).ToList())
            {
                msg.Status = "Seen";
            }
            await _context.SaveChangesAsync();
            return Ok();
        }
        private bool MessageExists(int id)
        {
            return _context.Messages.Any(e => e.Id == id);
        }
    }
}
