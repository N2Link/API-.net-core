using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Api.Hubs
{
    public class ChatHub : Hub
    {
        static Dictionary<int, List<string>> ChatUsers = new Dictionary<int, List<string>>();
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
        public void Connect(int id)
        {
            if (ChatUsers.ContainsKey(id))
            {
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
        public async Task Sendmessage(int toUserId, string message)
        {
            var fromUserId = ChatUsers.SingleOrDefault(p => p.Value.Contains(Context.ConnectionId)).Key;
            if (ChatUsers.ContainsKey(toUserId)&&ChatUsers.ContainsKey(fromUserId))
            {
                foreach (var item in ChatUsers[toUserId])
                {
                    await Clients.Client(item).SendAsync("ReceiveMessage", fromUserId, toUserId, message);
                }
            }
            Console.WriteLine("Sendmessage");

        }
    }
}
