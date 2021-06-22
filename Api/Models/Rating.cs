using System;
using System.Collections.Generic;

#nullable disable

namespace Api.Models
{
    public partial class Rating
    {
        public Rating()
        {
            Jobs = new HashSet<Job>();
        }

        public int RenterId { get; set; }
        public int FreelancerId { get; set; }
        public int Star { get; set; }
        public string Comment { get; set; }
        public int Id { get; set; }

        public virtual Account Freelancer { get; set; }
        public virtual Account Renter { get; set; }
        public virtual ICollection<Job> Jobs { get; set; }
    }
}
