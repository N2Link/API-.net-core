using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        public String FormatDataDB()
        {
            var name = "";
            foreach (var item in _context.Accounts.ToList())
            {
                name = item.AvatarUrl.Substring(item.AvatarUrl.LastIndexOf("\\") + 1);
                item.AvatarUrl = "freelancervn.somee.com/api/images/avatars/" + name;
            }
            foreach (var item in _context.CapacityProfiles.ToList())
            {
                name = item.ImageUrl.Substring(item.ImageUrl.LastIndexOf("\\") + 1);
                item.ImageUrl = "freelancervn.somee.com/api/images/images/" + name;
            }    
            foreach (var item in _context.Specialties.ToList())
            {
                name = item.Image.Substring(item.Image.LastIndexOf("\\") + 1);
                item.Image = "freelancervn.somee.com/api/images/assets/" + name;
            }
            _context.SaveChanges();
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

            return "OK";
        }
    }
}
