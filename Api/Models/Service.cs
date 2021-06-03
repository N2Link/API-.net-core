using System;
using System.Collections.Generic;

#nullable disable

namespace Api.Models
{
    public partial class Service
    {
        public Service()
        {
            FreelancerServices = new HashSet<FreelancerService>();
            ProfileServices = new HashSet<ProfileService>();
            SpecialtyServices = new HashSet<SpecialtyService>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<FreelancerService> FreelancerServices { get; set; }
        public virtual ICollection<ProfileService> ProfileServices { get; set; }
        public virtual ICollection<SpecialtyService> SpecialtyServices { get; set; }
    }
}
