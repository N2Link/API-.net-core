using System;
using System.Collections.Generic;
using System.Linq;
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
        [HttpGet]
        public String FormatDataDB()
        {
/*            foreach (var item in _context.Accounts.ToList())
            {
                item.AvatarUrl = "\\Avatars\\" + item.AvatarUrl;
            }
            _context.SaveChanges();
                    
            foreach (var item in _context.CapacityProfiles.ToList())
            {
                item.ImageUrl = "\\Images\\" + item.ImageUrl;
            }
            _context.SaveChanges();
              
            foreach (var item in _context.Specialties.ToList())
            {
                item.Image = "\\Assets\\" + item.Image;
            }
            _context.SaveChanges();*/

            return "OK";
        }
    }
}
