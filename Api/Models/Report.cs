using System;
using System.Collections.Generic;

#nullable disable

namespace Api.Models
{
    public partial class Report
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string Detail { get; set; }

        public virtual Account Account { get; set; }
    }
}
