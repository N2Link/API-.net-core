using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models
{
    public class OfferHistoryPost
    {
        public int JobId { get; set; }
        public int OfferPrice { get; set; }
        public string ExpectedDay { get; set; }
        public string Description { get; set; }
        public string TodoList { get; set; }
    }
}
