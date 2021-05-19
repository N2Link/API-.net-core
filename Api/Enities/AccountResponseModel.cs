using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Enities
{
    public class AccountResponseModel
    {
        public AccountResponseModel(Account account, bool isPrivate)
        {
            Id = account.Id;
            Name = account.Name;
            Phone = account.Phone;
            Email = account.Email;
            Tile = account.Tile;
            Description = account.Description;
            Website = account.Website;
            Balance = account.Balance;
            OnReady = account.OnReady;
            AvatarUrl = account.AvatarUrl;

            Role = account.Role==null? null: new ResponseIdName(account.Role);
            FormOfWork = account.FormOfWorkId ==null?null: new ResponseIdName(account.FormOfWork);
            Level = account.LevelId == null ? null : new ResponseIdName(account.Level);

            Specialty = account.SpecialtyId == null ? null 
                : new ResponseIdName(account.Specialty);

            try
            {
                CapacityProfiles = account.CapacityProfiles.Select(p => new CapacityProfile()
                {
                    Id = p.Id,
                    FreelancerId = p.FreelancerId,
                    ImageUrl = p.ImageUrl,
                    Name = p.Name,
                    Description = p.Description,
                    Urlweb = p.Urlweb,
                }).TakeLast(3).ToList();
            }
            catch (Exception){}
            try
            {
                FreelancerServices = account.FreelancerServices
                    .Select(p => new ResponseIdName(p.Service)).ToList();
            }
            catch (Exception){}    
            try
            {
                FreelancerSkills = account.FreelancerSkills
                    .Select(p => new ResponseIdName(p.Skill)).ToList();
            }
            catch (Exception){}
            try
            {
                this.JobRenters = account.JobRenters
                    .Select(p => new JobResponseModel(p)).ToList();
                this.JobFreelancers = account.JobFreelancers
                    .Select(p => new JobResponseModel(p)).ToList();
            }
            catch (Exception){}

            this.TotalRatingModel = new TotalRatingModel(account.Ratings.ToList());

            if (isPrivate)
            {
                this.Balance = null;
            }
            this.Earning = account.JobFreelancers.Count > 0 ? account.JobFreelancers.Count:0;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Tile { get; set; }
        public string Description { get; set; }
        public string Website { get; set; }
        public int? Balance { get; set; }
        public int Earning { get; set; }
        public bool? OnReady { get; set; }
        public string AvatarUrl { get; set; }

        public virtual ResponseIdName FormOfWork { get; set; }
        public virtual ResponseIdName Level { get; set; }
        public virtual ResponseIdName Role { get; set; }
        public virtual ResponseIdName Specialty { get; set; }
        public TotalRatingModel TotalRatingModel { get; set; }
        public virtual ICollection<ResponseIdName> FreelancerServices { get; set; }
        public virtual ICollection<ResponseIdName> FreelancerSkills { get; set; }
        public virtual ICollection<OfferHistory> OfferHistories { get; set; }
        public virtual ICollection<CapacityProfile> CapacityProfiles { get; set; }
        public virtual ICollection<JobResponseModel> JobFreelancers { get; set; }
        public virtual ICollection<JobResponseModel> JobRenters { get; set; }
    }
}
