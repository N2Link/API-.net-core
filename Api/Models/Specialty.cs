using System;
using System.Collections.Generic;

#nullable disable

namespace Api.Models
{
    public partial class Specialty
    {
        public Specialty()
        {
            SpecialtyServices = new HashSet<SpecialtyService>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<SpecialtyService> SpecialtyServices { get; set; }
    }
}
