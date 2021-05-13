using System;
using System.Collections.Generic;

#nullable disable

namespace Api.Models
{
    public partial class ProfileService
    {
        public int Cpid { get; set; }
        public int ServiceId { get; set; }

        public virtual CapacityProfile Cp { get; set; }
        public virtual Service Service { get; set; }
    }
}
