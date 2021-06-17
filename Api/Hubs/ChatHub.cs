using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Hubs
{
    public class ChatHub : Hub
    {
        private readonly FreeLancerVNContext _context;
        static public Dictionary<int, List<string>> ChatUsers = new Dictionary<int, List<string>>();
        public ChatHub(FreeLancerVNContext context)
        {
            _context = context;
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
        public void Connect(int id)
        {
            if (ChatUsers.ContainsKey(id))
            {
                if (ChatUsers[id].Contains(Context.ConnectionId))
                {
                    return;
                }
                ChatUsers[id].Add(Context.ConnectionId);
            }
            else
            {
                List<string> vs = new List<string>();
                vs.Add(Context.ConnectionId);
                ChatUsers.Add(id, vs);
            }
            Console.WriteLine("userid: "+ id.ToString() + "\tconnectid: " + Context.ConnectionId);
            
        }
        public void Disconnect(int id)
        {
            if (!ChatUsers.ContainsKey(id))
            {
                return;
            }
            if (ChatUsers[id].Contains(Context.ConnectionId))
            {
                ChatUsers[id].Remove(Context.ConnectionId);
                if(ChatUsers[id].Count == 0)
                {
                    ChatUsers.Remove(id);
                }
            }
            else
            {
                return;
            }
        }

        public async Task SendMessage(int jobId, int freelancerId,
            int toUserId, string message)
        {
            var fromUserId = ChatUsers.SingleOrDefault(p => p.Value.Contains(Context.ConnectionId)).Key;
            
            Message msg = new Message()
            {
                JobId = jobId,
                FreelancerId = freelancerId,
                SenderId = fromUserId,
                ReceiveId = toUserId,
                Message1 = message,
                Time = DateTime.Now,
                Status = "Unseen"
            };

            try
            {
                _context.Messages.Add(msg);
                await _context.SaveChangesAsync();
            }catch(Exception e) 
            {
                Console.WriteLine(e.Message); 
                return;
            }


            if (ChatUsers.ContainsKey(toUserId)&&ChatUsers.ContainsKey(fromUserId))
            {
                foreach (var item in ChatUsers[toUserId])
                {
                    await Clients.Client(item).SendAsync("ReceiveMessage", jobId, freelancerId,
                        fromUserId, toUserId, message, msg.Time);
                }
            }
            Console.WriteLine("SendMessage");
        }

        public async Task SeeMessage(int jobId, int freelancerId, int fromUserId)
        {
            var list = _context.Messages.Where(p => p.JobId == jobId && p.FreelancerId == freelancerId
               && p.Status == "Unseen" && p.ReceiveId == fromUserId).ToList();
            if(list.Count == 0)
            {
                return;
            }
            int senderId = list.First().SenderId;

            foreach (var msg in list)
            {
                msg.Status = "Seen";
            }
            await _context.SaveChangesAsync();

            if (ChatUsers.ContainsKey(senderId))
            {
                foreach (var contextid in ChatUsers[senderId])
                {
                    await Clients.Client(contextid).SendAsync("Seen", jobId, freelancerId, senderId);
                }
            }
        }

        public async Task SendFinishRequest(int jobId)
        {
            var job = _context.Jobs.Find(jobId);
            if(job == null)
            {
                return;
            }
            int freelancerId = Int32.Parse(job.FreelancerId.ToString());

            job.Status = "Request finish";
            await _context.SaveChangesAsync();
            if (!ChatUsers.ContainsKey(job.RenterId))
            {
                return;
            }
            foreach (var contextId in ChatUsers[job.RenterId])
            {
                await Clients.Client(contextId).SendAsync("Requestfinish", jobId, freelancerId);
            }


            foreach (var contextId in ChatUsers[freelancerId])
            {
                await Clients.Client(contextId).SendAsync("SendFinishRequest_Successfully", jobId, freelancerId);
            }
        }
        public async Task FinishJob(int jobId)
        {
            var job = _context.Jobs.Find(jobId);
            if (job == null)
            {
                return;
            }
            job.Status = "Done";
            await _context.SaveChangesAsync();
            int freelancerId = Int32.Parse(job.FreelancerId.ToString());
            if (!ChatUsers.ContainsKey(freelancerId))
            {
                return;
            }

            foreach (var contextId in ChatUsers[freelancerId])
            {
                await Clients.Client(contextId).SendAsync("Done",jobId, freelancerId);
            }

            foreach (var contextId in ChatUsers[job.RenterId])
            {
                await Clients.Client(contextId).SendAsync("FinishJob_Successfully", jobId, freelancerId);
            }
        }      
        public async Task SendRequestRework(int jobId)
        {
            var job = _context.Jobs.Find(jobId);
            if (job == null)
            {
                return;
            }
            job.Status = "Request rework";
            await _context.SaveChangesAsync();
            int freelancerId = Int32.Parse(job.FreelancerId.ToString());
            if (!ChatUsers.ContainsKey(freelancerId))
            {
                return;
            }

            foreach (var contextId in ChatUsers[freelancerId])
            {
                await Clients.Client(contextId).SendAsync("RequestRework", jobId, freelancerId);
            }

            foreach (var contextId in ChatUsers[job.RenterId])
            {
                await Clients.Client(contextId).SendAsync("SendRequestRework_Successfully", jobId, freelancerId);
            }
        }       

        public async Task SendRequestCancellation(int jobId)
        {
            var job = _context.Jobs.Find(jobId);
            if (job == null)
            {
                return;
            }
            job.Status = "Request cancellation";
            await _context.SaveChangesAsync();
            int freelancerId = Int32.Parse(job.FreelancerId.ToString());
            if (!ChatUsers.ContainsKey(freelancerId))
            {
                return;
            }

            foreach (var contextId in ChatUsers[freelancerId])
            {
                await Clients.Client(contextId).SendAsync("RequestCancellation", jobId, freelancerId);
            }

            foreach (var contextId in ChatUsers[job.RenterId])
            {
                await Clients.Client(contextId).SendAsync("SendRequestCancellation_Successfully", jobId, freelancerId);
            }
        }

        public async Task SendConfirmRequest(int jobId, bool confirm)
        {
            var job = _context.Jobs.Find(jobId);
            if (job == null)
            {
                return;
            }
            int freelancerId = Int32.Parse(job.FreelancerId.ToString());

            foreach (var contextId in ChatUsers[freelancerId])
            {
                await Clients.Client(contextId).SendAsync("ConfirmRequest", jobId, freelancerId, confirm);
            }

            foreach (var contextId in ChatUsers[job.RenterId])
            {
                await Clients.Client(contextId).SendAsync("ConfirmRequest", jobId, freelancerId, confirm);
            }

        }

    }
}
