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
using WebApi.Helpers;

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

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<JobResponseModel>>> GetListSerch
            (int page, int count,
            string search, int? specialtyId, string? serviceIds, string? skillIds,
            int? formId, int? payId, int? typeId,string? provinceId,
            long floorPrice =0, long cellingPrice= Int64.MaxValue)
        {
            List<string> partServiceId = new List<string>();
            if (serviceIds != null)
            {
                partServiceId = serviceIds.Split(" ").ToList();
            }
            List<string> partSkillId = new List<string>();
            if (serviceIds != null)
            {
                partSkillId = skillIds.Split(" ").ToList();
            }
            string[] partSearch = search.Split(" ");
            List<Job> list = new List<Job>();
            try
            {
                var listTemp = _context.Jobs
                .Where(p => p.Name.Contains(search) &&
                (specialtyId == null || p.SpecialtyId == specialtyId) &&
                (serviceIds == null || partServiceId.Contains(p.ServiceId.ToString())) &&
                (formId == null || p.FormId == formId) &&
                (payId == null || p.PayformId == payId) &&
                (typeId == null || p.TypeId == typeId) &&
                (provinceId == null || p.ProvinceId == provinceId) &&
                p.Floorprice >= floorPrice && p.Cellingprice <= cellingPrice
                ).ToList();
                foreach (var item in listTemp)
                {
                    foreach (var skillId in partSkillId)
                    {
                        if (item.JobSkills.Select(p => p.SkillId).ToList()
                            .Contains(Int32.Parse(skillId)))
                        {
                            list.Add(item);
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {

                return BadRequest(new {message="loi code linq"});
            }
            try
            {
                return PaginationJob(page, count, list);

            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("pagination")]
        public async Task<ActionResult<IEnumerable<JobResponseModel>>> GetPagination(int page, int count)
        {
            var list = await _context.Jobs
                .Include(p => p.Renter)
                .Include(p => p.Freelancer)
                .Include(p => p.Form)
                .Include(p => p.Type)
                .Include(p => p.JobSkills).ThenInclude(p => p.Skill)
                .Include(p => p.S).ThenInclude(p => p.Service)
                .Include(p => p.S).ThenInclude(p => p.Specialty)
                .Include(p => p.Payform)
                .Include(p => p.JobSkills).ThenInclude(p => p.Skill)
                .Include(p => p.Province)
                .Where(p=>p.Deadline>DateTime.Now)
                .ToListAsync();
            try
            {
                return Ok(PaginationJob(page, count, list));
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("availlable")]
        public ActionResult GetAmount()
        {
            return Ok(new { amount = _context.Jobs.Count() });
        }

        private ActionResult PaginationJob(int page, int count, List<Job> list)
        {
            page -= 1;
            if (count * page > list.Count)
            {
                throw new AppException("Overpage");
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
            var listTemp = jobs.Select(p => new JobResponseModel(p)).ToList();
            return Ok(
                new
                {
                    amount = list.Count(),
                    page = list.Count>0? Math.Ceiling(Decimal.Parse(list.Count.ToString()) / Decimal.Parse(count.ToString())) + 1:0,
                    Jobs = listTemp
                }); ;
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
            var job = await _context.Jobs
                .Include(p => p.Renter)
                .Include(p => p.Freelancer)
                .Include(p=>p.Form)
                .Include(p=>p.Type)
                .Include(p=>p.JobSkills).ThenInclude(p=>p.Skill)
                .Include(p => p.S).ThenInclude(p => p.Service)
                .Include(p => p.S).ThenInclude(p => p.Specialty)
                .Include(p => p.Payform)
                .Include(p => p.JobSkills).ThenInclude(p => p.Skill)
                .Include(p => p.Province)
                .SingleOrDefaultAsync(p=>p.Id == id);

            if (job == null)
            {
                return NotFound();
            }
            var jobresponse = new JobResponseModel(job);            
            return jobresponse;
        }

        // PUT: api/Jobs/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutJob(int id, JobPostModel jobEditModel)
        {
            Job job = _context.Jobs.Find(id);
            if(job == null)
            {
                return BadRequest();
            }
            if (jobEditModel.Deadline <= DateTime.Now)
            {
                return BadRequest(new { message = "DateTime Invalid" });
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
            if (job.RenterId != renter.Id)
            {
                return BadRequest();
            }

            job.Name = jobEditModel.Name;
            job.Details = jobEditModel.Details;
            job.TypeId = jobEditModel.TypeId;
            job.FormId = jobEditModel.FormId;
            job.WorkatId = jobEditModel.WorkatId;
            job.PayformId = jobEditModel.PayformId;
            job.Deadline = jobEditModel.Deadline;
            job.Floorprice = jobEditModel.Floorprice;
            job.Cellingprice = jobEditModel.Cellingprice;
            job.IsPrivate = jobEditModel.IsPrivate;
            job.SpecialtyId = jobEditModel.SpecialtyId;
            job.ServiceId = jobEditModel.ServiceId;
            job.ProvinceId = jobEditModel.ProvinceId;
            job.Status = "Waiting";

            var arrayRemove = _context.JobSkills.Where(p => p.JobId == id).ToArray();
            _context.JobSkills.RemoveRange(arrayRemove);
            await _context.SaveChangesAsync();
            foreach (var item in jobEditModel.Skills)
            {
               await _context.JobSkills.AddAsync(new JobSkill() 
               { 
                   JobId = id,
                   SkillId = item.Id 
               });
            }

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

            return Ok();
        }
        [HttpPut("done/{id}")]
        public async Task<ActionResult> DoneJob(int id)
        {
            var job = _context.Jobs.Find(id);
            if(job == null)
            {
                return BadRequest(new { message = "Job dont exits" });
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
            if(job.RenterId!= renter.Id)
            {
                return BadRequest(new { message = "You dont have this permission" });
            }
            job.Status = "Done";
            _context.Entry(job).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok();
        }      

        [HttpPut("cancel/{id}")]
        public async Task<ActionResult> CancelJob(int id)
        {
            var job = _context.Jobs.Find(id);
            if(job == null)
            {
                return BadRequest(new { message = "Job dont exits" });
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
            if(job.RenterId!= renter.Id)
            {
                return BadRequest(new { message = "You dont have this permission" });
            }
            job.Status = "Cancelled";
            _context.Entry(job).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok();
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
                .SingleOrDefaultAsync(p=>p.Email == email);
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
                ProvinceId = jobPostModel.ProvinceId,
                ServiceId = jobPostModel.ServiceId,
                Status = "Waiting",
            };
            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();
            foreach (var skill in jobPostModel.Skills)
            {
                _context.JobSkills.Add(new JobSkill()
                {
                    JobId = job.Id,
                    SkillId = skill.Id,
                });
            }
            await _context.SaveChangesAsync();
            job = await _context.Jobs
                .Include(p => p.Renter)
                .Include(p => p.Freelancer)
                .Include(p => p.Form)
                .Include(p => p.Type)
                .Include(p => p.JobSkills).ThenInclude(p => p.Skill)
                .Include(p => p.S).ThenInclude(p => p.Service)
                .Include(p => p.S).ThenInclude(p => p.Specialty)
                .Include(p => p.Payform)
                .Include(p => p.JobSkills).ThenInclude(p => p.Skill)
                .Include(p=>p.Province)
                .SingleOrDefaultAsync(p => p.Id == job.Id);
            return Ok(job);
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
