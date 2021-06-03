using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Enities
{
    public class RatingPost
    {
        public int JobID { get; set; }
        public int FreelancerId { get; set; }
        public int Star { get; set; }
        public string Comment { get; set; }
    }
}
