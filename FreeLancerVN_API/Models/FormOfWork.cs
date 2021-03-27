using System;
using System.Collections.Generic;

#nullable disable

namespace FreeLancerVN_API.Models
{
    public partial class FormOfWork
    {
        public FormOfWork()
        {
            Accounts = new HashSet<Account>();
            Jobs = new HashSet<Job>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<Job> Jobs { get; set; }
    }
}
