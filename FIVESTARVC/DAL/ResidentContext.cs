using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using FIVESTARVC.Models;

namespace FIVESTARVC.DAL
{
    public class ResidentContext : DbContext
    {
        public ResidentContext() : base("ResidentContext")
        {

        }

        public DbSet<Person> People { get; set; }
        public DbSet<Resident> Residents { get; set; }
        public DbSet<MilitaryCampaign> MilitaryCampaigns { get; set; }
        public DbSet<ProgramEvent> ProgramEvents { get; set; }
        public DbSet<ProgramType> ProgramTypes { get; set; }
        public DbSet<Benefit> Benefits { get; set; }
        public DbSet<StateTerritory> States { get; set; }

        public DbSet<Referral> Referrals { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Configurations.Add(new Person.ModelConfiguration());
            modelBuilder.Configurations.Add(new ProgramEvent.ModelConfiguration());

            modelBuilder.Entity<MilitaryCampaign>()
            .HasMany(r => r.Residents).WithMany(m => m.MilitaryCampaigns)
            .Map(t => t.MapLeftKey("MilitaryCampaignID")
            .MapRightKey("ResidentID")
            .ToTable("CampaignAssignment"));
                                  
        }
    }
}