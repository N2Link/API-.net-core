using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Api.Models
{
    public partial class FreeLancerVNContext : DbContext
    {
        public FreeLancerVNContext()
        {
        }

        public FreeLancerVNContext(DbContextOptions<FreeLancerVNContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Announcement> Announcements { get; set; }
        public virtual DbSet<AnnouncementAccount> AnnouncementAccounts { get; set; }
        public virtual DbSet<Bank> Banks { get; set; }
        public virtual DbSet<BankAccount> BankAccounts { get; set; }
        public virtual DbSet<CapacityProfile> CapacityProfiles { get; set; }
        public virtual DbSet<FormOfWork> FormOfWorks { get; set; }
        public virtual DbSet<FreelancerService> FreelancerServices { get; set; }
        public virtual DbSet<FreelancerSkill> FreelancerSkills { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<JobSkill> JobSkills { get; set; }
        public virtual DbSet<Level> Levels { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<OfferHistory> OfferHistories { get; set; }
        public virtual DbSet<Payform> Payforms { get; set; }
        public virtual DbSet<ProfileService> ProfileServices { get; set; }
        public virtual DbSet<Province> Provinces { get; set; }
        public virtual DbSet<Rating> Ratings { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<Skill> Skills { get; set; }
        public virtual DbSet<Specialty> Specialties { get; set; }
        public virtual DbSet<SpecialtyService> SpecialtyServices { get; set; }
        public virtual DbSet<TypeOfWork> TypeOfWorks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=.;Database=FreeLancerVN;Trusted_Connection=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AvatarUrl)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.BannedAtDate)
                    .HasColumnType("date")
                    .HasColumnName("Banned_at_Date");

                entity.Property(e => e.CreatedAtDate)
                    .HasColumnType("date")
                    .HasColumnName("Created_at_Date");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LevelId).HasColumnName("LevelID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(1024);

                entity.Property(e => e.PasswordSalt)
                    .IsRequired()
                    .HasMaxLength(1024);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(12);

                entity.Property(e => e.ProvinceId)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("ProvinceID");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.Tile).HasMaxLength(50);

                entity.Property(e => e.Website).HasMaxLength(30);

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.LevelId)
                    .HasConstraintName("FK_User_Level");

                entity.HasOne(d => d.Province)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.ProvinceId)
                    .HasConstraintName("FK_Account_Province");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Role");

                entity.HasOne(d => d.Specialty)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.SpecialtyId)
                    .HasConstraintName("FK_Account_Specialty");
            });

            modelBuilder.Entity<Announcement>(entity =>
            {
                entity.ToTable("Announcement");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Detail)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<AnnouncementAccount>(entity =>
            {
                entity.HasKey(e => new { e.AnnouncementId, e.AccountId });

                entity.ToTable("AnnouncementAccount");

                entity.Property(e => e.AnnouncementId).HasColumnName("AnnouncementID");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.AnnouncementAccounts)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AnnouncementAccount_Account");

                entity.HasOne(d => d.Announcement)
                    .WithMany(p => p.AnnouncementAccounts)
                    .HasForeignKey(d => d.AnnouncementId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AnnouncementAccount_Announcement");
            });

            modelBuilder.Entity<Bank>(entity =>
            {
                entity.ToTable("Bank");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(150);
            });

            modelBuilder.Entity<BankAccount>(entity =>
            {
                entity.ToTable("BankAccount");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.AccountNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.BankId).HasColumnName("BankID");

                entity.Property(e => e.BranchName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.OwnerName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.BankAccounts)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BankAccount_Account");

                entity.HasOne(d => d.Bank)
                    .WithMany(p => p.BankAccounts)
                    .HasForeignKey(d => d.BankId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BankAccount_Bank");
            });

            modelBuilder.Entity<CapacityProfile>(entity =>
            {
                entity.ToTable("CapacityProfile");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.FreelancerId).HasColumnName("FreelancerID");

                entity.Property(e => e.ImageUrl).HasMaxLength(100);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Urlweb).HasMaxLength(100);

                entity.HasOne(d => d.Freelancer)
                    .WithMany(p => p.CapacityProfiles)
                    .HasForeignKey(d => d.FreelancerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CapacityProfile_User");
            });

            modelBuilder.Entity<FormOfWork>(entity =>
            {
                entity.ToTable("FormOfWork");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<FreelancerService>(entity =>
            {
                entity.HasKey(e => new { e.FreelancerId, e.ServiceId });

                entity.ToTable("FreelancerService");

                entity.Property(e => e.FreelancerId).HasColumnName("FreelancerID");

                entity.Property(e => e.ServiceId).HasColumnName("ServiceID");

                entity.HasOne(d => d.Freelancer)
                    .WithMany(p => p.FreelancerServices)
                    .HasForeignKey(d => d.FreelancerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FreelancerService_User");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.FreelancerServices)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FreelancerService_Service");
            });

            modelBuilder.Entity<FreelancerSkill>(entity =>
            {
                entity.HasKey(e => new { e.FreelancerId, e.SkillId });

                entity.ToTable("FreelancerSkill");

                entity.Property(e => e.FreelancerId).HasColumnName("FreelancerID");

                entity.Property(e => e.SkillId).HasColumnName("SkillID");

                entity.HasOne(d => d.Freelancer)
                    .WithMany(p => p.FreelancerSkills)
                    .HasForeignKey(d => d.FreelancerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FreelancerSkill_User");

                entity.HasOne(d => d.Skill)
                    .WithMany(p => p.FreelancerSkills)
                    .HasForeignKey(d => d.SkillId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FreelancerSkill_Skill");
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.ToTable("Job");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("Create_at");

                entity.Property(e => e.Deadline).HasColumnType("date");

                entity.Property(e => e.Details)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.FormId).HasColumnName("FormID");

                entity.Property(e => e.FreelancerId).HasColumnName("FreelancerID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.PayformId).HasColumnName("PayformID");

                entity.Property(e => e.ProvinceId)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("ProvinceID");

                entity.Property(e => e.RenterId).HasColumnName("RenterID");

                entity.Property(e => e.ServiceId).HasColumnName("ServiceID");

                entity.Property(e => e.SpecialtyId).HasColumnName("SpecialtyID");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.TypeId).HasColumnName("TypeID");

                entity.HasOne(d => d.Form)
                    .WithMany(p => p.Jobs)
                    .HasForeignKey(d => d.FormId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Job_FormOfWork");

                entity.HasOne(d => d.Freelancer)
                    .WithMany(p => p.JobFreelancers)
                    .HasForeignKey(d => d.FreelancerId)
                    .HasConstraintName("FK_Job_User1");

                entity.HasOne(d => d.Payform)
                    .WithMany(p => p.Jobs)
                    .HasForeignKey(d => d.PayformId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Job_Payform");

                entity.HasOne(d => d.Province)
                    .WithMany(p => p.Jobs)
                    .HasForeignKey(d => d.ProvinceId)
                    .HasConstraintName("FK_Job_Province");

                entity.HasOne(d => d.Renter)
                    .WithMany(p => p.JobRenters)
                    .HasForeignKey(d => d.RenterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Job_User");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Jobs)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Job_TypeOfWork");

                entity.HasOne(d => d.S)
                    .WithMany(p => p.Jobs)
                    .HasForeignKey(d => new { d.SpecialtyId, d.ServiceId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Job_SpecialtyService");
            });

            modelBuilder.Entity<JobSkill>(entity =>
            {
                entity.HasKey(e => new { e.JobId, e.SkillId });

                entity.ToTable("JobSkill");

                entity.Property(e => e.JobId).HasColumnName("JobID");

                entity.Property(e => e.SkillId).HasColumnName("SkillID");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.JobSkills)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobSkill_Job");

                entity.HasOne(d => d.Skill)
                    .WithMany(p => p.JobSkills)
                    .HasForeignKey(d => d.SkillId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobSkill_Skill");
            });

            modelBuilder.Entity<Level>(entity =>
            {
                entity.ToTable("Level");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("Message");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.JobId).HasColumnName("JobID");

                entity.Property(e => e.Message1)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("Message");

                entity.Property(e => e.ReceiveId).HasColumnName("ReceiveID");

                entity.Property(e => e.SenderId).HasColumnName("SenderID");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.Messages)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_Message_Job");

                entity.HasOne(d => d.Receive)
                    .WithMany(p => p.MessageReceives)
                    .HasForeignKey(d => d.ReceiveId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Message_Account1");

                entity.HasOne(d => d.Sender)
                    .WithMany(p => p.MessageSenders)
                    .HasForeignKey(d => d.SenderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Message_Account");
            });

            modelBuilder.Entity<OfferHistory>(entity =>
            {
                entity.HasKey(e => new { e.JobId, e.FreelancerId });

                entity.ToTable("OfferHistory");

                entity.Property(e => e.JobId).HasColumnName("JobID");

                entity.Property(e => e.FreelancerId).HasColumnName("FreelancerID");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.ExpectedDay)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TodoList)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.Freelancer)
                    .WithMany(p => p.OfferHistories)
                    .HasForeignKey(d => d.FreelancerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OfferHistory_User");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.OfferHistories)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OfferHistory_Job");
            });

            modelBuilder.Entity<Payform>(entity =>
            {
                entity.ToTable("Payform");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<ProfileService>(entity =>
            {
                entity.HasKey(e => new { e.Cpid, e.ServiceId })
                    .HasName("PK_ProfileService_1");

                entity.ToTable("ProfileService");

                entity.Property(e => e.Cpid).HasColumnName("CPID");

                entity.Property(e => e.ServiceId).HasColumnName("ServiceID");

                entity.HasOne(d => d.Cp)
                    .WithMany(p => p.ProfileServices)
                    .HasForeignKey(d => d.Cpid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProfileService_CapacityProfile");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.ProfileServices)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProfileService_Service1");
            });

            modelBuilder.Entity<Province>(entity =>
            {
                entity.ToTable("Province");

                entity.Property(e => e.ProvinceId)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("ProvinceID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<Rating>(entity =>
            {
                entity.ToTable("Rating");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Comment).HasMaxLength(500);

                entity.Property(e => e.FreelancerId).HasColumnName("FreelancerID");

                entity.Property(e => e.JobId).HasColumnName("JobID");

                entity.Property(e => e.RenterId).HasColumnName("RenterID");

                entity.HasOne(d => d.Freelancer)
                    .WithMany(p => p.RatingFreelancers)
                    .HasForeignKey(d => d.FreelancerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Rating_User");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.Ratings)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Rating_Job");

                entity.HasOne(d => d.Renter)
                    .WithMany(p => p.RatingRenters)
                    .HasForeignKey(d => d.RenterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Rating_Account");
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.ToTable("Report");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.Detail)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Report_Account");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.ToTable("Service");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Skill>(entity =>
            {
                entity.ToTable("Skill");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Specialty>(entity =>
            {
                entity.ToTable("Specialty");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Image)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<SpecialtyService>(entity =>
            {
                entity.HasKey(e => new { e.SpecialtyId, e.ServiceId });

                entity.ToTable("SpecialtyService");

                entity.Property(e => e.SpecialtyId).HasColumnName("SpecialtyID");

                entity.Property(e => e.ServiceId).HasColumnName("ServiceID");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.SpecialtyServices)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SpecialtyService_Service");

                entity.HasOne(d => d.Specialty)
                    .WithMany(p => p.SpecialtyServices)
                    .HasForeignKey(d => d.SpecialtyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SpecialtyService_Specialty");
            });

            modelBuilder.Entity<TypeOfWork>(entity =>
            {
                entity.ToTable("TypeOfWork");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
