using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Api.Enities;
using Api.Helpers;
using Microsoft.AspNetCore.Cors;
using Api.Service;

namespace Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]

    public class AccountsController : ControllerBase
    {
        private readonly FreeLancerVNContext _context;
        public AccountsController(FreeLancerVNContext context)
        {
            _context = context;
        }

        // GET: api/Accounts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountForListResponse>>> GetAccounts()
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

            return await _context.Accounts
                .Include(p => p.Specialty)
                .Include(p => p.RatingFreelancers)
                .Include(p => p.Level)
                .Where(p=>p.BannedAtDate==null && p.Id!= account.Id)
                .Select(p => new AccountForListResponse(p)).ToListAsync();
        }  
        [HttpGet("search")]
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
            var account = await _context.Accounts
                .Include(p=>p.FreelancerServices).ThenInclude(p=>p.Service)
                .SingleOrDefaultAsync(p => p.Email == email);
            if (account == null) { return NotFound(); }

            var list = await
                _context.Accounts
                .Include(p => p.Specialty)
                .Include(p => p.RatingFreelancers)
                .Include(p => p.Level)
                .Where(p => (search==null || p.Name.Contains(search))
                &&(provinceId == "00"|| p.ProvinceId == provinceId)
                &&(levelId == 0||p.LevelId==levelId)
                &&(specialtyId == 0||p.SpecialtyId == specialtyId)
                &&(serviceId == 0
                ||p.FreelancerServices.Select(x=>x.Service)
                    .Where(x=>x.IsActive==true).Select(p=>p.Id)                
                    .ToList().Contains(serviceId))
                && p.BannedAtDate == null && p.Id != account.Id)
                .Select(p=> new AccountForListResponse(p))
                .ToListAsync();
            return list;
        }

        //Get listjob cho freelancer
        [HttpGet("{id}/jobfreelancers")]
        public async Task<ActionResult<IEnumerable<JobForListResponse>>> Getjobfreelancers(int id)
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
            var account = await _context.Accounts
                .Include(p=>p.JobFreelancers).ThenInclude(p=>p.Renter)
                .Include(p => p.JobFreelancers).ThenInclude(p => p.OfferHistories)
                .Include(p=>p.JobFreelancers).ThenInclude(p=>p.S).ThenInclude(p=>p.Specialty).AsSplitQuery()
                .SingleOrDefaultAsync(p => p.Id == id && p.Email == email);
            if (account == null)
            {
                return NotFound();
            }
            var jobFreelancers = account.JobFreelancers.Select(p=> new JobForListResponse(p)).ToList();
            return jobFreelancers;
        }             
        [HttpGet("{id}/jobfreelancers/inprogress")]
        public async Task<ActionResult<IEnumerable<JobForListResponse>>> Getjobfreelancerswaiting(int id)
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
            var account = await _context.Accounts
                .Include(p=>p.JobFreelancers).ThenInclude(p=>p.Renter)
                .Include(p => p.JobFreelancers).ThenInclude(p => p.OfferHistories)
                .Include(p => p.JobFreelancers).ThenInclude(p => p.S).ThenInclude(p=>p.Specialty).AsSplitQuery()
                .SingleOrDefaultAsync(p => p.Id == id && p.Email == email);
            if (account == null)
            {
                return NotFound();
            }
            var jobFreelancers = account.JobFreelancers
                .Where(p=>p.Status == "In progress"
                || p.Status == "Request rework"
                || p.Status == "Request cancellation")
                .Select(p=> new JobForListResponse(p)).ToList();

            return jobFreelancers;
        }      


        [HttpGet("{id}/jobfreelancers/past")]
        public async Task<ActionResult<IEnumerable<JobForListResponse>>> Getjobfreelancerspast(int id)
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
            var account = await _context.Accounts
                .Include(p => p.JobFreelancers).ThenInclude(p => p.Renter)
                .Include(p => p.JobFreelancers).ThenInclude(p => p.OfferHistories)
                .Include(p => p.JobFreelancers).ThenInclude(p => p.S).ThenInclude(p => p.Specialty).AsSplitQuery()
                .SingleOrDefaultAsync(p => p.Id == id && p.Email == email);
            if (account == null)
            {
                return NotFound();
            }
            var jobFreelancers = account.JobFreelancers
                .Where(p=>p.Status == "Finished" || p.Status =="Cancellation" )
                .Select(p=> new JobForListResponse(p)).ToList();
            return jobFreelancers;
        }       

        //get list job renter
        [HttpGet("{id}/jobrenters")]
        public async Task<ActionResult<IEnumerable<JobForListResponse>>> Getjobrenters(int id)
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
            var account = await _context.Accounts
                .Include(p => p.JobRenters).ThenInclude(p => p.Freelancer) 
                .Include(p => p.JobRenters).ThenInclude(p => p.OfferHistories)
                .Include(p => p.JobRenters).ThenInclude(p => p.S).ThenInclude(p => p.Specialty).AsSplitQuery()

                .SingleOrDefaultAsync(p => p.Id == id && p.Email == email);
            if (account == null)
            {
                return NotFound();
            }
            var JobRenters = account.JobRenters.Select(p=> new JobForListResponse(p)).ToList();
            return JobRenters;
        } 
        [HttpGet("{id}/jobrenters/waiting")]
        public async Task<ActionResult<IEnumerable<JobForListResponse>>> Getjobrenterswaiting(int id)
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
            var account = await _context.Accounts
                .Include(p => p.JobRenters).ThenInclude(p => p.Freelancer)
                .Include(p => p.JobRenters).ThenInclude(p => p.OfferHistories)
                .Include(p => p.JobRenters).ThenInclude(p => p.S).ThenInclude(p => p.Specialty).AsSplitQuery()
                .SingleOrDefaultAsync(p => p.Id == id && p.Email == email);
            if (account == null)
            {
                return NotFound();
            }
            var JobRenters = account.JobRenters
                .Where(p=> p.Status =="Waiting")
                .Select(p=> new JobForListResponse(p)).ToList();
            return JobRenters;
        }    
        [HttpGet("{id}/jobrenters/inprogress")]
        public async Task<ActionResult<IEnumerable<JobForListResponse>>> Getjobrentersinprogress(int id)
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
            var account = await _context.Accounts
                .Include(p => p.JobRenters).ThenInclude(p => p.Freelancer)
                .Include(p => p.JobRenters).ThenInclude(p => p.OfferHistories)
                .Include(p => p.JobRenters).ThenInclude(p => p.S).ThenInclude(p => p.Specialty).AsSplitQuery()
                .SingleOrDefaultAsync(p => p.Id == id && p.Email == email);
            if (account == null)
            {
                return NotFound();
            }
            var JobRenters = account.JobRenters
                .Where(p=>p.Status =="In progress")
                .Select(p=> new JobForListResponse(p)).ToList();
            return JobRenters;
        }    
        [HttpGet("{id}/jobrenters/past")]
        public async Task<ActionResult<IEnumerable<JobForListResponse>>> Getjobrenterspast(int id)
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
            var account = await _context.Accounts
                .Include(p => p.JobRenters).ThenInclude(p => p.Freelancer)
                .Include(p => p.JobRenters).ThenInclude(p => p.OfferHistories)
                .Include(p => p.JobRenters).ThenInclude(p => p.S).ThenInclude(p => p.Specialty).AsSplitQuery()
                .SingleOrDefaultAsync(p => p.Id == id && p.Email == email);
            if (account == null)
            {
                return NotFound();
            }
            var JobRenters = account.JobRenters
                .Where(p => p.Status == "Finished" || p.Status == "Closed" || p.Status =="Cancellation")
                .Select(p=> new JobForListResponse(p)).ToList();
            return JobRenters;
        }
        //get ratings freelancer
        [HttpGet("{id}/ratings")]
        public async Task<ActionResult<IEnumerable<RatingResponse>>> GetRatingsfreelancer(int id)
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

            var account = await _context.Accounts
                .Include(p=>p.RatingFreelancers).ThenInclude(p=>p.Renter)
                .Include(p=>p.RatingFreelancers).ThenInclude(p=>p.Jobs)
                .SingleOrDefaultAsync(p => p.Email == email);

            if (account == null) { return NotFound(); }
            return account.RatingFreelancers
                .Select(p => new RatingResponse(p)).ToList();
        }
        //get skill
        [HttpGet("{id}/skills")]
        public async Task<ActionResult<IEnumerable<ResponseIdName>>> GetFreelancerSkills(int id)
        {
            Account account = await _context.Accounts
                .Include(p=>p.FreelancerSkills).ThenInclude(p=>p.Skill)
                .SingleOrDefaultAsync(p=>p.Id ==id);
            if (account == null) { return NotFound(); }
            var skills = account.FreelancerSkills
                .Select(p => new ResponseIdName {Id = p.Skill.Id, Name = p.Skill.Name}).ToList();
            return skills;
        }
        //get service
        [HttpGet("{id}/services")]
        public async Task<ActionResult<IEnumerable<ResponseIdName>>> GetFreelancerServices(int id)
        {
            Account account = await _context.Accounts
                .Include(p=>p.FreelancerServices).ThenInclude(p=>p.Service)
                .SingleOrDefaultAsync(p=>p.Id ==id);
            if (account == null) { return NotFound(); }
            var service = account.FreelancerServices
                .Select(p => new ResponseIdName {Id = p.Service.Id, Name = p.Service.Name}).ToList();
            return service;
        }
        //get capacity profiles
        [HttpGet("{id}/capacityprofiles")]
        public async Task<ActionResult<IEnumerable<CapacityProfileResponse>>> GetCPListByUserId(int id)
        {
            return await _context.CapacityProfiles
                        .Include(p => p.ProfileServices).ThenInclude(p => p.Service)
                        .Where(p => p.FreelancerId == id)
                        .Select(p => new CapacityProfileResponse(p))
                        .ToListAsync();
        }

        // get offerhistory
        [HttpGet("{id}/offerhistories")]
        public ActionResult<List<OfferHistoryResponse>> GetFreelancerOfferHistories(int id)
        {
            var account = _context.Accounts
                .Include(p => p.OfferHistories).ThenInclude(p => p.Job).ThenInclude(p => p.Renter)
                .Include(p => p.OfferHistories).ThenInclude(p => p.Job).ThenInclude(p => p.S.Specialty)
                .SingleOrDefault(p => p.Id == id);
            if (account == null)
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
            if (account.Email != email)
            {
                return BadRequest();
            }
            var list = account.OfferHistories
                .Select(p => new OfferHistoryResponse(p, 1)).ToList().Reverse<OfferHistoryResponse>().ToList();
            return list;
        }
        //get offerhistory/waiting 
        [HttpGet("{id}/offerhistories/waiting")]
        public ActionResult<List<OfferHistoryResponse>> GetFreelancerOfferHistoriesWaiting(int id)
        {
            var account = _context.Accounts
                .Include(p => p.OfferHistories).ThenInclude(p => p.Job).ThenInclude(p=>p.Renter)
                .Include(p => p.OfferHistories).ThenInclude(p => p.Job).ThenInclude(p=>p.S.Specialty)
                .SingleOrDefault(p => p.Id == id);
            if (account == null)
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
            if (account.Email != email)
            {
                return BadRequest();
            }
            var list = account.OfferHistories.Where(p => p.Job.Status == "Waiting" )
                .Select(p => new OfferHistoryResponse(p, 1)).ToList().Reverse<OfferHistoryResponse>().ToList();
            return list;
        }            

    
        //[HttpGet("pagination")]
                 //public async Task<ActionResult> GetPagination(int page, int count)
                 //{
                 //    var list = await _context.Accounts
                 //        .Include(p => p.JobFreelancers)
                 //        .Include(p => p.FreelancerSkills).ThenInclude(p => p.Skill)
                 //        .Include(p => p.FreelancerServices).ThenInclude(p => p.Service)
                 //        .Include(p => p.Specialty)
                 //        .Include(p => p.Role)
                 //        .Include(p => p.RatingFreelancers)
                 //        .Include(p => p.Level)
                 //        .ToListAsync();
                 //    try
                 //    {
                 //        return PaginationAccount(page, count, list);
                 //    }
                 //    catch (AppException ex)
                 //    {

        //        return BadRequest(new { message = ex.Message });
        //    }
        //}
        //private ActionResult PaginationAccount(int page, int count, List<Account> list)
        //{
        //    page -= 1;
        //    if (count * page > list.Count)
        //    {
        //        throw new AppException("Overpage");
        //    }
        //    List<Account> accounts = new List<Account>();
        //    for (int i = count * page; i < count * page + count; i++)
        //    {
        //        if (count * page + i > list.Count - 1)
        //        {
        //            break;
        //        }
        //        accounts.Add(list[i]);
        //    }
        //    return Ok(new
        //    {
        //        amount = list.Count(),
        //        page = Math.Ceiling(Decimal.Parse(list.Count.ToString()) /
        //            Decimal.Parse(count.ToString())) + 1,
        //        list = accounts.Select(p => new AccountResponseModel(p, true)).ToList()
        //    });
        //}

        // GET: api/Accounts/5

        [HttpGet("{id}")]
        public async Task<ActionResult<AccountResponseModel>> GetAccount(int id)
        {
            var account = await _context.Accounts
                .Include(p => p.FreelancerSkills).ThenInclude(p => p.Skill)
                .AsSplitQuery()
                .Include(p => p.FreelancerServices).ThenInclude(p => p.Service)
                .AsSplitQuery()
                .Include(p => p.Specialty)
                .AsSplitQuery()
                .Include(p => p.Role)
                .AsSplitQuery()
                .Include(p => p.RatingFreelancers)
                .AsSplitQuery()
                .Include(p => p.Level)
                .AsSplitQuery()
                .Include(p => p.Province)
                .AsSplitQuery()
                .Include(p => p.OfferHistories)
                .AsSplitQuery()
                .Include(p => p.CapacityProfiles)
                .ThenInclude(p => p.ProfileServices).ThenInclude(p => p.Service)
                .AsSplitQuery()
                .SingleOrDefaultAsync(p => p.Id == id);

            if (account == null)
            {
                return NotFound();
            }

            return new AccountResponseModel(account, true);
        }

        // PUT: api/Accounts/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccount(int id, AccountEditModel accountEditModel)
        {
            var account = _context.Accounts.Find(id);
            if (account == null)
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
            if (account.Email != email)
            {
                return BadRequest();
            }

            account.Name = accountEditModel.Name;
            account.RoleId = accountEditModel.RoleId;
            account.Phone = accountEditModel.Phone;
            account.Tile = accountEditModel.Tile;
            account.Description = accountEditModel.Description;
            account.Website = accountEditModel.Website;
            account.SpecialtyId = accountEditModel.SpecialtyId;
            account.LevelId = accountEditModel.LevelId;
            account.ProvinceId = accountEditModel.ProvinceID;
            var arrSkillsRemove = _context.FreelancerSkills.Where(p => p.FreelancerId == account.Id).ToArray();
            var arrServicesRemove = _context.FreelancerServices.Where(p => p.FreelancerId == account.Id).ToArray();

            _context.FreelancerServices.RemoveRange(arrServicesRemove);
            _context.FreelancerSkills.RemoveRange(arrSkillsRemove);
            await _context.SaveChangesAsync();

            foreach (var item in accountEditModel.Skills)
            {
                _context.FreelancerSkills.Add(new FreelancerSkill()
                {
                    FreelancerId = account.Id,
                    SkillId = item.Id
                });
            }

            foreach (var item in accountEditModel.Services)
            {
                _context.FreelancerServices.Add(new FreelancerService()
                {
                    FreelancerId = account.Id,
                    ServiceId = item.Id
                });
            }
            _context.Entry(account).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id))
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

        [HttpPut("deposit/{money}")]
        public async Task<ActionResult> TopUp(int money)
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
            if(account == null)
            {
                return BadRequest();
            }
            account.Balance += money;
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpPut("{id}/onready")]
        public async Task<IActionResult> OnReady(int id)
        {
            var account = _context.Accounts.Find(id);
            if (account == null)
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
            if (account == null || account.Email != email)
            {
                return BadRequest();
            }
            account.OnReady = !account.OnReady;
            _context.Entry(account).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id))
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

        // POST: api/Accounts
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Account>> PostAccount(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAccount", new { id = account.Id }, account);
        }

        // DELETE: api/Accounts/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Account>> DeleteAccount(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            account.BannedAtDate = TimeVN.Now();
            //_context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return account;
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }
        [HttpGet("fromtoken")]
        public ActionResult<AccountResponseModel> GetUserFromToken()
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
            var account = _context.Accounts
                .Include(p => p.FreelancerSkills).ThenInclude(p => p.Skill)
                .AsSplitQuery()
                .Include(p => p.FreelancerServices).ThenInclude(p => p.Service)
                .AsSplitQuery()
                .Include(p => p.Specialty)
                .AsSplitQuery()
                .Include(p => p.Role)
                .AsSplitQuery()
                .Include(p => p.RatingFreelancers)
                .AsSplitQuery()
                .Include(p => p.Level)
                .AsSplitQuery()
                .Include(p => p.OfferHistories)
                .AsSplitQuery()
                .Include(p => p.Province)
                .AsSplitQuery()
                .Include(p => p.CapacityProfiles)
                .ThenInclude(p => p.ProfileServices).ThenInclude(p => p.Service)
                .AsSplitQuery()
                .Include(p=>p.BankAccounts).ThenInclude(p=>p.Bank)
                .AsSplitQuery()
                .SingleOrDefault(p => p.Email == email);

            if (account == null)
            {
                return BadRequest();
            }
            return new AccountResponseModel(account, false);
        }
    }
}
