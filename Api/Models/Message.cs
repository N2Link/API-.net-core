using System;
using System.Collections.Generic;

#nullable disable

namespace Api.Models
{
    public partial class Message
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int SenderId { get; set; }
        public int ReceiveId { get; set; }
        public string Message1 { get; set; }
        public string Status { get; set; }

        public virtual Job Job { get; set; }
        public virtual Account Receive { get; set; }
        public virtual Account Sender { get; set; }
    }
}
