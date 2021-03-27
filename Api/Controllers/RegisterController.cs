using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;
using Api.Service;
using Microsoft.AspNetCore.Authorization;
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
        public RegisterController()
        {
            userService = new UserService();
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            Account account = new Account();
            account.Username = model.Username;
            account.Firstname = model.FirstName;
            account.LastName = model.LastName;
            account.Phone = model.Phone;
            account.Email = model.Email;
            account.RoleId = model.RoleID;

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
