using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Enities
{
    public class RatingPost
    {
        public int JobId { get; set; }
        public int FreelancerId { get; set; }
        public int Quality { get; set; }
        public int Level { get; set; }
        public int Price { get; set; }
        public int Time { get; set; }
        public int Profession { get; set; }
        public string Comment { get; set; }
    }
}
