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
            try
            {
                Level = account.Level == null ? null : new ResponseIdName(account.Level);
            }
            catch (Exception){}
            try
            {
                 Specialty = account.Specialty == null ? null
                        : new ResponseIdName(account.Specialty);
            }
            catch (Exception){}

            this.TotalRatingModel = new TotalRatingModel(account.RatingFreelancers.ToList());

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
