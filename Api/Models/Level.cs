using System;
using System.Collections.Generic;

#nullable disable

namespace Api.Models
{
    public partial class Level
    {
        public Level()
        {
            Accounts = new HashSet<Account>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
    }
}
