using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Api.Enities;

namespace Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly FreeLancerVNContext _context;

        public MessagesController(FreeLancerVNContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<MessageModel>> PostMessage(MessageModel messagePost)
        {
            Message message = new Message()
            {
                JobId = messagePost.JobId,
                ReceiveId = messagePost.ReceiveId,
                SenderId = messagePost.SenderId,
                Message1 = messagePost.Message1,
                Status = "unseen"
            };
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool MessageExists(int id)
        {
            return _context.Messages.Any(e => e.Id == id);
        }
    }
}
