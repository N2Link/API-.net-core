using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;

namespace Api.Enities
{
    public class RatingResponse
    {
        public RatingResponse(Rating rating)
        {
            Id = rating.Id;
            Star = rating.Star;
            Comment = rating.Comment;
            Freelancer = new ResponseIdName(rating.Freelancer);
            Renter = new ResponseIdName(rating.Renter);
            Job = new ResponseIdName(rating.Job);
        }
        public int Id { get; set; }
        public int Star { get; set; }
        public string Comment { get; set; }

        public ResponseIdName Freelancer { get; set; }
        public ResponseIdName Job { get; set; }
        public ResponseIdName Renter { get; set; }
    }
}
