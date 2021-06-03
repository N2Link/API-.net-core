using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Enities
{
    public class MessageModel
    {
        public int JobId { get; set; }
        public int SenderId { get; set; }
        public int ReceiveId { get; set; }
        public string Message1 { get; set; }
        public string Status { get; set; }
    }
}
