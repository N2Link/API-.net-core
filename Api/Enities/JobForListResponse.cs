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
            Floorprice = job.Floorprice;
            Status = job.Status;
            Specialty = new ResponseIdName(job.S.Specialty);
            Freelancer = job.Freelancer==null?null : new ResponseIdName(job.Freelancer);
            Renter = job.Renter==null?null : new ResponseIdName(job.Renter);
            AvatarUrl = job.Renter == null ? null : job.Renter.AvatarUrl;
            BidCount = job.OfferHistories.Count();
            Price = job.Price;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime CreatAt { get; set; }
        public int Floorprice { get; set; }
        public int Cellingprice { get; set; }
        public string Status{ get; set; }
        public string AvatarUrl { get; set; }
        public int BidCount { get; set; }
        public int Price { get; set; }
        public ResponseIdName Specialty { get; set; }
        public ResponseIdName Freelancer { get; set; }
        public ResponseIdName Renter { get; set; }
    }
}
