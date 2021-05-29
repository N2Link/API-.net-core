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

namespace Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OfferHistoriesController : ControllerBase
    {
        private readonly FreeLancerVNContext _context;

        public OfferHistoriesController(FreeLancerVNContext context)
        {
            _context = context;
        }

        // GET: api/OfferHistories
        [HttpGet("freelancer/{freelancerid}")]
        public ActionResult<List<OfferHistoryResponse>> GetFreelancerOfferHistories(int freelancerid)
        {
            var account = _context.Accounts.Include(p=>p.OfferHistories).ThenInclude(p=>p.Job)
                .SingleOrDefault(p=>p.Id ==freelancerid);
            if (account == null)
            {
                return BadRequest();
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
            return account.OfferHistories.Select(p=>new OfferHistoryResponse(p, 1)).ToList();
        }

        [HttpGet("job/{jobid}")]
        public ActionResult<List<OfferHistoryResponse>> GetJobOfferHistories(int jobid)
        {
            var job = _context.Jobs
                .Include(p=>p.Freelancer).ThenInclude(p=>p.RatingFreelancers).AsSplitQuery()
                .Include(p=>p.OfferHistories)
                .SingleOrDefault(p=>p.Id == jobid);
            if (job == null)
            {
                return BadRequest();
            }
            var account = job.Renter;

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
            return job.OfferHistories.Select(p=>new OfferHistoryResponse(p,2)).ToList();
        }

        // GET: api/OfferHistories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OfferHistory>> GetOfferHistory(int id)
        {
            var offerHistory = await _context.OfferHistories.FindAsync(id);

            if (offerHistory == null)
            {
                return NotFound();
            }

            return offerHistory;
        }

        // PUT: api/OfferHistories/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("choose/{id}")]
        public async Task<IActionResult> PutOfferHistory(int id, OfferHistory offerHistory)
        {
            if (id != offerHistory.JobId)
            {
                return BadRequest();
            }
            var job = _context.Jobs.Find(offerHistory.JobId);
            if (job == null)
            {
                return BadRequest();
            }
            var account = job.Renter;

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
            job.Status = "In progress";
            _context.Entry(offerHistory).State = EntityState.Modified;
            _context.Entry(job).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OfferHistoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }//add todo list to database
/*            if (offerHistory.Status == "Done")
            {
                var listOffersRejected = _context.OfferHistories
                    .Where(p => p.JobId == id && p.FreelancerId != offerHistory.FreelancerId).ToList();
                foreach (var item in listOffersRejected)
                {
                    item.Status = "Rejected";
                }
                String[] part = offerHistory.TodoList.Split("%split%");
                foreach (var item in part)
                {
                    _context.Todolists.Add(new Todolist()
                    {
                        JobId = offerHistory.JobId,
                        Todo = item,
                        Status = "Unfinished"
                    });
                }
                await _context.SaveChangesAsync();
            }*/
            return Ok();
        }

        // POST: api/OfferHistories
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<OfferHistory>> PostOfferHistory([FromBody] OfferHistoryPost offerHistoryPost)
        {
            if (_context.Jobs.Find(offerHistoryPost.JobId) == null)
            {
                return BadRequest();
            }
            var freelancer = _context.Accounts.Find(offerHistoryPost.FreelancerId);
            if (freelancer == null)
            {
                return BadRequest();
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
            if (email != freelancer.Email || freelancer.IsAccuracy == false)
            {
                return BadRequest();
            }
            if (_context.OfferHistories
                .Where(p => p.FreelancerId == freelancer.Id && offerHistoryPost.JobId == p.JobId).Count() >= 3)
            {
                return Ok("Oops... You have only three times to offer this job.");
            }
            var offerHistory = new OfferHistory()
            {
                JobId = offerHistoryPost.JobId,
                FreelancerId = offerHistoryPost.FreelancerId,
                Description = offerHistoryPost.Description,
                ExpectedDay = offerHistoryPost.ExpectedDay,
                OfferPrice = offerHistoryPost.OfferPrice,
                Status = "Waiting"
            };
            _context.OfferHistories.Add(offerHistory);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (OfferHistoryExists(offerHistory.JobId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return Ok(offerHistory);
        }

        // DELETE: api/OfferHistories/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<OfferHistory>> DeleteOfferHistory(int id)
        {
            var offerHistory = await _context.OfferHistories.FindAsync(id);
            if (offerHistory == null)
            {
                return NotFound();
            }

            _context.OfferHistories.Remove(offerHistory);
            await _context.SaveChangesAsync();

            return offerHistory;
        }

        private bool OfferHistoryExists(int id)
        {
            return _context.OfferHistories.Any(e => e.JobId == id);
        }
    }
}
