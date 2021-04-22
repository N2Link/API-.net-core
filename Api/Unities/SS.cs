using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Unities
{
    public class SS
    {
        public int SpecialtyID { get; set; }
        public String SpecialtyName { get; set; }
        public int ServiceID { get; set; }
        public String ServiceName { get; set; }
        public SS(SpecialtyService specialtyService)
        {
            this.SpecialtyID = specialtyService.SpecialtyId;
            this.SpecialtyName = specialtyService.Specialty.Name;
            this.ServiceID = specialtyService.ServiceId;
            this.ServiceName = specialtyService.Service.Name;

        }
        public SS() { }


    }
}
