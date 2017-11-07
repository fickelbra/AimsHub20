namespace AimsHub.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class PatientLogModel : DbContext
    {
        public PatientLogModel()
            : base("name=PatientLogModel")
        {
        }

        public virtual DbSet<Billing> Billings { get; set; }
        public virtual DbSet<PatientLog> PatientLogs { get; set; }
        public virtual DbSet<PatientLogTmp> PatientLogTmps { get; set; }
        public virtual DbSet<PCPCommunication> PCPCommunications { get; set; }
        public virtual DbSet<ReferringPractice> ReferringPractices { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserType> UserTypes { get; set; }
        public virtual DbSet<Audit> Audits { get; set; }
        public virtual DbSet<BillerPreference> BillerPreferences { get; set; }
        public virtual DbSet<Gender> Genders { get; set; }
        public virtual DbSet<Schedule> Schedules { get; set; }
        public virtual DbSet<ScheduleWorkArea> ScheduleWorkAreas { get; set; }
        public virtual DbSet<ScheduleFavorite> ScheduleFavorites { get; set; }
        public virtual DbSet<ScheduleType> ScheduleTypes { get; set; }
        public virtual DbSet<ScheduleRoundingType> ScheduleRoundingTypes { get; set; }
        public virtual DbSet<ServiceType> ServiceTypes { get; set; }
        public virtual DbSet<SchedulePattern> SchedulePatterns { get; set; }
        public virtual DbSet<Hospital> Hospitals { get; set; }
        public virtual DbSet<PatientClass> PatientClasses { get; set; }
        public virtual DbSet<UserDetail> UserDetails { get; set; }

        public virtual DbSet<FaxServiceType> FaxServiceTypes { get; set; }
        public virtual DbSet<FileImportColumn> FileImportColumns { get; set; }

        public virtual DbSet<BillingCPTCode> BillingCPTCodes { get; set; }
        public virtual DbSet<BillingMODCode> BillingMODCodes { get; set; }
        public virtual DbSet<BillingPOSCode> BillingPOSCodes { get; set; }
        public virtual DbSet<BillingICD10Code> BillingICD10Codes { get; set; }

        public virtual DbSet<RefPracAdmin> RefPracAdmins { get; set; }
        public virtual DbSet<RefPracUser> RefPracUsers { get; set; }
        public virtual DbSet<RefPracSpecialty> RefPracSpecialties { get; set; }

        public virtual DbSet<MissingNotesLog> MissingNotesLogs { get; set; }

        public virtual DbSet<UserPreference> UserPreferences { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<PatientLogTmp>()
            //    .HasOptional(e => e.PatientLogTmp1)
            //    .WithRequired(e => e.PatientLogTmp2);

            modelBuilder.Entity<ReferringPractice>()
                .Property(e => e.City)
                .IsUnicode(false);

            modelBuilder.Entity<ReferringPractice>()
                .Property(e => e.State)
                .IsUnicode(false);

            modelBuilder.Entity<ReferringPractice>()
                .Property(e => e.Zip)
                .IsUnicode(false);

            modelBuilder.Entity<ReferringPractice>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<ReferringPractice>()
                .Property(e => e.Fax)
                .IsUnicode(false);

            modelBuilder.Entity<ReferringPractice>()
                .Property(e => e.OfficeManager)
                .IsUnicode(false);

            modelBuilder.Entity<Schedule>()
                .Property(e => e.HospitalShortName)
                .IsUnicode(false);

            modelBuilder.Entity<ScheduleWorkArea>()
                .Property(e => e.HospitalShortName)
                .IsUnicode(false);
        }

        public System.Data.Entity.DbSet<AimsHub.Models.AccountModel> AccountModels { get; set; }
    }
}
