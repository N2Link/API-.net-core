using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Enities
{
    public class BankAccountPostModel
    {
        public int AccountId { get; set; }
        public int BankId { get; set; }
        public string OwnerName { get; set; }
        public string AccountNumber { get; set; }
        public string BranchName { get; set; }
    }
}
