using System;
using System.Collections.Generic;

#nullable disable

namespace Api.Models
{
    public partial class CapacityProfile
    {
        public CapacityProfile()
        {
            ProfileServices = new HashSet<ProfileService>();
        }

        public int Id { get; set; }
        public int FreelancerId { get; set; }
        public string Name { get; set; }
        public string Urlweb { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        public virtual Account Freelancer { get; set; }
        public virtual ICollection<ProfileService> ProfileServices { get; set; }
    }
}
