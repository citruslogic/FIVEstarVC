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

        public DbSet<Resident> Residents { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<ResidentCampaign> ResidentCampaigns { get; set; }
        public DbSet<MilitaryCampaign> MilitaryCampaigns { get; set; }
        public DbSet<Program> Programs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}