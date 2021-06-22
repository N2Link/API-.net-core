using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Enities
{
    public class MessageUserResponse
    {
        public ResponseIdName Job { get; set; }
        public ResponseIdName Freelancer { get; set; }
        public AccountForListResponse ToUser { get; set; }
        public ResponseIdName LastSender { get; set; }
        public string LastMessage { get; set; }
        public DateTime Time { get; set; }
        public string LastMsgStatus { get; set; }
        public string Status { get; set; }
        public int UnseenCount { get; set; }
    }
}
