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

namespace Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
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
            return await _context.Accounts
                .Include(p => p.Specialty)
                .Include(p => p.RatingFreelancers)
                .Include(p => p.Level)
                .Select(p => new AccountForListResponse(p)).ToListAsync();
        }
/*        [HttpGet("search")]
        public async Task<ActionResult> GetLisSearch(int page, int count, string search)
        {
            var list = _context.Accounts.Where(p => p.Name.Contains(search));
        }*/

        [HttpGet("pagination")]
        public async Task<ActionResult> GetPagination(int page, int count)
        {
            var list = await _context.Accounts
                .Include(p => p.JobFreelancers)
                .Include(p => p.FreelancerSkills).ThenInclude(p => p.Skill)
                .Include(p => p.FreelancerServices).ThenInclude(p => p.Service)
                .Include(p => p.Specialty)
                .Include(p => p.Role)
                .Include(p => p.RatingFreelancers)
                .Include(p => p.Level)
                .ToListAsync();
            try
            {
                return PaginationAccount(page, count, list);
            }
            catch (AppException ex)
            {

                return BadRequest(new { message = ex.Message });
            }
        }
        private ActionResult PaginationAccount(int page, int count, List<Account> list)
        {
            page -= 1;
            if (count * page > list.Count)
            {
                throw new AppException("Overpage");
            }
            List<Account> accounts = new List<Account>();
            for (int i = count * page; i < count * page + count; i++)
            {
                if (count * page + i > list.Count - 1)
                {
                    break;
                }
                accounts.Add(list[i]);
            }
            return Ok(new
            {
                amount = list.Count(),
                page = Math.Ceiling(Decimal.Parse(list.Count.ToString()) /
                    Decimal.Parse(count.ToString())) + 1,
                list = accounts.Select(p => new AccountResponseModel(p, true)).ToList()
            });
        }

        // GET: api/Accounts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AccountResponseModel>> GetAccount(int id)
        {
            var account = await _context.Accounts
                .Include(p => p.JobRenters)
                .ThenInclude(p => p.S).ThenInclude(p => p.Service)
                .AsSplitQuery()
                .Include(p => p.JobRenters)
                .ThenInclude(p => p.S).ThenInclude(p => p.Specialty)
                .AsSplitQuery()
                .Include(p => p.JobRenters)
                .ThenInclude(p => p.Form)
                .AsSplitQuery()
                .Include(p => p.JobRenters)
                .ThenInclude(p => p.Type)
                .AsSplitQuery()
                .Include(p => p.JobRenters)
                .ThenInclude(p => p.Payform)
                .AsSplitQuery()
                .Include(p => p.JobRenters)
                .ThenInclude(p => p.JobSkills).ThenInclude(p => p.Skill)
                .AsSplitQuery()


                .Include(p => p.JobFreelancers)
                .ThenInclude(p => p.S).ThenInclude(p => p.Service)
                .AsSplitQuery()
                .Include(p => p.JobFreelancers)
                .ThenInclude(p => p.S).ThenInclude(p => p.Specialty)
                .AsSplitQuery()
                .Include(p => p.JobFreelancers)
                .ThenInclude(p => p.Form)
                .AsSplitQuery()
                .Include(p => p.JobFreelancers)
                .ThenInclude(p => p.Type)
                .AsSplitQuery()
                .Include(p => p.JobFreelancers)
                .ThenInclude(p => p.Payform)
                .AsSplitQuery()
                .Include(p => p.JobFreelancers)
                .ThenInclude(p => p.JobSkills).ThenInclude(p => p.Skill)
                .AsSplitQuery()


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

            String jwt = Request.Headers["Authorization"];
            jwt = jwt.Substring(7);
            //Decode jwt and get payload
            var stream = jwt;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var tokenS = jsonToken as JwtSecurityToken;
            //I can get Claims using:
            var email = tokenS.Claims.First(claim => claim.Type == "email").Value;
            var account = _context.Accounts.Find(id);
            if (account == null || account.Email != email)
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
            account.OnReady = accountEditModel.OnReady;

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

            _context.Accounts.Remove(account);
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
                .Include(p => p.JobRenters)
                .ThenInclude(p => p.S).ThenInclude(p => p.Service)
                .AsSplitQuery()
                .Include(p => p.JobRenters)
                .ThenInclude(p => p.S).ThenInclude(p => p.Specialty)
                .AsSplitQuery()
                .Include(p => p.JobRenters)
                .ThenInclude(p => p.Form)
                .AsSplitQuery()
                .Include(p => p.JobRenters)
                .ThenInclude(p => p.Type)
                .AsSplitQuery()
                .Include(p => p.JobRenters)
                .ThenInclude(p => p.Payform)
                .AsSplitQuery()
                .Include(p => p.JobRenters)
                .ThenInclude(p => p.JobSkills).ThenInclude(p => p.Skill)
                .AsSplitQuery()


                .Include(p => p.JobFreelancers)
                .ThenInclude(p => p.S).ThenInclude(p => p.Service)
                .AsSplitQuery()
                .Include(p => p.JobFreelancers)
                .ThenInclude(p => p.S).ThenInclude(p => p.Specialty)
                .AsSplitQuery()
                .Include(p => p.JobFreelancers)
                .ThenInclude(p => p.Form)
                .AsSplitQuery()
                .Include(p => p.JobFreelancers)
                .ThenInclude(p => p.Type)
                .AsSplitQuery()
                .Include(p => p.JobFreelancers)
                .ThenInclude(p => p.Payform)
                .AsSplitQuery()
                .Include(p => p.JobFreelancers)
                .ThenInclude(p => p.JobSkills).ThenInclude(p => p.Skill)
                .AsSplitQuery()
                .AsSplitQuery()


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
                .Include(p => p.CapacityProfiles)
                .ThenInclude(p => p.ProfileServices).ThenInclude(p => p.Service)
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
