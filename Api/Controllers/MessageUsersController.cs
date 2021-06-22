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
using Api.Service;

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
                .Include(p=>p.Sender)
                .Include(p => p.Freelancer)
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
                int count = listMessage.Where(p => p.JobId == item.JobId
                && p.FreelancerId == p.FreelancerId && p.Status == "Unseen").Count();

                MessageUserResponse messageUserResponse = new MessageUserResponse()
                {
                    Job = new ResponseIdName(last.Job),
                    Freelancer = new ResponseIdName(last.Freelancer),
                    ToUser = toUser,
                    LastSender = new ResponseIdName(last.Sender),
                    LastMessage = last.Message1,
                    LastMsgStatus = last.Status,
                    Time = last.Time,
                    UnseenCount = count,
                };

                if(last.Job.Status == "Waiting")
                {
                    messageUserResponse.Status = "In discussion";
                }else if(last.Job.Status == "Cancellation" || last.Job.Status == "Closed" 
                    || last.Job.FreelancerId != last.FreelancerId)
                {
                    messageUserResponse.Status = "Stop discussion";
                }else if(last.Job.Status == "Finished")
                {
                    messageUserResponse.Status = "Finished";
                }
                else
                {
                    messageUserResponse.Status = "In progress";
                }          
                
                list.Add(messageUserResponse);
            }

            return list.OrderByDescending(p=>p.Time).ToList();
        }

        [HttpGet("checkassign")]
        public async Task<ActionResult> CheckAssign(int jobId, int freelancerId )
        {
            string jwt = Request.Headers["Authorization"];
            jwt = jwt.Substring(7);
            var handler = new JwtSecurityTokenHandler();
            var jsonHandler = handler.ReadJwtToken(jwt);
            var tokenS = jsonHandler as JwtSecurityToken;
            var email = tokenS.Claims.SingleOrDefault(claim => claim.Type == "email").Value;
            var account = await _context.Accounts
                .Include(p => p.JobRenters)
                .SingleOrDefaultAsync(p => p.Email == email);
            if (account == null)
            {
                return BadRequest();
            }

            var job = account.JobRenters.SingleOrDefault(p => p.Id == jobId);
            if (job == null)
            {
                return BadRequest();
            }
            if (job.Status == "Waiting" )
            {
                foreach (var item in _context.Messages.Where(p => p.JobId == jobId && p.FreelancerId == freelancerId).ToList())
                {
                    if (item.Type == "SuggestedPrice" && item.Confirmation == null)
                    {
                        return Ok(new { canAssign = false, message = "Bạn đã gửi yêu cầu trước đó rồi, hãy chờ freelancer xác nhận" });
                    }
                }
                return Ok(new { canAssign = true, message = "Bạn có thể giao việc này" });
            }
            return Ok(new { canAssign = false, message = "Bạn không thể giao việc này cho freelancer" });
        }

        [HttpGet("checkrequest")]
        public async Task<ActionResult> CheckRequest(int jobId, int freelancerId)
        {
            string jwt = Request.Headers["Authorization"];
            jwt = jwt.Substring(7);
            var handler = new JwtSecurityTokenHandler();
            var jsonHandler = handler.ReadJwtToken(jwt);
            var tokenS = jsonHandler as JwtSecurityToken;
            var email = tokenS.Claims.SingleOrDefault(claim => claim.Type == "email").Value;
            var account = await _context.Accounts
                .Include(p => p.JobRenters)
                .SingleOrDefaultAsync(p => p.Email == email);
            if (account == null)
            {
                return BadRequest();
            }

            var job = _context.Jobs.SingleOrDefault(p => p.Id == jobId);
            if (job == null)
            {
                return BadRequest();
            }
            var check = _context.Messages.First(p => p.JobId == jobId && p.FreelancerId == freelancerId 
                                         && p.Type == "FinishRequest" && p.Confirmation == null);
            if(check == null)
            {
                return Ok(new { canRequest = true, message = "Bạn có thể yêu cầu kết thúc công việc" });
            }
            return Ok(new { canRequest = false, message = "Bạn không không thể gửi thêm yêu cầu kết thúc công việc" });

        }
    }
}
