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
using Api.Service;

namespace Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly FreeLancerVNContext _context;

        public JobsController(FreeLancerVNContext context)
        {
            _context = context;
        }

        [HttpGet("pagination")]
        public async Task<ActionResult<IEnumerable<JobResponseModel>>> GetPagination(int page, int count)
        {
            var list = await _context.Jobs.Include(p=>p.Renter)
                .Include(p=>p.S).ThenInclude(p=>p.Service)
                .Include(p=>p.S).ThenInclude(p=>p.Specialty)
                .Include(p=>p.Payform).Include(p=>p.JobSkills)
                .ToListAsync();

            if (count * page > list.Count)
            {
                return Ok("Overpage");
            }
            List<Job> jobs = new List<Job>();
            for (int i = count * page; i < count * page + count; i++)
            {
                if (count * page + i > list.Count - 1)
                {
                    break;
                }
                jobs.Add(list[i]);
            }
            return Ok(jobs.Select(p => new JobResponseModel()
            {
                Id = p.Id,
                Name = p.Name,
                Renter = new IUserService.UserEntitis(p.Renter),
                Freelancer = p.Freelancer != null ? new IUserService.UserEntitis(p.Freelancer):null,
                Deadline = p.Deadline,
                SS = new SS(p.S),
                Cellingprice = p.Cellingprice,
                Details = p.Details,
                Floorprice = p.Floorprice,
                Payform = p.Payform,
                JobSkills = p.JobSkills,
            }).ToList());
        }

        // GET: api/Jobs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Job>>> GetJobs()
        {
            return await _context.Jobs.ToListAsync();
        }

        // GET: api/Jobs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<JobResponseModel>> GetJob(int id)
        {
            var job = await _context.Jobs.FindAsync(id);

            if (job == null)
            {
                return NotFound();
            }
            var jobresponse = new JobResponseModel()
            {
                Id = job.Id,
                Name = job.Name,
                Renter = new IUserService.UserEntitis(job.Renter),
                Freelancer = job.Freelancer != null ? new IUserService.UserEntitis(job.Freelancer) : null,
                Deadline = job.Deadline,
                SS = new SS(job.S),
                Cellingprice = job.Cellingprice,
                Details = job.Details,
                Floorprice = job.Floorprice,
                Payform = job.Payform,
                JobSkills = job.JobSkills,
            };
            return jobresponse;
        }

        // PUT: api/Jobs/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutJob(int id, Job job)
        {
            if (id != job.Id)
            {
                return BadRequest();
            }

            var renter = _context.Accounts.Find(job.RenterId);
            if (renter == null)
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
            if (email != renter.Email)
            {
                return BadRequest();
            }

            _context.Entry(job).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Jobs
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<JobPostModel>> PostJob(JobPostModel jobPostModel)
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
                .SingleOrDefaultAsync(p=>p.Email == email && p.RoleId==3);
            if(renter == null)
            {
                return BadRequest();
            }
            var job = new Job()
            {
                Name = jobPostModel.Name,
                RenterId = renter.Id,
                Details = jobPostModel.Details,
                TypeId = jobPostModel.TypeId,
                FormId = jobPostModel.FormId,
                WorkatId = jobPostModel.WorkatId,
                PayformId = jobPostModel.PayformId,
                Deadline = jobPostModel.Deadline,
                Floorprice = jobPostModel.Floorprice,
                Cellingprice = jobPostModel.Cellingprice,
                IsPrivate = jobPostModel.IsPrivate,
                SpecialtyId = jobPostModel.SpecialtyId,
                ServiceId = jobPostModel.ServiceId,
            };

        _context.Jobs.Add(job);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetJob", new { id = job.Id }, job);
        }

        // DELETE: api/Jobs/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Job>> DeleteJob(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();

            return job;
        }

        private bool JobExists(int id)
        {
            return _context.Jobs.Any(e => e.Id == id);
        }
    }
}
