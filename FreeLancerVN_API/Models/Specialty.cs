using System;
using System.Collections.Generic;

#nullable disable

namespace FreeLancerVN_API.Models
{
    public partial class Specialty
    {
        public Specialty()
        {
            Jobs = new HashSet<Job>();
            Services = new HashSet<Service>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Job> Jobs { get; set; }
        public virtual ICollection<Service> Services { get; set; }
    }
}
