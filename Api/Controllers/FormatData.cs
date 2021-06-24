using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Models;
using Api.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormatData : ControllerBase
    {
        private readonly FreeLancerVNContext _context;
        public FormatData(FreeLancerVNContext context)
        {
            _context = context;
        }
        [HttpGet("getclaim")]
        public ActionResult getclaim()
        {
            ClaimsPrincipal principal = HttpContext.User;
            return Ok(principal.Claims);
            //return Ok();
        }
        [HttpGet("name")]
        public ActionResult Suanam()
        {

            foreach (var item in _context.OfferHistories.ToList())
            {
                item.Description = "Chào bạn mình có thể liên lạc trực tiếp với bạn được chứ. MÌnh có 3 năm kinh nghiệm phát triển app Mollie và 2 năm làm app về flutter, Mình sẽ tư vấn thêm cho bạn nhiều khía cạnh khác thi phát triển app của mình.";
                item.TodoList = "Cần login, register, Các trang tìm kiếm, đăng việc, quản lý công việc, và trang admin cần việc viết backend phải tốt thì hệ thống mới chạy tốt được. Còn Build phần UI mình có nhiều kinh nghiệm chắc sẽ cũng nhanh";
            }
            _context.SaveChanges();
            return Ok();
        }
        [HttpGet("randomdate")]
        public ActionResult RandomDate()
        {
            var list = _context.Jobs.Include(p => p.Renter).ToList();
            list = list.Where(p => p.Status == "Closed").ToList();
            Random random = new Random();

            foreach (var item in list)
            {
                DateTime temp = item.CreateAt;

                temp = temp.AddDays(random.Next(7));

                item.FinishAt = temp;
            }
            _context.SaveChanges();

            //var list = _context.Jobs.Where(p => p.Floorprice < 2500).ToList();
            //foreach (var item in list)
            //{
            //    item.Floorprice = 850000;
            //    item.Cellingprice = 1000000;
            //}
            //_context.SaveChanges();
            return Ok();
        }

        [HttpGet]
        public String FormatDataDB()
        {
            //var name = "";
            //foreach (var item in _context.Accounts.ToList())
            //{
            //    name = item.AvatarUrl.Substring(item.AvatarUrl.LastIndexOf("\\") + 1);
            //    item.AvatarUrl = "freelancervn.somee.com/api/images/avatars/" + name;
            //}
            //foreach (var item in _context.CapacityProfiles.ToList())
            //{
            //    name = item.ImageUrl.Substring(item.ImageUrl.LastIndexOf("\\") + 1);
            //    item.ImageUrl = "freelancervn.somee.com/api/images/images/" + name;
            //}    
            //foreach (var item in _context.Specialties.ToList())
            //{
            //    name = item.Image.Substring(item.Image.LastIndexOf("\\") + 1);
            //    item.Image = "freelancervn.somee.com/api/images/assets/" + name;
            //}
            //_context.SaveChanges();
            //foreach (var item in _context.Accounts.ToList())
            //{
            //    item.AvatarUrl = "\\Avatars\\" + item.AvatarUrl;
            //}
            //_context.SaveChanges();

            //foreach (var item in _context.CapacityProfiles.ToList())
            //{
            //    item.ImageUrl = "\\Images\\" + item.ImageUrl;
            //}
            //_context.SaveChanges();

            //foreach (var item in _context.Specialties.ToList())
            //{
            //    item.Image = "\\Assets\\" + item.Image;
            //}
            //_context.SaveChanges();

            return TimeVN.Now().ToString();
        }


    }
}
