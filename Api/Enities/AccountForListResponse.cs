using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Enities
{
    public class AccountForListResponse
    {
        public AccountForListResponse(Account account)
        {
            Id = account.Id;
            Name = account.Name;
            Title = account.Tile;
            AvatarUrl = account.AvatarUrl;

            Level = account.LevelId == null ? null : new ResponseIdName(account.Level);

            Specialty = account.SpecialtyId == null ? null
                : new ResponseIdName(account.Specialty);

            this.TotalRatingModel = new TotalRatingModel(account.Ratings.ToList());

        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string AvatarUrl { get; set; }

        public virtual ResponseIdName Level { get; set; }
        public virtual ResponseIdName Specialty { get; set; }
        public TotalRatingModel TotalRatingModel { get; set; }
    }
}
