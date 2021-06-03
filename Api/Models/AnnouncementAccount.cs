using System;
using System.Collections.Generic;

#nullable disable

namespace Api.Models
{
    public partial class AnnouncementAccount
    {
        public int AnnouncementId { get; set; }
        public int AccountId { get; set; }

        public virtual Account Account { get; set; }
        public virtual Announcement Announcement { get; set; }
    }
}
