using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Enities
{
    public class TotalRatingModel
    {
        public TotalRatingModel(List<Rating> ratings)
        {
            if(ratings == null || ratings.Count == 0)
            {
                return;
            }
            this.Avg = Int32.Parse(ratings.Average(p => p.Star).ToString());
            Count = ratings.Count();
        }
        public int Avg { get; set; }
        public int Count { get; set; }

    }
}
