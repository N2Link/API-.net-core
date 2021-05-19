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
            Specialty = new ResponseIdName(job.S.Specialty);
            Service = new ResponseIdName(job.S.Service);
            Renter = new ResponseIdName( job.RenterId, job.Renter.Name );            
            Freelancer = job.Freelancer!=null? new ResponseIdName( job.Freelancer.Id, job.Freelancer.Name ):null;
            Cellingprice = job.Cellingprice;
            Details = job.Details;
            Floorprice = job.Floorprice;
            Payform = new ResponseIdName(job.Payform);
            FormOfWork = new ResponseIdName(job.Form);
            TypeOfWork = new ResponseIdName(job.Type);
            Province = job.Province !=null? new ProvinceResponse(job.Province):null;
            Status = job.Status;
            Skills = job.JobSkills
            .Select(p => new ResponseIdName(p.Skill)).ToList();
        } 
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Deadline { get; set; }
        public string Details { get; set; }
        public ResponseIdName Renter { get; set; }
        public ResponseIdName Freelancer { get; set; }
        public int Floorprice { get; set; }
        public int Cellingprice { get; set; }
        public ResponseIdName Payform { get; set; }
        public ResponseIdName Specialty { get; set; }
        public ResponseIdName Service { get; set; }
        public ResponseIdName TypeOfWork { get; set; }
        public ResponseIdName FormOfWork { get; set; }
        public ProvinceResponse Province { get; set; }
        public string Status { get; set; }
        public ICollection<ResponseIdName> Skills { get; set; }



    }
}
