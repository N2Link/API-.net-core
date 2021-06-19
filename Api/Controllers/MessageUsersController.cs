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
        public async Task<ActionResult<IEnumerable<MessageUserResponse>>> GetMessageUsers( )
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

            var listMessage = _context.Messages
                .Include(p=>p.Sender).AsSplitQuery()
                .Include(p => p.Freelancer).AsSplitQuery()
                .Include(p=>p.Job).ThenInclude(p=>p.Renter).AsSplitQuery().ToList();

            var listDistinct = _context.Messages.Include(p => p.Job)
                .Where(p => p.FreelancerId == account.Id || p.Job.RenterId == account.Id)
                .Select(p => new { p.JobId, p.FreelancerId }).ToList();

            listDistinct = listDistinct.Distinct().ToList();

            List<MessageUserResponse> list = new List<MessageUserResponse>();

            foreach (var item in listDistinct)
            {
                var last = listMessage.Where(p => p.JobId == item.JobId
                && p.FreelancerId == item.FreelancerId).Last();

                AccountForListResponse toUser = account.Id == last.FreelancerId
                                    ? new AccountForListResponse(last.Job.Renter)
                                    : new AccountForListResponse(last.Freelancer);

                MessageUserResponse messageUserResponse = new MessageUserResponse()
                {
                    Job = new ResponseIdName(last.Job),
                    Freelancer = new ResponseIdName(last.Freelancer),
                    ToUser = toUser,
                    LastSender = new ResponseIdName(last.Sender),
                    LastMessage = last.Message1,
                    Status = last.Status,
                    Time = last.Time,
                };
                list.Add(messageUserResponse);
            }

            return list;
        }
    }
}
