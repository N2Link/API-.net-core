using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Api.Service;
using Api.Enities;

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
                Time = TimeVN.Now(),
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
                    await Clients.Client(item).SendAsync("ReceiveMessage", new MessageResponse(msg));//gọi người nhận nhận tin nhắn
                }  
            }

            //gọi người nhận nhận tin nhắn
            foreach (var item in ChatUsers[fromUserId])
            {
                await Clients.Client(item).SendAsync("SendMessage_Successfully", new MessageResponse(msg));
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

        //Undo
        public async Task Undo(int jobId, int freelancerId, int msgId)
        {
            var job = _context.Jobs.Find(jobId);
            if (job == null)
            {
                return;
            };
            var renter = _context.Accounts.Find(job.RenterId);
            if(renter == null)
            {
                return;
            }
            var freelancer = _context.Accounts.Find(freelancerId);
            if (freelancer == null)
            {
                return;
            }

            var msg = _context.Messages
                .Where(p => p.Type == "SuggestedPrice" && p.JobId == jobId 
                &&p.FreelancerId == freelancerId).ToList();
            if(msg.Count() == 0 || msg.Last().Confirmation != null)
            {
                return;
            }

            renter.Balance += job.Price;
            msg.Last().Confirmation = "Undo";
            job.Price = 0;

            await _context.SaveChangesAsync();

            if (ChatUsers.ContainsKey(msg.Last().ReceiveId))
            {
                foreach (var item in ChatUsers[msg.Last().ReceiveId])
                {
                    await Clients.Client(item).SendAsync("Undo_Form", jobId, freelancerId, msg);
                }
            }
        }

        //Renter gửi đề nghị số tiền sẽ trả cho freelancer
        public async Task SuggestedPrice(int jobId, int freelancerId, int money)
        {

            var job = _context.Jobs.Find(jobId);
            if (job == null || job.Status != "Waiting")
            {
                return;
            }

            var freelancer = _context.Accounts.Find(freelancerId);
            if(freelancer == null)
            {
                return;
            }     
            
            var renter = _context.Accounts.Find(job.RenterId);

            if(renter == null || renter.Balance < money)
            {
                return;
            }

            Message msg = new Message()
            {
                JobId = jobId,
                FreelancerId = freelancerId,
                Message1 = money.ToString(),
                ReceiveId = freelancerId,
                SenderId = job.RenterId,
                Status = "Unseen",
                Time = TimeVN.Now(),
                Type = "SuggestedPrice"
            };
            
            job.Price += money;
            renter.Balance -= money;

            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();

            if (ChatUsers.ContainsKey(freelancerId))
            {
                try
                {
                    //Chuyển lời đề nghị đến cho Freelancer
                    foreach (var contextId in ChatUsers[freelancerId])
                    {
                        await Clients.Client(contextId).SendAsync("SuggestedPrice", new MessageResponse(msg));
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
                Console.WriteLine(job.RenterId.ToString()+"\t"+contextId + "\tPutMoney_Successfully");
                await Clients.Clients(contextId).SendAsync("PutMoney_Successfully", new MessageResponse(msg));
            }

        }
        //Freelancer xác nhận có đồng ý số tiền hay không
        public async Task ConfirmSuggestedPrice(int jobId, int freelancerId, int msgId, bool confirm)
        {
            var job = _context.Jobs.Find(jobId);
            if (job == null || job.Status != "Waiting")
            {
                return;
            }

            var freelancer = _context.Accounts.Find(freelancerId);
            if (freelancer == null)
            {
                return;
            }
            var renter = _context.Accounts.Find(job.RenterId);
            var msg = _context.Messages.Find(msgId);
            if(msg == null||msg.Type != "SuggestedPrice") { return; }
            int money = Int32.Parse(msg.Message1);

            if (renter == null)
            {
                return;
            }

            if (confirm)
            {
                job.Status = "In progress";
                job.FreelancerId = freelancerId;
                job.StartAt = TimeVN.Now();
                msg.Confirmation = "Accept";
            }
            else
            {
                renter.Balance += job.Price;
                job.Price = 0;
                msg.Confirmation = "Decline";
            }
            await _context.SaveChangesAsync();


            if (!ChatUsers.ContainsKey(job.RenterId))
            {
                return;
            }
            //thông báo đã xác nhận giá tiền cho renter Confirmation = Accept/Decline
            foreach (var contextId in ChatUsers[job.RenterId])
            {
                await Clients.Clients(contextId).SendAsync("Confirm", new MessageResponse(msg));
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
            Message msg = new Message()
            {
                JobId = jobId,
                FreelancerId = freelancerId,
                ReceiveId = job.RenterId,
                SenderId = freelancerId,
                Message1 = "Freelancer đã gửi yêu cầu kết thúc dự án",
                Status = "Unseen",
                Time = TimeVN.Now(),
                Type = "FinishRequest"
            };

            job.Status = "Request finish";

            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();

            if (ChatUsers.ContainsKey(job.RenterId))
            {
                //gửi đến renter yêu cầu kết thúc công việc của freelancer
                foreach (var contextId in ChatUsers[job.RenterId])
                {
                    await Clients.Client(contextId).SendAsync("Requestfinish", new MessageResponse(msg));
                }
            }

            if (!ChatUsers.ContainsKey(freelancerId))
            {
                return;
            }
            // thông báo cho freelancer đã gửi yêu cầu kết thúc thành công
            foreach (var contextId in ChatUsers[freelancerId])
            {
                await Clients.Client(contextId).SendAsync("SendFinishRequest_Successfully", new MessageResponse(msg));
            }
        }

        // renter gửi xác nhận kết thúc job 
        public async Task FinishJob(int jobId, int msgId)
        {
            var job = _context.Jobs.Find(jobId);
            if (job == null)
            {
                return;
            }

            var msg = _context.Messages.Find(msgId);
            if(msg == null)
            {
                return;
            }
            if(msg.Type != "FinishRequest")
            {
                return;
            }else if(msg.Confirmation != null) { return; }

            msg.Confirmation = "Finished";
            job.Status = "Finished";
            job.FinishAt = TimeVN.Now();
            var freelancer = _context.Accounts.Find(job.FreelancerId);
            freelancer.Balance += job.Price;

            await _context.SaveChangesAsync();
            int freelancerId = freelancer.Id;

            if (ChatUsers.ContainsKey(freelancerId))
            {
                try
                {
                    // gửi xác nhận kết thúc job từ renter đến freelancer
                    foreach (var contextId in ChatUsers[freelancerId])
                    {
                        await Clients.Client(contextId).SendAsync("Finish", jobId, freelancerId, msgId);
                    }
                }
                catch { }
            }
        }

        // renter gửi yêu cầu làm lại job 
        public async Task SendRequestRework(int jobId, int msgId)
        {
            var job = _context.Jobs.Find(jobId);
            if (job == null)
            {
                return;
            }
            var msg = _context.Messages.Find(msgId);
            if (msg == null)
            {
                return;
            }
            if (msg.Type != "FinishRequest")
            {
                return;
            }
            else if (msg.Confirmation != null) { return; }

            msg.Confirmation = "Request rework";
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
                        await Clients.Client(contextId).SendAsync("RequestRework", jobId, freelancerId, msgId);
                    }
                }
                catch { }
            }
        }
        // renter gửi yêu cầu hủy job
        public async Task SendRequestCancellation(int jobId, int msgId = 0)
        {
            var job = _context.Jobs.Find(jobId);
            if (job == null)
            {
                return;
            }

            if(msgId == 0)
            {
                Message newMsg = new Message()
                {
                    JobId = jobId,
                    FreelancerId = Int32.Parse(job.FreelancerId.ToString()),
                    SenderId = job.RenterId,
                    ReceiveId = Int32.Parse(job.FreelancerId.ToString()),
                    Message1 = "Renter đã yêu cầu hủy bỏ công việc",
                    Status = "Unseen",
                    Time = TimeVN.Now(),
                    Type = "FinishRequest",
                    Confirmation = "Request cancellation"
                };

                job.Status = "Request cancellation";
                _context.Messages.Add(newMsg);
                await _context.SaveChangesAsync();
                int flId = Int32.Parse(job.FreelancerId.ToString());

                if (ChatUsers.ContainsKey(flId))
                {
                    try
                    {
                        // gửi yêu cầu hủy job từ renter đến freelancer
                        foreach (var contextId in ChatUsers[flId])
                        {
                            await Clients.Client(contextId).SendAsync("RequestCancellation", newMsg);
                        }
                    }
                    catch { }
                }
                return;
            }

            var msg = _context.Messages.Find(msgId);
            if (msg == null)
            {
                return;
            }
            if (msg.Type != "FinishRequest")
            {
                return;
            }
            else if (msg.Confirmation != null) { return; }

            msg.Confirmation = "Request cancellation";

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
                        await Clients.Client(contextId).SendAsync("RequestCancellation", jobId, freelancerId, msgId);
                    }
                }
                catch {}
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
                job.FinishAt = TimeVN.Now();
            }

            var msgEd = _context.Messages.Where(p => p.JobId == jobId
            && p.FreelancerId == freelancerId && p.Type == "FinishRequest").ToList().Last();

            msgEd.Confirmation = status;


            Message msgToRenter = new Message()
            {
                JobId = job.Id,
                FreelancerId = freelancerId,
                Message1 = message,
                SenderId = adminId,
                ReceiveId = job.RenterId,
                Status = "Unseen",
                Confirmation = status,
                Type = "Notification",
                Time = TimeVN.Now(),
            };
            //đây là message của admin cho quyết định của mình, load db check thấy senderId == receiveId 
            //thì để nó là một dòng note trên màn hình

            _context.Messages.Add(msgToRenter);
            await _context.SaveChangesAsync();

            //gửi  quyết định của admin đến user
            if (ChatUsers.ContainsKey(freelancerId))
            {
                foreach (var contextId in ChatUsers[freelancerId])
                {
                    await Clients.Client(contextId).SendAsync("ConfirmRequest", new MessageResponse(msgToRenter));
                }
            }

            if (ChatUsers.ContainsKey(renter.Id))
            {
                foreach (var contextId in ChatUsers[job.RenterId])
                {
                    await Clients.Client(contextId).SendAsync("ConfirmRequest", new MessageResponse(msgToRenter));
                }
            }


        }
    }
}
