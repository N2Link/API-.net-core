using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Enities
{
    public class ServicePModel
    {
        public string Name { get; set; }
        public List<ResponseIdName> Services { get; set; }
    }
}
