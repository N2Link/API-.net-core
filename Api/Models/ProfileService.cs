﻿using System;
using System.Collections.Generic;

#nullable disable

namespace Api.Models
{
    public partial class ProfileService
    {
        public int FreelancerId { get; set; }
        public int ServiceId { get; set; }
        public string Name { get; set; }

        public virtual CapacityProfile CapacityProfile { get; set; }
        public virtual Service Service { get; set; }
    }
}
