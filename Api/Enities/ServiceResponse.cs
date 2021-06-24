using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Enities
{
    public class ServiceResponse
    {
        public ServiceResponse(Api.Models.Service service)
        {
            this.Id = service.Id;
            this.Name = service.Name;
            this.Specialty = service.SpecialtyServices
                .Where(p=>p.IsActive == true && p.Specialty.IsActive == true)
                .Select(p => new ResponseIdName(p.Specialty)).ToList();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ResponseIdName> Specialty { get; set; }
    }
}
