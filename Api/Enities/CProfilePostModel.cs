using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Enities
{
    public class CProfilePostModel
    {
        public string Name { get; set; }
        public string Urlweb { get; set; }
        public string Description { get; set; }
        public string ImageName { get; set; }
        public string ImageBase64 { get; set; }
    }
}
