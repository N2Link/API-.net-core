using System;
using System.Collections.Generic;

#nullable disable

namespace Api.Models
{
    public partial class Todolist
    {
        public int JobId { get; set; }
        public string Todo { get; set; }
        public string Status { get; set; }

        public virtual Job Job { get; set; }
    }
}
