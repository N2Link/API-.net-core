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
            Title = account.Tile;
            Description = account.Description;
            Website = account.Website;
            OnReady = account.OnReady;
            AvatarUrl = account.AvatarUrl;
            CreatedAtDate = account.CreatedAtDate;
            BannedAtDate = account.BannedAtDate;

            Role = account.Role == null ? null : new ResponseIdName(account.Role);
            Level = account.LevelId == null ? null : new ResponseIdName(account.Level);

            Specialty = account.SpecialtyId == null ? null
                : new ResponseIdName(account.Specialty);

            try
            {
                CapacityProfiles = account.CapacityProfiles
                    .Select(p => new CapacityProfileResponse(p)).TakeLast(3).OrderByDescending(p => p.Id).ToList();
            }
            catch (Exception)
            {
                this.CapacityProfiles = new List<CapacityProfileResponse>();
            }
            try
            {
                FreelancerServices = account.FreelancerServices
                    .Where(p=>p.Service.IsActive==true)
                    .Select(p => new ResponseIdName(p.Service)).ToList();
            }
            catch (Exception) { }
            try
            {
                FreelancerSkills = account.FreelancerSkills
                    .Where(p=>p.Skill.IsActive)
                    .Select(p => new ResponseIdName(p.Skill)).ToList();
            }
            catch (Exception) { }

            Province = account.ProvinceId!=null? new ProvinceResponse(account.Province) :null;
            this.TotalRatingModel = new TotalRatingModel(account.RatingFreelancers.ToList());

            if (!isPrivate)
            {
                BankAccounts = account.BankAccounts.Select(p => new BankAccount()
                {
                    Bank = new Bank() { Id = p.Bank.Id, Name = p.Bank.Name },
                    OwnerName = p.OwnerName,
                    AccountNumber = p.AccountNumber,
                    BranchName = p.BranchName
                }).ToList();
                Balance = account.Balance;
            }
            this.Earning = account.JobFreelancers.Count > 0 ? account.JobFreelancers.Count : 0;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Website { get; set; }
        public int? Balance { get; set; }
        public int Earning { get; set; }
        public bool? OnReady { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime CreatedAtDate { get; set; }
        public DateTime? BannedAtDate { get; set; }

        public virtual ResponseIdName Level { get; set; }
        public virtual ResponseIdName Role { get; set; }
        public virtual ResponseIdName Specialty { get; set; }
        public TotalRatingModel TotalRatingModel { get; set; }
        public virtual List<BankAccount> BankAccounts { get; set; }
        public ProvinceResponse Province { get; set; }
        public virtual ICollection<ResponseIdName> FreelancerServices { get; set; }
        public virtual ICollection<ResponseIdName> FreelancerSkills { get; set; }
        public virtual ICollection<CapacityProfileResponse> CapacityProfiles { get; set; }
    }
}
