using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Enities
{
    public class JobRequestDetail : JobForListResponse
    {
        public JobRequestDetail(Job job) : base(job)
        {
            Messages = job.Messages.Where(p => p.JobId == job.Id && p.FreelancerId == job.FreelancerId)
                .Select(p => new MessageResponse(p)).ToList();
        }
        public List<MessageResponse> Messages { get; set; }
    }
}
