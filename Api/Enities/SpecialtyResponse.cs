using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Enities
{
    public class SpecialtyResponse
    {
        public SpecialtyResponse (Specialty specialty)
        {
            this.Id = specialty.Id;
            this.Name = specialty.Name;
            this.Image = specialty.Image;

            this.Services = specialty.SpecialtyServices
                .Where(p=>p.IsActive == true && p.Service.IsActive ==true)
                .Select(p => new ResponseIdName(p.Service)).ToList();
        }

        public int  Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public List<ResponseIdName> Services { get; set; }
    }
}
