using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Enities
{
    public class ImageCPEdit
    {
        public  int CPID { get; set; }
        public string ImageName { get; set; }
        public string ImageBase64 { get; set; }
    }
}
