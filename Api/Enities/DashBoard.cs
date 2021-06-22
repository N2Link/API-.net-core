using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Enities
{
    public class DashBoard
    {
        public int TotalJob { get; set; }
        public int TotalAssigned { get; set; }
        public int TotalDone { get; set; }
        public int TotalCancelled { get; set; }

        public int TotalUser { get; set; }
        public int UserConfirmed { get; set; }

        public List<TotalMonth> TotalMonths { get; set; }


    }
}
