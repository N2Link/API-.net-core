using System;
using System.Collections.Generic;

#nullable disable

namespace FreeLancerVN_API.Models
{
    public partial class FreelancerSkill
    {
        public int FreelancerId { get; set; }
        public int SkilId { get; set; }

        public virtual Account Freelancer { get; set; }
        public virtual Skill Skil { get; set; }
    }
}
