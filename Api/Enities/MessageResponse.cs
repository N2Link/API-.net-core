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
            this.Message1 = message.Message1;
            this.Status = message.Status;
            this.Job = new ResponseIdName(message.Job);
            this.Sender = new ResponseIdName(message.Sender);
            this.Receive = new ResponseIdName(message.Receive);
        }
        public ResponseIdName Job { get; set; }
        public ResponseIdName Sender { get; set; }
        public ResponseIdName Receive { get; set; }
        public string Message1 { get; set; }
        public string Status { get; set; }


    }
}
