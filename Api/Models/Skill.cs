using System;
using System.Collections.Generic;

#nullable disable

namespace Api.Models
{
    public partial class Skill
    {
        public Skill()
        {
            FreelancerSkills = new HashSet<FreelancerSkill>();
            JobSkills = new HashSet<JobSkill>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<FreelancerSkill> FreelancerSkills { get; set; }
        public virtual ICollection<JobSkill> JobSkills { get; set; }
    }
}
