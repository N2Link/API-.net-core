using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Enities
{
    public class ProvinceResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ProvinceResponse(Province province)
        {
            this.Id = province.ProvinceId;
            this.Name = province.Name;
        }
    }
}
