using System;
using System.Collections.Generic;

#nullable disable

namespace Api.Models
{
    public partial class Account
    {
        public Account()
        {
            AnnouncementAccounts = new HashSet<AnnouncementAccount>();
            BankAccounts = new HashSet<BankAccount>();
            CapacityProfiles = new HashSet<CapacityProfile>();
            FreelancerServices = new HashSet<FreelancerService>();
            FreelancerSkills = new HashSet<FreelancerSkill>();
            JobFreelancers = new HashSet<Job>();
            JobRenters = new HashSet<Job>();
            MessageReceives = new HashSet<Message>();
            MessageSenders = new HashSet<Message>();
            OfferHistories = new HashSet<OfferHistory>();
            RatingFreelancers = new HashSet<Rating>();
            RatingRenters = new HashSet<Rating>();
            Reports = new HashSet<Report>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public string Tile { get; set; }
        public string Description { get; set; }
        public string Website { get; set; }
        public int Balance { get; set; }
        public bool IsAccuracy { get; set; }
        public int? SpecialtyId { get; set; }
        public int? LevelId { get; set; }
        public string ProvinceId { get; set; }
        public bool OnReady { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime CreatedAtDate { get; set; }
        public DateTime? BannedAtDate { get; set; }

        public virtual Level Level { get; set; }
        public virtual Province Province { get; set; }
        public virtual Role Role { get; set; }
        public virtual Specialty Specialty { get; set; }
        public virtual ICollection<AnnouncementAccount> AnnouncementAccounts { get; set; }
        public virtual ICollection<BankAccount> BankAccounts { get; set; }
        public virtual ICollection<CapacityProfile> CapacityProfiles { get; set; }
        public virtual ICollection<FreelancerService> FreelancerServices { get; set; }
        public virtual ICollection<FreelancerSkill> FreelancerSkills { get; set; }
        public virtual ICollection<Job> JobFreelancers { get; set; }
        public virtual ICollection<Job> JobRenters { get; set; }
        public virtual ICollection<Message> MessageReceives { get; set; }
        public virtual ICollection<Message> MessageSenders { get; set; }
        public virtual ICollection<OfferHistory> OfferHistories { get; set; }
        public virtual ICollection<Rating> RatingFreelancers { get; set; }
        public virtual ICollection<Rating> RatingRenters { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
    }
}
