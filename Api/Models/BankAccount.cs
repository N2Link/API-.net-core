using System;
using System.Collections.Generic;

#nullable disable

namespace Api.Models
{
    public partial class BankAccount
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int BankId { get; set; }
        public string OwnerName { get; set; }
        public string AccountNumber { get; set; }
        public string BranchName { get; set; }

        public virtual Account Account { get; set; }
        public virtual Bank Bank { get; set; }
    }
}
