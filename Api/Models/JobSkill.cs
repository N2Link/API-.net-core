using System;
using System.Collections.Generic;

#nullable disable

namespace Api.Models
{
    public partial class JobSkill
    {
        public int JobId { get; set; }
        public int SkillId { get; set; }

        public virtual Job Job { get; set; }
        public virtual Skill Skill { get; set; }
    }
}
