﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Enities
{
    public class JobPostModel
    {
        public string Name { get; set; }
        public string Details { get; set; }
        public int TypeId { get; set; }
        public int FormId { get; set; }
        public int WorkatId { get; set; }
        public int PayformId { get; set; }
        public DateTime Deadline { get; set; }
        public int Floorprice { get; set; }
        public int Cellingprice { get; set; }
        public int IsPrivate { get; set; }
        public int SpecialtyId { get; set; }
        public int ServiceId { get; set; }
    }
}
