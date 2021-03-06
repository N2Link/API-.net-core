using System;
using System.Collections.Generic;

#nullable disable

namespace Api.Models
{
    public partial class FreelancerService
    {
        public int FreelancerId { get; set; }
        public int ServiceId { get; set; }

        public virtual Account Freelancer { get; set; }
        public virtual Service Service { get; set; }
    }
}
