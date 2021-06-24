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
            if (account == null && account.RoleId != 1) { return BadRequest(); }

            var listMonth = _context.Jobs.ToList().OrderBy(p => p.CreateAt)
                .Select(p => p.CreateAt.ToString("yyyy-MM"));

            List<string> distinct = listMonth.Distinct().ToList();
            List<TotalJobMonth> totalJobMonths = new List<TotalJobMonth>();

            foreach (var item in distinct)
            {
                int year = Convert.ToInt32(item.Substring(0, 4));
                int month = Convert.ToInt32(item.Substring(5));
                int newjob = _context.Jobs.Where(p => p.CreateAt.Month == month && p.CreateAt.Year == year).Count();
                
                int assigned = _context.Jobs
                    .Where(p => p.StartAt!= null)
                    .ToList()
                    .Where(p => DateTime.Parse(p.StartAt.ToString()).Year == year
                    && DateTime.Parse(p.StartAt.ToString()).Month == month).Count();

                int done = _context.Jobs
                    .Where(p => p.FinishAt!=null && p.Status == "Finished")
                    .ToList()
                    .Where(p=> DateTime.Parse(p.FinishAt.ToString()).Year == year
                    && DateTime.Parse(p.FinishAt.ToString()).Month == month).Count();

                int unCompeleted = _context.Jobs
                    .Where(p => p.FinishAt != null && p.Status != "Finished")
                    .ToList()
                    .Where(p=>DateTime.Parse(p.FinishAt.ToString()).Year == year
                    && DateTime.Parse(p.FinishAt.ToString()).Month == month ).Count();

                int money = _context.Jobs
                    .Where(p => p.FinishAt != null && p.Status == "Finished")
                    .ToList()
                    .Where(p => DateTime.Parse(p.FinishAt.ToString()).Year == year
                    && DateTime.Parse(p.FinishAt.ToString()).Month == month
                    ).Sum(p => p.Price);


                totalJobMonths.Add(new TotalJobMonth()
                {
                    Month = item,
                    NewJob = newjob,
                    Assigned = assigned,
                    Done = done,
                    Cancelled = unCompeleted,
                    Money = money,
                });
            }
            listMonth = _context.Accounts.ToList().OrderBy(p => p.CreatedAtDate)
                .Select(p => p.CreatedAtDate.ToString("yyyy-MM"));
            distinct = listMonth.Distinct().ToList();

            List<TotalUserMonth> totalUserMonths = new List<TotalUserMonth>();
            foreach (var item in distinct)
            {
                int year = Convert.ToInt32(item.Substring(0, 4));
                int month = Convert.ToInt32(item.Substring(5));
                int userCount = _context.Accounts.Where(p => p.CreatedAtDate.Year == year
                                && p.CreatedAtDate.Month == month).Count();
                totalUserMonths.Add(new TotalUserMonth()
                {
                    Month = item,
                    NewUser = userCount,
                });

            }

            DashBoard dashBoard = new DashBoard()
            {
                TotalJob = _context.Jobs.Count(),
                TotalAssigned = _context.Jobs.Where(p => p.FreelancerId != null).Count(),
                TotalDone = _context.Jobs.Where(p => p.Status == "Finished").Count(),
                TotalCancelled = _context.Jobs.Where(p => p.Status == "Closed" || p.Status == "Cancellation").Count(),
                TotalUser = _context.Accounts.Count(),
                TotalJobMonths = totalJobMonths,
                TotalUserMonths = totalUserMonths,
                
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
            var account = await _context.Accounts.SingleOrDefaultAsync(p => p.Email == email && p.RoleId == 1);
            if (account == null) { return BadRequest(); }

            return await _context.Accounts
                .Include(p => p.Specialty)
                .Include(p => p.RatingFreelancers)
                .Include(p => p.Level)
                .Select(p => new AccountForListResponse(p)).ToListAsync();
        }        
        
        [HttpDelete("accounts/{id}")]
        public async Task<ActionResult> DeteleAccount(int id)
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

            var user = _context.Accounts.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            user.BannedAtDate = TimeVN.Now();
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("accouns/search")]
        public async Task<ActionResult<IEnumerable<AccountForListResponse>>>
    GetListSearch(string search, int specialtyId, int serviceId,
    string provinceId, int levelId)
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
            var account = await _context.Accounts.SingleOrDefaultAsync(p => p.Email == email && p.RoleId == 1);
            if (account == null) { return BadRequest(); }

            var list = await
                _context.Accounts
                .Include(p => p.Specialty)
                .Include(p => p.RatingFreelancers)
                .Include(p => p.Level)
                .Where(p => (search == null || p.Name.Contains(search))
                && (provinceId == "00" || p.ProvinceId == provinceId)
                && (levelId == 0 || p.LevelId == levelId)
                && (specialtyId == 0 || p.SpecialtyId == specialtyId)
                && (serviceId == 0
                || p.FreelancerServices.Select(x => x.Service)
                    .Where(x => x.IsActive == true).Select(p => p.Id)
                    .ToList().Contains(serviceId))
                && p.BannedAtDate == null && p.Id != account.Id)
                .Select(p => new AccountForListResponse(p))
                .ToListAsync();
            return list;
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
        [HttpGet("jobs/search")]
        public async Task<ActionResult<IEnumerable<JobForListResponse>>> GetListSearch(
    string search,
    int floorPrice, int cellingPrice,
    int specialtyId, int serviceId, int payFormId, int formOfWorkId, int typeOfWorkId,
    string provinceId)
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
            if (account == null) { return NotFound(); }

            var list = await _context.Jobs.Include(p => p.OfferHistories)
                .Include(p => p.S).ThenInclude(p => p.Specialty).AsSplitQuery()
                .Where(p => (search == null || p.Name.Contains(search))
                && p.Floorprice >= floorPrice
                && p.Cellingprice <= cellingPrice
                && (specialtyId == 0 || p.SpecialtyId == specialtyId)
                && (serviceId == 0 || p.ServiceId == serviceId)
                && (payFormId == 0 || p.PayformId == payFormId)
                && (provinceId == "00" || p.ProvinceId == provinceId)
                && (formOfWorkId == 0 || p.FormId == formOfWorkId)
                && (typeOfWorkId == 0 || p.TypeId == typeOfWorkId))
                .OrderByDescending(p => p.CreateAt)
                .Select(p => new JobForListResponse(p))
                .ToListAsync();
            return list;
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
                                 .Include(p => p.S).ThenInclude(p => p.Specialty).AsSplitQuery()
                                 .Include(p => p.Freelancer).AsSplitQuery()
                                 .Where(p => p.Status == "Request rework" || p.Status == "Request cancellation")
                                .OrderByDescending(p => p.CreateAt)
                                .Select(p => new JobForListResponse(p)).ToListAsync();
        } 
        //get job request
        [HttpGet("jobrequests/{id}")]
        public async Task<ActionResult<IEnumerable<MessageResponse>>> getJobsRequestById(int id)
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
            var account = await _context.Accounts.SingleOrDefaultAsync(p => p.Email == email);
            if (account == null || account.RoleId != 1)
            {
                return BadRequest();
            }

            return await _context.Messages.Include(p=>p.Job)
                        .Include(p=>p.Freelancer)
                        .Include(p=>p.Sender)
                        .Include(p=>p.Receive)
                        .Where(p => p.JobId == job.Id&& p.FreelancerId == job.FreelancerId)
                        .OrderBy(p => p.Time)
                        .Select(p => new MessageResponse(p))
                        .ToListAsync();
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
            if (admin == null) { return BadRequest(); }
            return await _context.Services.ToListAsync();
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
