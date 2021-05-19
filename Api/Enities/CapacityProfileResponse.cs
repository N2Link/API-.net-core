using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Enities
{
    public class CapacityProfileResponse
    {
        public CapacityProfileResponse(CapacityProfile capacityProfile)
        {
            this.Id = capacityProfile.Id;
            this.FreelancerId = capacityProfile.Id;
            this.Name = capacityProfile.Name;
            this.Urlweb = capacityProfile.Urlweb;
            this.Description = capacityProfile.Description;
            this.ImageUrl = capacityProfile.ImageUrl;
            try
            {
                this.Services = capacityProfile.ProfileServices
                            .Select(p => new ResponseIdName(p.Service)).ToList();
            }
            catch (Exception){}
        }
        public int Id { get; set; }
        public int FreelancerId { get; set; }
        public string Name { get; set; }
        public string Urlweb { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        public virtual List<ResponseIdName> Services { get; set; }
    }
}
