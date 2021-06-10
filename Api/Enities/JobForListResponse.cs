using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Enities;

namespace Api.Enities
{
    public class JobForListResponse
    {
        public JobForListResponse() { }
        public JobForListResponse(Job job)
        {
            Id = job.Id;
            Name = job.Name;
            Deadline = job.Deadline;
            CreatAt = job.CreateAt;
            Cellingprice = job.Cellingprice;
            Details = job.Details;
            Floorprice = job.Floorprice;
            Status = job.Status;
            Freelancer = job.Freelancer==null?null : new ResponseIdName(job.Freelancer);
            Renter = job.Renter==null?null : new ResponseIdName(job.Renter);
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime CreatAt { get; set; }
        public string Details { get; set; }
        public int Floorprice { get; set; }
        public int Cellingprice { get; set; }
        public string Status{ get; set; }
        public ResponseIdName Freelancer { get; set; }
        public ResponseIdName Renter { get; set; }
    }
}
