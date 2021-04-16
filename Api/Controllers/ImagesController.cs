using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        IWebHostEnvironment _webHostEnvironment;
        public ImagesController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpPost("Avatars")]
        public String PostAvatar([FromBody] ImageModel imageModel)
        {

            if (System.IO.File.Exists(imageModel.Url))
            {
                if (imageModel.Name != _webHostEnvironment.WebRootPath + "\\Avatars\\default.jpg")
                {
                    System.IO.File.Delete(imageModel.Url);
                }
            }
            string newURL = _webHostEnvironment.WebRootPath + "\\Avatars\\" + imageModel.Name;
            using (FileStream fs = System.IO.File.Create(newURL))
            {
                System.IO.File.WriteAllBytes(newURL, Convert.FromBase64String(imageModel.ImageBase64));
            }
            return newURL;
        }
        [HttpPost("Images")]
        public List<String> PostImageCProfile([FromBody] List<ImageModel> image)
        {
            return new List<string>();
        }
    }
}
