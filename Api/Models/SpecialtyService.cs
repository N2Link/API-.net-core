using System;
using System.Collections.Generic;

#nullable disable

namespace Api.Models
{
    public partial class SpecialtyService
    {
        public SpecialtyService()
        {
            Jobs = new HashSet<Job>();
        }

        public int SpecialtyId { get; set; }
        public int ServiceId { get; set; }
        public bool IsActive { get; set; }

        public virtual Service Service { get; set; }
        public virtual Specialty Specialty { get; set; }
        public virtual ICollection<Job> Jobs { get; set; }
    }
}
