using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Api.Models;
using Api.Enities;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessageUsersController : ControllerBase
    {
        private readonly FreeLancerVNContext _context;
        public MessageUsersController(FreeLancerVNContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageUserResponse>>> 
            GetMessageUsers([FromBody] MessageUser messageUser )
        {
            string jwt = Request.Headers["Authorization"];
            jwt = jwt.Substring(7);
            var handler = new JwtSecurityTokenHandler();
            var jsonHandler = handler.ReadJwtToken(jwt);
            var tokenS = jsonHandler as JwtSecurityToken;
            var email = tokenS.Claims.SingleOrDefault(claim => claim.Type == "email").Value;
            Account account = await _context.Accounts
                .Include(p=>p.MessageSenders).ThenInclude(p=>p.Job).AsSplitQuery()
                .Include(p=>p.MessageReceives).ThenInclude(p=>p.Job).AsSplitQuery()
                .SingleOrDefaultAsync(p => p.Email == email);
                
            if (account == null) { return NotFound(); }

            var listMessage = _context.Messages.Include(p => p.Freelancer)
                .Include(p=>p.Job).ToList();

            var x = _context.Messages.Include(p=>p.Job)
                .Where(p=>p.FreelancerId == account.Id || p.Job.RenterId == account.Id)
                .GroupBy(p => new { p.JobId, p.FreelancerId })
                .Select(p => new MessageUserResponse
                {
                    Job = new ResponseIdName(_context.Jobs.Find(p.Key.JobId)),
                    Freelancer = new ResponseIdName(_context.Jobs.Find(p.Key.FreelancerId)),
                    LastSender = new ResponseIdName(listMessage.Where(x =>
                    x.JobId == p.Key.JobId
                    && x.FreelancerId == p.Key.FreelancerId)
                    .Select(p => p.Freelancer).Last()),

                    LastMessage = listMessage.Where(x =>
                    x.JobId == p.Key.JobId
                    && x.FreelancerId == p.Key.FreelancerId)
                    .Select(p => p.Message1).Last(),

                    Time = listMessage.Where(x =>
                    x.JobId == p.Key.JobId
                    && x.FreelancerId == p.Key.FreelancerId)
                    .Select(p => p.Time).Last(),

                    Status = listMessage.Where(x =>
                    x.JobId == p.Key.JobId
                    && x.FreelancerId == p.Key.FreelancerId)
                    .Select(p => p.Status).Last()
                }).OrderByDescending(p=>p.Time).ToList();

/*            var chatFreelancer = await _context.Messages
                .Where(p => (p.ReceiveId == account.Id || p.SenderId == account.Id)
                && (p.Job.FreelancerId == null || p.Job.FreelancerId == account.Id))
                .ToListAsync();

            var groupFreelancer = chatFreelancer.GroupBy(p => p.Job)
                .Select(p => new MessageUserResponse()
                {
                    Job = new ResponseIdName(p.Key),
                    LastSender = new ResponseIdName(chatFreelancer
                    .Where(x => x.JobId == p.Key.Id).Last().Sender),
                    LastMessage = chatFreelancer.Where(x => x.JobId == p.Key.Id).Last().Message1,
                    Time = chatFreelancer.Where(x => x.JobId == p.Key.Id).Last().Time,
                });  
            
            var chatRenter = await _context.Messages
                .Where(p => (p.ReceiveId == account.Id || p.SenderId == account.Id)
                && p.Job.RenterId ==account.Id)
                .ToListAsync();

            var groupRenter = chatRenter.GroupBy(p => new (p.Job, p.))
                .Select(p => new MessageUserResponse()
                {
                    Job = new ResponseIdName(p.Key),
                    LastSender = new ResponseIdName(chatFreelancer
                    .Where(x => x.JobId == p.Key.Id).Last().Sender),
                    LastMessage = chatFreelancer.Where(x => x.JobId == p.Key.Id).Last().Message1,
                    Time = chatFreelancer.Where(x => x.JobId == p.Key.Id).Last().Time,
                });*/


            return x;
        }
    }
}
