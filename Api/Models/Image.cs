using System;
using System.Collections.Generic;

#nullable disable

namespace Api.Models
{
    public partial class Image
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public int FreeLancerId { get; set; }
        public string CprofileName { get; set; }

        public virtual CapacityProfile CapacityProfile { get; set; }
    }
}
