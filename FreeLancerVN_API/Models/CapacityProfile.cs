using System;
using System.Collections.Generic;

#nullable disable

namespace FreeLancerVN_API.Models
{
    public partial class CapacityProfile
    {
        public CapacityProfile()
        {
            ProfileServices = new HashSet<ProfileService>();
        }

        public int FreelancerId { get; set; }
        public string Name { get; set; }
        public string Urlweb { get; set; }
        public string File { get; set; }
        public string Description { get; set; }

        public virtual Account Freelancer { get; set; }
        public virtual ICollection<ProfileService> ProfileServices { get; set; }
    }
}
