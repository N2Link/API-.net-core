using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Enities
{
    public class TotalMonth
    {
        public string Month { get; set; }
        public int NewJob { get; set; }
        public int Assigned { get; set; }
        public int Done { get; set; }
        public int Cancelled { get; set; }
        public int Money { get; set; }
    }
}
