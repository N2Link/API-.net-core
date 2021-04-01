using System;
using System.Collections.Generic;

#nullable disable

namespace Api.Models
{
    public partial class CapacityProfile
    {
        public CapacityProfile()
        {
            Images = new HashSet<Image>();
            ProfileServices = new HashSet<ProfileService>();
        }

        public int FreelancerId { get; set; }
        public string Name { get; set; }
        public string Urlweb { get; set; }
        public string Description { get; set; }

        public virtual Account Freelancer { get; set; }
        public virtual ICollection<Image> Images { get; set; }
        public virtual ICollection<ProfileService> ProfileServices { get; set; }
    }
}
