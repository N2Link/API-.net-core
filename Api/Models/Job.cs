using System;
using System.Collections.Generic;

#nullable disable

namespace Api.Models
{
    public partial class Job
    {
        public Job()
        {
            JobSkills = new HashSet<JobSkill>();
            Messages = new HashSet<Message>();
            OfferHistories = new HashSet<OfferHistory>();
            Todolists = new HashSet<Todolist>();
        }

        public int Id { get; set; }
        public int RenterId { get; set; }
        public int? FreelancerId { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public int TypeId { get; set; }
        public int FormId { get; set; }
        public int WorkatId { get; set; }
        public int PayformId { get; set; }
        public DateTime Deadline { get; set; }
        public int Floorprice { get; set; }
        public int Cellingprice { get; set; }
        public int IsPrivate { get; set; }
        public int SpecialtyId { get; set; }
        public int ServiceId { get; set; }

        public virtual FormOfWork Form { get; set; }
        public virtual Account Freelancer { get; set; }
        public virtual Rating IdNavigation { get; set; }
        public virtual Payform Payform { get; set; }
        public virtual Account Renter { get; set; }
        public virtual SpecialtyService SpecialtyService { get; set; }
        public virtual TypeOfWork Type { get; set; }
        public virtual ICollection<JobSkill> JobSkills { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<OfferHistory> OfferHistories { get; set; }
        public virtual ICollection<Todolist> Todolists { get; set; }
    }
}
