using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Api.Enities;
using Api.Helpers;
using Microsoft.AspNetCore.Cors;
using Api.Service;
using Api.Hubs;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [EnableCors]
    public class Admin : ControllerBase
    {
        private readonly FreeLancerVNContext _context;
        public Admin(FreeLancerVNContext context)
        {
            _context = context;
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<DashBoard>> getDashBoard()
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
            var account = await _context.Accounts.SingleOrDefaultAsync(p => p.Email == email);
            if (account == null || account.RoleId != 1)
            {
                return BadRequest();
            }

            var listMonth = _context.Jobs.ToList().OrderByDescending(p => p.CreateAt)
                .Select(p => p.CreateAt.ToString("yyyy-MM"));

            List<string> distinct = listMonth.Distinct().ToList();
            List<TotalMonth> totalMonths = new List<TotalMonth>();

            foreach (var item in distinct)
            {
                int year = Convert.ToInt32(item.Substring(0, 4));
                int month = Convert.ToInt32(item.Substring(5));
                int newjob = _context.Jobs.Where(p => p.CreateAt.Month == month && p.CreateAt.Year == year).Count();
                int assigned = _context.Jobs
                    .Where(p => p.StartAt.GetValueOrDefault().Year == year
                    && p.StartAt.GetValueOrDefault().Month == month).Count();
                int done = _context.Jobs
                    .Where(p => p.FinishAt.GetValueOrDefault().Year == year
                    && p.FinishAt.GetValueOrDefault().Month == month && p.Status == "Finished").Count();
                int unCompeleted = _context.Jobs
                    .Where(p => p.FinishAt.GetValueOrDefault().Year == year
                    && p.FinishAt.GetValueOrDefault().Month == month && p.Status != "Finished").Count();
                int money = _context.Jobs
                    .Where(p => p.Status != "Finished").Sum(p => p.Price);
                totalMonths.Add(new TotalMonth()
                {
                    Month = item,
                    NewJob = newjob,
                    Assigned = assigned,
                    Done = done,
                    Cancelled = unCompeleted,
                    Money = money,
                });
            }
            DashBoard dashBoard = new DashBoard()
            {
                TotalJob = _context.Jobs.Count(),
                TotalAssigned = _context.Jobs.Where(p => p.FreelancerId != null).Count(),
                TotalDone = _context.Jobs.Where(p => p.Status == "Finished").Count(),
                TotalCancelled = _context.Jobs.Where(p => p.Status == "Closed" || p.Status == "Cancellation").Count(),
                TotalUser = _context.Accounts.Count(),
                UserConfirmed = _context.Accounts.Where(p => p.IsAccuracy == true).Count(),
                TotalMonths = totalMonths
            };
            return dashBoard;
        }
        [HttpGet("accounts")]
        public async Task<ActionResult<IEnumerable<AccountForListResponse>>> GetAllAccounts()
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
            var account = await _context.Accounts.SingleOrDefaultAsync(p => p.Email == email);
            if (account == null && account.RoleId != 1) { return BadRequest(); }

            return await _context.Accounts
                .Include(p => p.Specialty)
                .Include(p => p.RatingFreelancers)
                .Include(p => p.Level)
                .Select(p => new AccountForListResponse(p)).ToListAsync();
        }

        [HttpGet("jobs")]
        public async Task<ActionResult<IEnumerable<JobForListResponse>>> getJobs()
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
            var account = await _context.Accounts.SingleOrDefaultAsync(p => p.Email == email);
            if (account == null && account.RoleId != 1) { return BadRequest(); }
            return await _context.Jobs.Include(p => p.Renter).Include(p => p.OfferHistories)
                .Include(p => p.S).ThenInclude(p => p.Specialty).AsSplitQuery()
                .OrderByDescending(p => p.CreateAt)
                .Select(p => new JobForListResponse(p)).ToListAsync();
        }
        //get job request
        [HttpGet("jobrequests")]
        public async Task<ActionResult<IEnumerable<JobForListResponse>>> getJobsRequest()
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
            var account = await _context.Accounts.SingleOrDefaultAsync(p => p.Email == email);
            if (account == null) { return BadRequest(); }
            if (account.RoleId != 1)
            {
                return BadRequest();
            }
            return await _context.Jobs.Include(p => p.Renter).Include(p => p.OfferHistories)
                                 .Include(p => p.S).ThenInclude(p => p.Specialty).AsSplitQuery()
                                 .Where(p => p.Status == "Request rework" || p.Status == "Request cancellation")
                                .OrderByDescending(p => p.CreateAt)
                                .Select(p => new JobForListResponse(p)).ToListAsync();
        } 
        //get job request
        [HttpGet("jobrequests/{id}")]
        public async Task<ActionResult<IEnumerable<JobRequestDetail>>> getJobsRequestById(int id)
        {
            if (_context.Jobs.Find(id) == null)
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
            var account = await _context.Accounts.SingleOrDefaultAsync(p => p.Email == email);
            if (account == null || account.RoleId != 1)
            {
                return BadRequest();
            }
            return await _context.Jobs.Include(p => p.Renter).Include(p => p.OfferHistories)
                                 .Include(p => p.S).ThenInclude(p => p.Specialty).AsSplitQuery()
                                 .Include(p=>p.Messages).ThenInclude(p=>p.Sender).AsSplitQuery()
                                 .Include(p=>p.Messages).ThenInclude(p=>p.Receive).AsSplitQuery()
                                 .Where(p => p.Id == id)
                                .OrderByDescending(p => p.CreateAt)
                                .Select(p => new JobRequestDetail(p)).ToListAsync();
        }
        //put rework .. admin
        [HttpPut("jobs/{id}/rework")]
        public async Task<ActionResult> Reworkjob(int id)
        {
            var job = _context.Jobs.Find(id);
            if (job == null)
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

            var renter = await _context.Accounts
                .SingleOrDefaultAsync(p => p.Email == email);
            if (job.RenterId != renter.Id)
            {
                return BadRequest(new { message = "You dont have this permission" });
            }
            if (job.Status != "Request rework")
            {
                return BadRequest();
            }
            job.Status = "In progress";

            _context.Entry(job).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok();
        }

        //put cancel .. admin
        [HttpPut("jobs/{id}/cancel")]
        public async Task<ActionResult> CancelJob(int id)
        {
            var job = _context.Jobs.Find(id);
            if (job == null)
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

            var renter = await _context.Accounts
                .SingleOrDefaultAsync(p => p.Email == email);
            if (job.RenterId != renter.Id)
            {
                return BadRequest(new { message = "You dont have this permission" });
            }
            if (job.Status != "Request cancellation")
            {
                return BadRequest();
            }
            job.Status = "Cancellation";
            job.FinishAt = TimeVN.Now();
            _context.Entry(job).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("services")]
        public async Task<ActionResult<IEnumerable<ServiceResponse>>> GetServicesadmin()
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
            return await _context.Services
                .Include(p => p.SpecialtyServices).ThenInclude(p => p.Specialty)
                .Where(p => p.IsActive == true)
                .Select(p => new ServiceResponse(p)).ToListAsync();
        }

        [HttpGet("skills")]
        public async Task<ActionResult<IEnumerable<Skill>>> GetSkillsadmin()
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
            return await _context.Skills
                .Select(p => new Skill()
                {
                    Id = p.Id,
                    Name = p.Name,
                    IsActive = p.IsActive
                }).ToListAsync();
        }

        [HttpGet("specialties")]
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
                .Include(p => p.SpecialtyServices)
                .ThenInclude(p => p.Service).ToListAsync();
        }
    }
}
