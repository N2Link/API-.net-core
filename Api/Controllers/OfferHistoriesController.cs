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
using Microsoft.AspNetCore.Cors;

namespace Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]

    public class OfferHistoriesController : ControllerBase
    {
        private readonly FreeLancerVNContext _context;

        public OfferHistoriesController(FreeLancerVNContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<OfferHistory>> GetOfferHistory(int freelancerId , int jobId)
        {
            var offerHistory = await _context.OfferHistories.
                SingleOrDefaultAsync(p=>p.FreelancerId == freelancerId && p.JobId == jobId);

            if (offerHistory == null)
            {
                return NotFound();
            }

            return offerHistory;
        }
        [HttpPost]
        public async Task<ActionResult<OfferHistory>> PostOfferHistory([FromBody] OfferHistoryPost offerHistoryPost)
        {
            if (_context.Jobs.Find(offerHistoryPost.JobId) == null)
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
            var freelancer = await _context.Accounts.SingleOrDefaultAsync(p => p.Email == email);
            if(freelancer == null)
            {
                return BadRequest();
            }

            if (_context.OfferHistories
                .SingleOrDefault(p => p.FreelancerId == freelancer.Id && offerHistoryPost.JobId == p.JobId)!=null)
            {
                return BadRequest(new {message = "Oops... you have offered this job" });
            }

            var offerHistory = new OfferHistory()
            {
                JobId = offerHistoryPost.JobId,
                FreelancerId = freelancer.Id,
                Description = offerHistoryPost.Description,
                ExpectedDay = offerHistoryPost.ExpectedDay,
                OfferPrice = offerHistoryPost.OfferPrice,
                TodoList = offerHistoryPost.TodoList,
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
            return Ok();
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
