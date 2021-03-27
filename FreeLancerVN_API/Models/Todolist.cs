using System;
using System.Collections.Generic;

#nullable disable

namespace FreeLancerVN_API.Models
{
    public partial class Todolist
    {
        public int JobId { get; set; }
        public string Todo { get; set; }
        public string Status { get; set; }

        public virtual Job Job { get; set; }
    }
}
