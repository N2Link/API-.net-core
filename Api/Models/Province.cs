using System;
using System.Collections.Generic;

#nullable disable

namespace Api.Models
{
    public partial class Province
    {
        public Province()
        {
            Accounts = new HashSet<Account>();
            Jobs = new HashSet<Job>();
        }

        public string ProvinceId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<Job> Jobs { get; set; }
    }
}
