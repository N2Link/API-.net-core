using System;
using System.Collections.Generic;

#nullable disable

namespace Api.Models
{
    public partial class OfferHistory
    {
        public int JobId { get; set; }
        public int FreelancerId { get; set; }
        public int OfferPrice { get; set; }
        public int ExpectedTimeId { get; set; }
        public string Description { get; set; }
        public string TodoList { get; set; }
        public string Status { get; set; }

        public virtual Account Freelancer { get; set; }
        public virtual Job Job { get; set; }
    }
}
