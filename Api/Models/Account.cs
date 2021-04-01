using System;
using System.Collections.Generic;

#nullable disable

namespace Api.Models
{
    public partial class Account
    {
        public Account()
        {
            CapacityProfiles = new HashSet<CapacityProfile>();
            FreelancerServices = new HashSet<FreelancerService>();
            FreelancerSkills = new HashSet<FreelancerSkill>();
            JobFreelancers = new HashSet<Job>();
            JobRenters = new HashSet<Job>();
            OfferHistories = new HashSet<OfferHistory>();
            Ratings = new HashSet<Rating>();
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Firstname { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public string Tile { get; set; }
        public string Description { get; set; }
        public string Website { get; set; }
        public int Balance { get; set; }
        public bool IsAccuracy { get; set; }
        public int? Speccializeid { get; set; }
        public int? LevelId { get; set; }
        public bool? OnReady { get; set; }
        public int? FormOnWorkId { get; set; }
        public string AvatarUrl { get; set; }

        public virtual FormOfWork FormOnWork { get; set; }
        public virtual Level Level { get; set; }
        public virtual Role Role { get; set; }
        public virtual ICollection<CapacityProfile> CapacityProfiles { get; set; }
        public virtual ICollection<FreelancerService> FreelancerServices { get; set; }
        public virtual ICollection<FreelancerSkill> FreelancerSkills { get; set; }
        public virtual ICollection<Job> JobFreelancers { get; set; }
        public virtual ICollection<Job> JobRenters { get; set; }
        public virtual ICollection<OfferHistory> OfferHistories { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
    }
}
