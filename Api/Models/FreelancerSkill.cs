using System;
using System.Collections.Generic;

#nullable disable

namespace Api.Models
{
    public partial class FreelancerSkill
    {
        public int FreelancerId { get; set; }
        public int SkillId { get; set; }

        public virtual Account Freelancer { get; set; }
        public virtual Skill Skill { get; set; }
    }
}
