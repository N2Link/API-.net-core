using System;
using System.Collections.Generic;

#nullable disable

namespace Api.Models
{
    public partial class Rating
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int RenterId { get; set; }
        public int FreelancerId { get; set; }
        public int Star { get; set; }
        public string Comment { get; set; }

        public virtual Account Freelancer { get; set; }
        public virtual Job Job { get; set; }
        public virtual Account Renter { get; set; }
    }
}
