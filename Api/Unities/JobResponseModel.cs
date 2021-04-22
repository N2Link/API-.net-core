using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Unities
{
    public class JobResponseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Deadline { get; set; }
        public string Details { get; set; }
        public Account Renter { get; set; }
        public ICollection<JobSkill> JobSkills { get; set; }
        public SS SS { get; set; }
        public int Floorprice { get; set; }
        public int Cellingprice { get; set; }
        public Payform Payform { get; set; }


    }
}
