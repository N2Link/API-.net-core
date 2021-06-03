using System;
using System.Collections.Generic;

#nullable disable

namespace Api.Models
{
    public partial class Bank
    {
        public Bank()
        {
            BankAccounts = new HashSet<BankAccount>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<BankAccount> BankAccounts { get; set; }
    }
}
