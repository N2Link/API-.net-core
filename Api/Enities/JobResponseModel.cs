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
        public JobResponseModel() { }
        public JobResponseModel(Job job)
        {
            Id = job.Id;
            Name = job.Name;
            Deadline = job.Deadline;
            SpecialtyService = new SpecialtyService()
            {
                Service = new Models.Service() { Id = job.S.ServiceId, Name = job.S.Service.Name },
                Specialty = new Specialty() { Id = job.S.SpecialtyId, Name = job.S.Specialty.Name }
            };
            Cellingprice = job.Cellingprice;
            Details = job.Details;
            Floorprice = job.Floorprice;
            Payform = new Payform() { Id = job.Payform.Id, Name = job.Payform.Name };
            TypeOfWork = new TypeOfWork() { Id = job.Type.Id, Name = job.Type.Name };
            FormOfWork = new FormOfWork() { Id = job.Form.Id, Name = job.Type.Name };
            Skills = job.JobSkills
            .Select(p => new Skill() { Id = p.Skill.Id, Name = p.Skill.Name }).ToList();
        } 
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
