﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Enities
{
    public class SpecialtyPModel
    {
        public string Name { get; set; }
        public string ImageName { get; set; }
        public string ImageBase64 { get; set; }
        public List<ResponseIdName> Services { get; set; }
    }
}
