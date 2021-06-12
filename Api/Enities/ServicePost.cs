using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Enities
{
    public class ServicePost
    {
        public string Name { get; set; }
        public List<ResponseIdName> Specialties { get; set; }
    }
}
