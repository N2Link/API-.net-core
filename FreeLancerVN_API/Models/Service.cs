using System;
using System.Collections.Generic;

#nullable disable

namespace FreeLancerVN_API.Models
{
    public partial class Service
    {
        public Service()
        {
            FreelancerServices = new HashSet<FreelancerService>();
            Jobs = new HashSet<Job>();
            ProfileServices = new HashSet<ProfileService>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int? SpecialtyId { get; set; }

        public virtual Specialty Specialty { get; set; }
        public virtual ICollection<FreelancerService> FreelancerServices { get; set; }
        public virtual ICollection<Job> Jobs { get; set; }
        public virtual ICollection<ProfileService> ProfileServices { get; set; }
    }
}
