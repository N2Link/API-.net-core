using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Enities
{
    public class AccountEditModel
    {
        public string Name { get; set; }
        public int RoleId { get; set; }
        public string Phone { get; set; }
        public string Tile { get; set; }
        public string Description { get; set; }
        public string Website { get; set; }
        public int SpecialtyId { get; set; }
        public int LevelId { get; set; }
        public string ProvinceID { get; set; }
        public bool OnReady { get; set; }
        public List<ResponseIdName> Skills { get; set; }
        public List<ResponseIdName> Services { get; set; }
    }
}
