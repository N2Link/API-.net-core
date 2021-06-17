using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Enities;
using Api.Models;
using Api.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Api.Helpers;
using Api.Models;
using Microsoft.AspNetCore.Cors;

namespace Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]

    public class RegisterController : ControllerBase
    {
        IUserService userService;
        IWebHostEnvironment _webHostEnvironment;
        public RegisterController(IWebHostEnvironment webHostEnvironment, IUserService  userService)
        {
            _webHostEnvironment = webHostEnvironment;
            this.userService = userService ;
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            Account account = new Account();
            account.Name = model.Name;
            account.Phone = model.Phone;
            account.Email = model.Email;
            account.RoleId = model.RoleID;
            //string path = _webHostEnvironment.WebRootPath + "\\Avatars\\default.jpg";
            account.AvatarUrl = "freelancervn.somee.com/api/images/avatars/default.jpg";
            try
            {
                IUserService.UserEntitis userEntitis =
                 userService.Create(account, model.Password).createUserToken();
                return Ok(userEntitis);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
