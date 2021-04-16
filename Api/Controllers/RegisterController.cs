using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;
using Api.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Helpers;

namespace Api.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        IUserService userService;
        IWebHostEnvironment _webHostEnvironment;
        public RegisterController(IWebHostEnvironment webHostEnvironment)
        {
            userService = new UserService();
            _webHostEnvironment = webHostEnvironment;
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
            string path = _webHostEnvironment.WebRootPath + "\\Avatars\\default.jpg";
            account.AvatarUrl = path;

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
