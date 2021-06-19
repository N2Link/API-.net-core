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
        //đăng ký thiết bị 
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
            Clients.Client(Context.ConnectionId).SendAsync("Connected"); // báo đã connect thành công
            Console.WriteLine("userid: "+ id.ToString() + "\tconnectid: " + Context.ConnectionId);
            
        }
        //hủy kết nối thiết bị
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
        //Gửi tin nhắn
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


            if (ChatUsers.ContainsKey(toUserId))
            {
                foreach (var item in ChatUsers[toUserId])
                {
                    await Clients.Client(item).SendAsync("ReceiveMessage", jobId, freelancerId,
                        fromUserId, toUserId, message, msg.Time);//gọi người nhận nhận tin nhắn
                }  
            }
            if (ChatUsers.ContainsKey(fromUserId))
            {
                foreach (var item in ChatUsers[fromUserId])
                {
                    await Clients.Client(item).SendAsync("SendMessage_Successfully", jobId, freelancerId,
                        fromUserId, toUserId, message, msg.Time);// thông báo người gửi đã gửi thành công tin nhắn 
                }
            }


        }

        //xem tin nhắn, gọi khi người dùng mở mục chat => chuyển tin nhắn chưa xem thành đã xem
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

        //Renter gửi đề nghị số tiền sẽ trả cho freelancer
        public async Task PutMoney(int jobId, int freelancerId, int money)
        {
            var job = _context.Jobs.Find(jobId);
            if (job == null||!(job.Status == "Waiting"&&job.Deadline<DateTime.Now))
            {
                return;
            }
            var freelancer = _context.Accounts.Find(freelancerId);
            if(freelancer == null)
            {
                return;
            }

            if (ChatUsers.ContainsKey(freelancerId))
            {
                try
                {
                    //Chuyển lời đề nghị đến cho Freelancer
                    foreach (var contextId in ChatUsers[freelancerId])
                    {
                        await Clients.Client(contextId).SendAsync("SuggestedPrice", jobId, freelancerId, money );
                    }
                }
                catch {}
            }
            if (!ChatUsers.ContainsKey(job.RenterId))
            {
                return;
            }
            //Thông báo gửi đề nghị thành công
            foreach (var contextId in ChatUsers[job.RenterId])
            {
                await Clients.Clients(contextId).SendAsync("PutMoney_Successfully", jobId, freelancerId);
            }

        }
        //Freelancer xác nhận có đồng ý số tiền hay không
        public async Task ConfirmSuggestedPrice(int jobId, int freelancerId, int money, bool confirm)
        {
            var job = _context.Jobs.Find(jobId);
            if (job == null || !(job.Status == "Waiting" && job.Deadline < DateTime.Now))
            {
                return;
            }
            var freelancer = _context.Accounts.Find(freelancerId);
            if (freelancer == null)
            {
                return;
            }
            var renter = _context.Accounts.Find(job.RenterId);
            if (renter == null || renter.Balance < money)
            {
                return;
            }
            if (confirm)
            {
                job.Price += money;
                job.Status = "In progress";
                renter.Balance -= money;
                await _context.SaveChangesAsync();
            }

            if (ChatUsers.ContainsKey(freelancerId))
            {
                try
                {
                    //thông báo đã xác nhận giá tiền cho freelancer confirm = yes/no
                    foreach (var contextId in ChatUsers[freelancerId])
                    {
                        await Clients.Client(contextId).SendAsync("Confirm", jobId, freelancerId, confirm);
                    }
                }
                catch { }
            }

            if (!ChatUsers.ContainsKey(job.RenterId))
            {
                return;
            }
            //thông báo đã xác nhận giá tiền cho renter confirm = yes/no
            foreach (var contextId in ChatUsers[job.RenterId])
            {
                await Clients.Clients(contextId).SendAsync("Confirm", jobId, freelancerId, confirm);
            }

        }
        //freelancer gửi yêu cầu kết thông công việc
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
            if (ChatUsers.ContainsKey(job.RenterId))
            {
                //gửi đến renter yêu cầu kết thúc công việc của freelancer
                foreach (var contextId in ChatUsers[job.RenterId])
                {
                    await Clients.Client(contextId).SendAsync("Requestfinish", jobId, freelancerId);
                }
            }

            if (!ChatUsers.ContainsKey(freelancerId))
            {
                return;
            }
            // thông báo cho freelancer đã gửi yêu cầu kết thúc thành công
            foreach (var contextId in ChatUsers[freelancerId])
            {
                await Clients.Client(contextId).SendAsync("SendFinishRequest_Successfully", jobId, freelancerId);
            }
        }

        // renter gửi xác nhận kết thúc job 
        public async Task FinishJob(int jobId)
        {
            var job = _context.Jobs.Find(jobId);
            if (job == null)
            {
                return;
            }
            job.Status = "Finish";
            var freelancer = _context.Accounts.Find(job.FreelancerId);
            freelancer.Balance += job.Price;

            await _context.SaveChangesAsync();
            int freelancerId = freelancer.Id;
            if (!ChatUsers.ContainsKey(freelancerId))
            {
                return;
            }

            if (ChatUsers.ContainsKey(freelancerId))
            {
                try
                {
                    // gửi xác nhận kết thúc job từ renter đến freelancer
                    foreach (var contextId in ChatUsers[freelancerId])
                    {
                        await Clients.Client(contextId).SendAsync("Finish", jobId, freelancerId);
                    }
                }
                catch { }
            }

            if (!ChatUsers.ContainsKey(job.RenterId))
            {
                return;
            }
            // thông báo renter đã gửi xác nhận kết thúc job thành công
            foreach (var contextId in ChatUsers[job.RenterId])
            {
                await Clients.Client(contextId).SendAsync("FinishJob_Successfully", jobId, freelancerId);
            }
        }

        // renter gửi yêu cầu làm lại job 
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

            if (ChatUsers.ContainsKey(freelancerId))
            {
                try
                {
                    // gửi yêu cầu làm lại job từ renter đến freelancer
                    foreach (var contextId in ChatUsers[freelancerId])
                    {
                        await Clients.Client(contextId).SendAsync("RequestRework", jobId, freelancerId);
                    }
                }
                catch { }
            }


            if (!ChatUsers.ContainsKey(job.RenterId))
            {
                return;
            }

            // xác nhận renter gửi yêu cầu làm lại job thành công
            foreach (var contextId in ChatUsers[job.RenterId])
            {
                await Clients.Client(contextId).SendAsync("SendRequestRework_Successfully", jobId, freelancerId);
            }
        }
        // renter gửi yêu cầu hủy job
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

            if (ChatUsers.ContainsKey(freelancerId))
            {
                try
                {
                    // gửi yêu cầu hủy job từ renter đến freelancer
                    foreach (var contextId in ChatUsers[freelancerId])
                    {
                        await Clients.Client(contextId).SendAsync("RequestCancellation", jobId, freelancerId);
                    }
                }
                catch {}
            }

            if (!ChatUsers.ContainsKey(job.RenterId))
            {
                return;
            }
            // xác nhận renter gửi yêu cầu hủy job thành công
            foreach (var contextId in ChatUsers[job.RenterId])
            {
                await Clients.Client(contextId).SendAsync("SendRequestCancellation_Successfully", jobId, freelancerId);
            }
        }

        // admin gửi quyết định cho các request
        public async Task SendConfirmRequest(int jobId, string status, int adminId,string message)
        {
            var job = _context.Jobs.Find(jobId);
            if (job == null)
            {
                return;
            }
            var renter = _context.Accounts.Find(job.RenterId);

            int freelancerId = Int32.Parse(job.FreelancerId.ToString());

            job.Status = status;
            if(status == "Cancellation")
            {
                renter.Balance += job.Price;
            }

            Message msg = new Message()
            {
                JobId = job.Id,
                FreelancerId = freelancerId,
                Message1 = message,
                SenderId = adminId,
                ReceiveId = adminId,
                Status = "Seen",
                Time = DateTime.Now,
            };//đây là message của admin cho quyết định của mình, load db check thấy senderId == receiveId 
            //thì để nó là một dòng note trên màn hình

            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();


            //gửi  quyết định của admin đến user
            foreach (var contextId in ChatUsers[freelancerId])
            {
                await Clients.Client(contextId).SendAsync("ConfirmRequest", jobId, freelancerId, status, message);
            }

            foreach (var contextId in ChatUsers[job.RenterId])
            {
                await Clients.Client(contextId).SendAsync("ConfirmRequest", jobId, freelancerId, status, message);
            }

        }

    }
}
