using System;
using System.Collections.Generic;

#nullable disable

namespace Api.Models
{
    public partial class Announcement
    {
        public Announcement()
        {
            AnnouncementAccounts = new HashSet<AnnouncementAccount>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }

        public virtual ICollection<AnnouncementAccount> AnnouncementAccounts { get; set; }
    }
}
