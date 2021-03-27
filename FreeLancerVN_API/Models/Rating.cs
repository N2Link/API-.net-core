using System;
using System.Collections.Generic;

#nullable disable

namespace FreeLancerVN_API.Models
{
    public partial class Rating
    {
        public int JobId { get; set; }
        public int? FreelancerId { get; set; }
        public int? Quality { get; set; }
        public int? Level { get; set; }
        public int? Price { get; set; }
        public int? Time { get; set; }
        public int? Profession { get; set; }

        public virtual Account Freelancer { get; set; }
        public virtual Job Job { get; set; }
    }
}
