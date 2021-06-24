using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Api.Enities;
using Microsoft.AspNetCore.Cors;

namespace Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]

    public class RatingsController : ControllerBase
    {
        private readonly FreeLancerVNContext _context;

        public RatingsController(FreeLancerVNContext context)
        {
            _context = context;
        }

        // GET: api/Ratings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rating>>> GetRatings()
        {
            return await _context.Ratings.Select(p => new Rating()
            {
                Id = p.Id,
                FreelancerId = p.FreelancerId,
                RenterId = p.RenterId,
                Star = p.Star,
                Comment = p.Comment
            }).ToListAsync();
        }  

        // GET: api/Ratings/5
        [HttpGet("{freelancerId}")]
        public async Task<ActionResult> GetRating(int freelancerId)
        {
            var ratings = await _context.Ratings.Where(p=>p.FreelancerId == freelancerId).ToListAsync();

            if (ratings == null)
            {
                return NotFound();
            }
            if (ratings.Count == 0)
            {
                return Ok(new { message = "There is no current evaluation" });
            }

            return Ok(new TotalRatingModel(ratings));
        }

        // PUT: api/Ratings/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRating(int id, RatingPost ratingPost)
        {
            Rating rating = _context.Ratings.Find(id);
            if (rating == null)
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
            if (renter == null)
            {
                return BadRequest();
            }
            var job = renter.JobRenters.SingleOrDefault(p => p.Id == ratingPost.JobID);

            if (job == null)
            {
                return BadRequest();
            }

            rating.RenterId =renter.Id;
            rating.FreelancerId = ratingPost.FreelancerId;
            rating.Star = rating.Star;
            rating.Comment = ratingPost.Comment;
            _context.Entry(rating).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok();
        }

        // POST: api/Ratings
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<RatingPost>> PostRating(RatingPost ratingPost)
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

            var renter = await _context.Accounts.Include(p=>p.JobRenters)
                .SingleOrDefaultAsync(p => p.Email == email);
            if (renter == null)
            {
                return BadRequest();
            }

            var job = renter.JobRenters.SingleOrDefault(p => p.Id == ratingPost.JobID);

            if(job == null || !(job.Status == "Finished" || job.Status == "Cancellation"))
            {
                return BadRequest();
            }
            Rating rating = new Rating()
            {
                RenterId = renter.Id,
                FreelancerId = ratingPost.FreelancerId,
                Star = ratingPost.Star,
                Comment = ratingPost.Comment,
            };

            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();
            job.RatingId = rating.Id;
            await _context.SaveChangesAsync();
            return Ok();
        }

        // DELETE: api/Ratings/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Rating>> DeleteRating(int id)
        {
            var rating = await _context.Ratings.FindAsync(id);
            if (rating == null)
            {
                return NotFound();
            }

            _context.Ratings.Remove(rating);
            await _context.SaveChangesAsync();

            return rating;
        }

        private bool RatingExists(int id)
        {
            return _context.Ratings.Any(e => e.Id == id);
        }
    }
}
