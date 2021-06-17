using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Enities
{
    public class MessageUser
    {
        public ResponseIdName Job { get; set; }
        public ResponseIdName Renter { get; set; }
        public ResponseIdName Freelancer { get; set; }
    }
}
