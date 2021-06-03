using System;
using System.Collections.Generic;

#nullable disable

namespace Api.Models
{
    public partial class Specialty
    {
        public Specialty()
        {
            Accounts = new HashSet<Account>();
            SpecialtyServices = new HashSet<SpecialtyService>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<SpecialtyService> SpecialtyServices { get; set; }
    }
}
