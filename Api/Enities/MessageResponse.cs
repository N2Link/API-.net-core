using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Enities
{
    public class MessageResponse
    {
        public MessageResponse(Message message)
        {
            this.Id = message.Id;
            this.Message1 = message.Message1;
            this.Status = message.Status;
            this.JobId = Int32.Parse(message.JobId.ToString());
            this.FreelancerId = message.FreelancerId;
            this.SenderId = message.SenderId;
            this.ReceiveId = message.ReceiveId;
            this.Time = message.Time;

            this.Type = message.Type;
            this.Confirmation = message.Confirmation;
        }

        public int Id { get; set; }
        public int JobId { get; set; }
        public int SenderId { get; set; }
        public int ReceiveId { get; set; }
        public string Message1 { get; set; }
        public string Status { get; set; }
        public DateTime Time { get; set; }
        public int FreelancerId { get; set; }
        public string Type { get; set; }
        public string Confirmation { get; set; }
    }
}
