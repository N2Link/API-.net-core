using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Api.Enities;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        IWebHostEnvironment _webHostEnvironment;
        FreeLancerVNContext _context;
        string rootpath;
        public ImagesController(IWebHostEnvironment webHostEnvironment, FreeLancerVNContext context)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
            rootpath = _webHostEnvironment.WebRootPath;
        }
/*        [HttpGet("avatars/{name}")]
        public IActionResult GetAvatar(string name)
        {
            string contentType;
            var fileName = _webHostEnvironment.WebRootPath + "\\Avatars\\"+name;
            new FileExtensionContentTypeProvider().TryGetContentType(fileName, out contentType);
            return PhysicalFile(fileName, contentType);
        } */
        [HttpGet("{url}")]
        public IActionResult GetImage(string url)
        {
            string contentType;
            var fileName = rootpath + url;
            try
            {
                new FileExtensionContentTypeProvider().TryGetContentType(fileName, out contentType);
                return PhysicalFile(fileName, contentType);
            }
            catch (Exception)
            {

                return Ok();
            }

        }
        [AllowAnonymous]
        [HttpPost("avatars")]
        public IActionResult PostAvatar([FromBody] ImageModel imageModel)
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
            var account = _context.Accounts.SingleOrDefault(p => p.Email == email);
            if(account == null)
            {
                return BadRequest();
            }


            if(account.AvatarUrl != "\\Avatars\\default.jpg")
            {
                try
                {
                    System.IO.File.Delete(rootpath + account.AvatarUrl);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            string newURL = "\\Avatars\\" + account.Id+"_"+ imageModel.Name;

            using (FileStream fs = System.IO.File.Create(rootpath+newURL))
            {
                fs.Close();
                System.IO.File.WriteAllBytes(rootpath + newURL, Convert.FromBase64String(imageModel.ImageBase64));

            }
            account.AvatarUrl = newURL;
            _context.Entry(account).State = EntityState.Modified;
            _context.SaveChanges();
            return Ok(new {message = "Successful", url=newURL });
        }

        [HttpPost("images")]
        public async Task<IActionResult> PostImageCProfile(ImageCPEdit imageCPEdit)
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

            var cp = _context.CapacityProfiles.Find(imageCPEdit.CPID);
            if(cp== null)
            {
                return BadRequest();
            }

            try
            {
                System.IO.File.Delete(rootpath + cp.ImageUrl);
            }
            catch (Exception)
            {
                throw;
            }

            string newURL = "\\Images\\" + account.Id + "_"
                + cp.Name.Trim().Substring(0, 10) + "_" + imageCPEdit.ImageName;
            using (FileStream fs = System.IO.File.Create(rootpath+newURL))
            {
                fs.Close();
                System.IO.File.WriteAllBytes(rootpath+newURL, Convert.FromBase64String(imageCPEdit.ImageBase64));
            }
            cp.ImageUrl= newURL;
            _context.Entry(cp).State = EntityState.Modified;
            _context.SaveChanges();
            return Ok(new { message = "Successful", url = newURL });
        }
    }
}
