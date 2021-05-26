using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            AvatarRenter = job.Renter.AvatarUrl;
            Cellingprice = job.Cellingprice;
            Details = job.Details;
            Floorprice = job.Floorprice;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Deadline { get; set; }
        public string Details { get; set; }
        public string AvatarRenter { get; set; }
        public int Floorprice { get; set; }
        public int Cellingprice { get; set; }
    }
}
