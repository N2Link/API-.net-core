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

namespace Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
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
                JobId = p.JobId,
                Level = p.Level,
                Price = p.Price,
                Profession = p.Profession,
                Quality = p.Quality,
                Time = p.Time
            }).ToListAsync();
        }  
        [HttpGet("listRatingFreelancer/{freelancerId}")]
        public async Task<ActionResult<IEnumerable<Rating>>> GetlistRatingFreelancer(int freelancerId)
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

            var freelancer = await _context.Accounts
                .SingleOrDefaultAsync(p => p.Email == email);
            if (freelancer.Id != freelancerId) { return BadRequest(); }
            return await _context.Ratings
                .Where(p => p.FreelancerId == freelancerId)
                .Select(p=>new Rating() 
                { 
                    Id = p.Id,
                    FreelancerId = p.FreelancerId,
                    JobId =p.JobId,
                    Level = p.Level,
                    Price = p.Price,
                    Profession = p.Profession,
                    Quality = p.Quality,
                    Time= p.Time
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
            Rating rating = new Rating()
            {
                Price = Int32.Parse(ratings.Select(p => p.Price).Average().ToString()),
                Level = Int32.Parse(ratings.Select(p => p.Level).Average().ToString()),
                Profession = Int32.Parse(ratings.Select(p => p.Profession).Average().ToString()),
                Quality = Int32.Parse(ratings.Select(p => p.Quality).Average().ToString()),
                Time = Int32.Parse(ratings.Select(p => p.Time).Average().ToString())
            };
            float a = (rating.Profession + rating.Price + rating.Level + rating.Quality + rating.Time) / 5;
            return Ok(new { 
                Avg = a,
                Count= ratings.Count(),
                Rating= rating,
            });
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

            var renter = await _context.Accounts
                .SingleOrDefaultAsync(p => p.Email == email);

            var check = renter.JobRenters.SingleOrDefault(p => p.Id == ratingPost.JobId);
            if (check == null)
            {
                return BadRequest();
            }
            if (check.FreelancerId == null || check.FreelancerId != ratingPost.FreelancerId)
            {
                return BadRequest();
            }
            rating.JobId = ratingPost.JobId;
            rating.FreelancerId = ratingPost.FreelancerId;
            rating.Level = ratingPost.Level;
            rating.Price = ratingPost.Price;
            rating.Profession = ratingPost.Profession;
            rating.Quality = ratingPost.Quality;
            rating.Time = ratingPost.Quality;
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

            var renter = await _context.Accounts
                .SingleOrDefaultAsync(p => p.Email == email);

            var check = renter.JobRenters.SingleOrDefault(p => p.Id == ratingPost.JobId);
            if (check == null)
            {
                return BadRequest();
            }
            if (check.FreelancerId == null || check.FreelancerId != ratingPost.FreelancerId)
            {
                return BadRequest();
            }
            Rating rating = new Rating()
            {
                JobId = ratingPost.JobId,
                FreelancerId = ratingPost.FreelancerId,
                Level = ratingPost.Level,
                Price = ratingPost.Price,
                Profession = ratingPost.Profession,
                Quality = ratingPost.Quality,
                Time = ratingPost.Quality,
                Comment = ratingPost.Comment,
            };
            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();
            return Ok(rating);
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
