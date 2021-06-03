using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Enities
{
    public class AnnouncementPostModel
    {
        public string Title { get; set; }
        public string Detail { get; set; }

        public List<int> AccountIDs { get; set; }
    }
}
