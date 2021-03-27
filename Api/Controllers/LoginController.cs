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
    public class LoginController : ControllerBase
    {
        private IUserService userService;
        public LoginController()
        {
            this.userService = new UserService();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] LoginModel model)
        {
            try
            {
                return Ok(userService.Auth(model.Username, model.Password));
            }catch(AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
