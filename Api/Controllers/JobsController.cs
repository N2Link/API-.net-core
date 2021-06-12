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
        [HttpGet]
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
            if (account == null) { return NotFound(); }

            return await _context.Jobs.Include(p => p.Renter).OrderByDescending(p => p.CreateAt)
                .Where(p=>p.RenterId!= account.Id)
                .Select(p => new JobForListResponse(p)).ToListAsync();
        }

        [HttpGet("search")]
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

            var list = await _context.Jobs                
                .Where(p=>p.Name.Contains(search)
                &&p.Floorprice>=floorPrice
                &&p.Cellingprice<=cellingPrice
                &&(specialtyId == 0|| p.SpecialtyId==specialtyId)
                &&(serviceId ==0 || p.ServiceId==serviceId)
                &&(payFormId==0||p.PayformId == payFormId)
                &&(provinceId == "00" || p.ProvinceId == provinceId)
                && (formOfWorkId == 0|| p.FormId == formOfWorkId)
                &&(typeOfWorkId== 0 || p.TypeId == typeOfWorkId))
                .Select(p=> new JobForListResponse(p))
                .ToListAsync();
            return list;

            //List<string> partServiceId = new List<string>();
            //if (serviceIds != null)
            //{
            //    partServiceId = serviceIds.Split(" ").ToList();
            //}
            //List<string> partSkillId = new List<string>();
            //if (serviceIds != null)
            //{
            //    partSkillId = skillIds.Split(" ").ToList();
            //}
            //string[] partSearch = search.Split(" ");
            //List<Job> list = new List<Job>();
            //try
            //{
            //    var listTemp = _context.Jobs
            //    .Where(p => p.Name.Contains(search) &&
            //    (specialtyId == null || p.SpecialtyId == specialtyId) &&
            //    (serviceIds == null || partServiceId.Contains(p.ServiceId.ToString())) &&
            //    (formId == null || p.FormId == formId) &&
            //    (payId == null || p.PayformId == payId) &&
            //    (typeId == null || p.TypeId == typeId) &&
            //    (provinceId == null || p.ProvinceId == provinceId) &&
            //    p.Floorprice >= floorPrice && p.Cellingprice <= cellingPrice
            //    ).ToList();
            //    foreach (var item in listTemp)
            //    {
            //        foreach (var skillId in partSkillId)
            //        {
            //            if (item.JobSkills.Select(p => p.SkillId).ToList()
            //                .Contains(Int32.Parse(skillId)))
            //            {
            //                list.Add(item);
            //                break;
            //            }
            //        }
            //    }
            //}
            //catch (Exception)
            //{

            //    return BadRequest(new {message="loi code linq"});
            //}
            //try
            //{
            //    return PaginationJob(page, count, list);

            //}
            //catch (AppException ex)
            //{
            //    return BadRequest(new { message = ex.Message });
            //}
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
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutJob(int id, JobPostModel jobEditModel)
        //{
        //    Job job = _context.Jobs.Find(id);
        //    if(job == null)
        //    {
        //        return BadRequest();
        //    }
        //    if (jobEditModel.Deadline <= DateTime.Now)
        //    {
        //        return BadRequest(new { message = "DateTime Invalid" });
        //    }
        //    String jwt = Request.Headers["Authorization"];
        //    jwt = jwt.Substring(7);
        //    //Decode jwt and get payload
        //    var stream = jwt;
        //    var handler = new JwtSecurityTokenHandler();
        //    var jsonToken = handler.ReadToken(stream);
        //    var tokenS = jsonToken as JwtSecurityToken;
        //    //I can get Claims using:
        //    var email = tokenS.Claims.First(claim => claim.Type == "email").Value;

        //    var renter = await _context.Accounts
        //        .SingleOrDefaultAsync(p => p.Email == email);
        //    if (renter == null)
        //    {
        //        return BadRequest();
        //    }
        //    if (job.RenterId != renter.Id)
        //    {
        //        return BadRequest();
        //    }

        //    job.Name = jobEditModel.Name;
        //    job.Details = jobEditModel.Details;
        //    job.TypeId = jobEditModel.TypeId;
        //    job.FormId = jobEditModel.FormId;
        //    job.PayformId = jobEditModel.PayformId;
        //    job.Deadline = jobEditModel.Deadline;
        //    job.Floorprice = jobEditModel.Floorprice;
        //    job.Cellingprice = jobEditModel.Cellingprice;
        //    job.IsPrivate = jobEditModel.IsPrivate;
        //    job.SpecialtyId = jobEditModel.SpecialtyId;
        //    job.ServiceId = jobEditModel.ServiceId;
        //    job.ProvinceId = jobEditModel.ProvinceId;
        //    job.Status = "Waiting";

        //    var arrayRemove = _context.JobSkills.Where(p => p.JobId == id).ToArray();
        //    _context.JobSkills.RemoveRange(arrayRemove);
        //    await _context.SaveChangesAsync();
        //    foreach (var item in jobEditModel.Skills)
        //    {
        //       await _context.JobSkills.AddAsync(new JobSkill() 
        //       { 
        //           JobId = id,
        //           SkillId = item.Id 
        //       });
        //    }

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!JobExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Ok();
        //}
        
        //get offerhistory
        [HttpGet("{id}/offerhistorys")]
        public ActionResult<List<OfferHistoryResponse>> GetJobOfferHistories(int id)
        {
            var job = _context.Jobs
                .Include(p => p.Freelancer).ThenInclude(p => p.RatingFreelancers).AsSplitQuery()
                .Include(p => p.OfferHistories)
                .SingleOrDefault(p => p.Id == id);
            if (job == null)
            {
                return NotFound();
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
            return job.OfferHistories.Select(p => new OfferHistoryResponse(p, 2)).ToList();
        }
        //PUT STATUS
        //put inprogress
        [HttpPut("{id}/offerhistory/{freelancerid}/choose")]
        public async Task<IActionResult> PutOfferHistory(int id, int freelancerid)
        {
            Job job = await _context.Jobs
                .Include(p => p.OfferHistories).ThenInclude(p => p.Freelancer)
                .Include(p => p.Renter)
                .SingleOrDefaultAsync(p => p.Id == id);
            if (job == null) { NotFound(); }

            String jwt = Request.Headers["Authorization"];
            jwt = jwt.Substring(7);
            //Decode jwt and get payload
            var stream = jwt;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var tokenS = jsonToken as JwtSecurityToken;
            //I can get Claims using:
            var email = tokenS.Claims.First(claim => claim.Type == "email").Value;
            var account = job.Renter;
            if (account.Email != email)
            {
                return BadRequest();
            }
            job.Status = "In progress";
            job.FreelancerId = freelancerid;
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
            return Ok();
        }   
        //Get messages
        [HttpGet("{id}/messages")]
        public ActionResult<List<MessageResponse>> Getmessages(int id)
        {
            var job = _context.Jobs
                .Include(p=>p.Messages).ThenInclude(p=>p.Sender)
                .Include(p=>p.Messages).ThenInclude(p=>p.Receive)
                .SingleOrDefault(p => p.Id == id);
            if (job == null)
            {
                return NotFound();
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
            return job.Messages.Select(p=>new MessageResponse(p)).ToList();
        }
        //put close .. renter
        [HttpPut("{id}/close")]
        public async Task<ActionResult> CloseJob(int id)
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
            if (job.Status != "Waiting")
            {
                return BadRequest(new { message = "Can't close this job." });
            }
            job.Status = "Closed";
            _context.Entry(job).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok();
        }
        //put request finish .. freelancer
        [HttpPut("{id}/requestfinish")]
        public async Task<ActionResult> Requestfinish(int id)
        {
            var job = _context.Jobs.Find(id);
            if(job == null)
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

            var freelancer = await _context.Accounts
                .SingleOrDefaultAsync(p => p.Email == email);
            if(job.FreelancerId!= freelancer.Id)
            {
                return BadRequest(new { message = "You dont have this permission" });
            }
            if(job.Status !="In progress")
            {
                return BadRequest();
            }
            job.Status = "Request finish";
            _context.Entry(job).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok();
        }    
        //put done
        [HttpPut("{id}/done")]
        public async Task<ActionResult> DoneJob(int id)
        {
            var job = _context.Jobs.Find(id);
            if(job == null)
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
            if(job.RenterId!= renter.Id)
            {
                return BadRequest(new { message = "You dont have this permission" });
            }
            if (job.Status != "Request finish") 
            {
                return BadRequest();
            }
            job.Status = "Done";
            _context.Entry(job).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok();
        }    
        //put Rework request
        [HttpPut("{id}/requestrework")]
        public async Task<IActionResult> RequestRework(int id)
        {
            Job job = _context.Jobs.Find(id);
            if (job == null) { NotFound(); }
            String jwt = Request.Headers["Authorization"];
            jwt = jwt.Substring(7);
            //Decode jwt and get payload
            var stream = jwt;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var tokenS = jsonToken as JwtSecurityToken;
            //I can get Claims using:
            var email = tokenS.Claims.First(claim => claim.Type == "email").Value;
            var account = job.Renter;
            if (account.Email != email)
            {
                return BadRequest();
            }
            if (job.Status != "Request finish")
            {
                return BadRequest();
            }
            job.Status = "Request rework";

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
            return Ok();
        }        
        //put  request cancellation
        [HttpPut("{id}/requestcancellation")]
        public async Task<IActionResult> CancelRequest(int id)
        {
            Job job = _context.Jobs.Find(id);
            if (job == null) { NotFound(); }
            String jwt = Request.Headers["Authorization"];
            jwt = jwt.Substring(7);
            //Decode jwt and get payload
            var stream = jwt;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var tokenS = jsonToken as JwtSecurityToken;
            //I can get Claims using:
            var email = tokenS.Claims.First(claim => claim.Type == "email").Value;
            var account = job.Renter;
            if (account.Email != email)
            {
                return BadRequest();
            }
            if (job.Status != "Request finish")
            {
                return BadRequest();
            }
            job.Status = "Request cancellation";

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
            return Ok();
        }
        //put rework .. admin
        [HttpPut("adminmode/{id}/rework")]
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
        [HttpPut("adminmode/{id}/cancel")]
        public async Task<ActionResult> CancelJob(int id)
        {
            var job = _context.Jobs.Find(id);
            if(job == null)
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
            if(job.RenterId!= renter.Id)
            {
                return BadRequest(new { message = "You dont have this permission" });
            }
            if(job.Status != "Request cancellation")
            {
                return BadRequest();
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
                PayformId = jobPostModel.PayformId,
                Deadline = jobPostModel.Deadline,
                Floorprice = jobPostModel.Floorprice,
                Cellingprice = jobPostModel.Cellingprice,
                IsPrivate = jobPostModel.IsPrivate,
                SpecialtyId = jobPostModel.SpecialtyId,
                ProvinceId = jobPostModel.ProvinceId,
                ServiceId = jobPostModel.ServiceId,
                CreateAt = DateTime.Now,
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
            return Ok(new JobResponseModel(job));
        }
        private bool JobExists(int id)
        {
            return _context.Jobs.Any(e => e.Id == id);
        }
    }
}
