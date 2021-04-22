using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        IWebHostEnvironment _webHostEnvironment;
        FreeLancerVNContext _context;
        public ImagesController(IWebHostEnvironment webHostEnvironment, FreeLancerVNContext context)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }
        [HttpGet("avatars/{name}")]
        public IActionResult GetAvatar(string name)
        {
            string contentType;
            var fileName = _webHostEnvironment.WebRootPath + "\\Avatars\\"+name;
            new FileExtensionContentTypeProvider().TryGetContentType(fileName, out contentType);
            return PhysicalFile(fileName, contentType);
        }

        [HttpPost("avatars")]
        public String PostAvatar([FromBody] ImageModel imageModel)
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
            var accountID = _context.Accounts.SingleOrDefault(p => p.Email == email).Id;

            string avatarUrl = _webHostEnvironment.WebRootPath + "\\Avatars\\";

            if (System.IO.File.Exists(avatarUrl+imageModel.Url))
            {
                if (imageModel.Name != "default.jpg")
                {
                    System.IO.File.Delete(avatarUrl+imageModel.Url);
                }
            }
            string newURL = accountID+"_"+ imageModel.Name;

            using (FileStream fs = System.IO.File.Create(newURL))
            {
                System.IO.File.WriteAllBytes(avatarUrl+imageModel.Name, Convert.FromBase64String(imageModel.ImageBase64));
            }
            return newURL;
        }
        [HttpPost("images")]
        public List<String> PostImageCProfile([FromBody] List<ImageModel> image)
        {
            return new List<string>();
        }
    }
}
