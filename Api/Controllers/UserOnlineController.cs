using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]
    public class UserOnlineController : Controller
    {
        [HttpGet]
        public Dictionary<int, List<string>> GetUserOnline()
        {
            return ChatHub.ChatUsers;
        }     

        [HttpDelete("id")]
        public IActionResult DeleteUser(int id)
        {
            ChatHub.ChatUsers.Remove(id);
            return Ok();
        }
    }
}
