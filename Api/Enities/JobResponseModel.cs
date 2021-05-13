using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Service;

namespace Api.Enities
{
    public class JobResponseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Deadline { get; set; }
        public string Details { get; set; }
        public IUserService.UserEntitis Renter { get; set; }
        public IUserService.UserEntitis Freelancer { get; set; }
        public ICollection<Skill> Skills { get; set; }
        public SpecialtyService SpecialtyService { get; set; }
        public TypeOfWork TypeOfWork { get; set; }
        public FormOfWork FormOfWork { get; set; }
        public Province Province { get; set; }
        public int Floorprice { get; set; }
        public int Cellingprice { get; set; }
        public Payform Payform { get; set; }


    }
}
