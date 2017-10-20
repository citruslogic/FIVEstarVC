namespace FIVEstarVC.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class FiveStarModel : DbContext
    {
        public FiveStarModel()
            : base("name=FiveStarModel")
        {
        }

        public virtual DbSet<Disability> Disabilities { get; set; }
        public virtual DbSet<MilitaryCampaign> MilitaryCampaigns { get; set; }
        public virtual DbSet<MilitaryService> MilitaryServices { get; set; }
        public virtual DbSet<ProgramType> ProgramTypes { get; set; }
        public virtual DbSet<ReasonType> ReasonTypes { get; set; }
        public virtual DbSet<Resident> Residents { get; set; }
        public virtual DbSet<Resident_Disability> Resident_Disability { get; set; }
        public virtual DbSet<Resident_MilitaryService> Resident_MilitaryService { get; set; }
        public virtual DbSet<Resident_ProgramEvent> Resident_ProgramEvent { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Disability>()
                .Property(e => e.Condition)
                .IsUnicode(false);

            modelBuilder.Entity<Disability>()
                .Property(e => e.BenefitAmount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Disability>()
                .HasMany(e => e.Resident_Disability)
                .WithRequired(e => e.Disability)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<MilitaryCampaign>()
                .Property(e => e.Campaign)
                .IsUnicode(false);

            modelBuilder.Entity<MilitaryService>()
                .Property(e => e.ServiceName)
                .IsUnicode(false);

            modelBuilder.Entity<MilitaryService>()
                .Property(e => e.MilitaryServiceID)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<MilitaryService>()
                .HasMany(e => e.Resident_MilitaryService)
                .WithRequired(e => e.MilitaryService)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProgramType>()
                .Property(e => e.ProgramDescription)
                .IsUnicode(false);

            modelBuilder.Entity<ProgramType>()
                .HasMany(e => e.Resident_ProgramEvent)
                .WithRequired(e => e.ProgramType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ReasonType>()
                .Property(e => e.DischargeReason)
                .IsUnicode(false);

            modelBuilder.Entity<Resident>()
                .Property(e => e.LastName)
                .IsUnicode(false);

            modelBuilder.Entity<Resident>()
                .Property(e => e.FirstName)
                .IsUnicode(false);

            modelBuilder.Entity<Resident>()
                .HasMany(e => e.Resident_Disability)
                .WithRequired(e => e.Resident)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Resident>()
                .HasMany(e => e.Resident_ProgramEvent)
                .WithRequired(e => e.Resident)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Resident>()
                .HasMany(e => e.Resident_MilitaryService)
                .WithRequired(e => e.Resident)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Resident>()
                .HasMany(e => e.MilitaryCampaigns)
                .WithMany(e => e.Residents)
                .Map(m => m.ToTable("Resident_MilitaryCampaign").MapLeftKey("ResidentID").MapRightKey("MilitaryCampaignID"));

            modelBuilder.Entity<Resident_MilitaryService>()
                .Property(e => e.ResidentMilitaryServiceID)
                .IsUnicode(false);

            modelBuilder.Entity<Resident_MilitaryService>()
                .Property(e => e.MilitaryServiceID)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Resident_ProgramEvent>()
                .Property(e => e.Notes)
                .IsUnicode(false);

            modelBuilder.Entity<Resident_ProgramEvent>()
                .HasMany(e => e.ReasonTypes)
                .WithMany(e => e.Resident_ProgramEvent)
                .Map(m => m.ToTable("Resident_ProgramEvent_ReasonType").MapLeftKey(new[] { "ProgramEventID", "ProgramTypeID", "ResidentID" }).MapRightKey("ReasonID"));
        }
    }
}
