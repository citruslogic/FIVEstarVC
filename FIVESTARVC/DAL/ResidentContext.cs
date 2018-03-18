using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
        public DbSet<Room> Rooms { get; set; }
        public DbSet<StateTerritory> States { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<MilitaryCampaign>()
            .HasMany(r => r.Residents).WithMany(m => m.MilitaryCampaigns)
            .Map(t => t.MapLeftKey("MilitaryCampaignID")
            .MapRightKey("ResidentID")
            .ToTable("CampaignAssignment"));
        }
    }
}