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
            this.Price = Int32.Parse(ratings.Select(p => p.Price).Average().ToString());
            this.Level = Int32.Parse(ratings.Select(p => p.Level).Average().ToString());
            this.Profession = Int32.Parse(ratings.Select(p => p.Profession).Average().ToString());
            this.Quality = Int32.Parse(ratings.Select(p => p.Quality).Average().ToString());
            this.Time = Int32.Parse(ratings.Select(p => p.Time).Average().ToString());
            this.Avg = (this.Profession + this.Price + this.Level + this.Quality + this.Time) / 5;
            Count = ratings.Count();
        }
        public int Avg { get; set; }
        public int Count { get; set; }
        public int Quality { get; set; }
        public int Level { get; set; }
        public int Price { get; set; }
        public int Time { get; set; }
        public int Profession { get; set; }
    }
}
