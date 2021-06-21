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
            this.Job = new ResponseIdName(message.Job);
            this.Freelancer = new ResponseIdName(message.Freelancer);
            this.Sender = new ResponseIdName(message.Sender);
            this.Receiver = new ResponseIdName(message.Receive);
            this.Time = message.Time;

            this.Form = message.Form;
            this.Confirmation = message.Confirmation;
        }
        public int Id { get; set; }
        public ResponseIdName Job { get; set; }
        public ResponseIdName Freelancer { get; set; }
        public ResponseIdName Sender { get; set; }
        public ResponseIdName Receiver { get; set; }
        public string AvatarUrl { get; set; }
        public string Message1 { get; set; }
        public string Status { get; set; }
        public string Form { get; set; }
        public string Confirmation { get; set; }
        public DateTime Time { get; set; }


    }
}
