using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using FIVESTARVC.Models;

namespace FIVESTARVC.DAL
{
    public class CenterInitializer : DropCreateDatabaseIfModelChanges<ResidentContext>
    {
        protected override void Seed(ResidentContext context)
        {
            var residents = new List<Resident>
            {
                new Resident { FirstMidName="Carson", LastName="Steven", ServiceBranch=ServiceType.ARMY, RoomNumber=100 },
                new Resident { FirstMidName="Naomi", LastName="Wildman", ServiceBranch=ServiceType.AIRFORCE, RoomNumber=200 },
                new Resident { FirstMidName="Gary", LastName="Noonan", ServiceBranch=ServiceType.NAVY, RoomNumber=110 }
            };

            residents.ForEach(r => context.Residents.Add(r));
            context.SaveChanges();

            //var events = new List<ProgramEvent>
            //{
            //    new ProgramEvent { StartDate=DateTime.Parse("2005-09-01"), EndDate=DateTime.Parse("2005-09-25")},
            //    new ProgramEvent { StartDate=DateTime.Parse("2005-10-05"), EndDate=DateTime.Parse("2005-10-15")},
            //    new ProgramEvent { StartDate=DateTime.Parse("2005-11-19"), EndDate = null}
            //};

            //events.ForEach(e => context.ProgramEvents.Add(e));
            //context.SaveChanges();

            //var programs = new List<ProgramType>
            //{
            //    new ProgramType { ResidentProgramType=ResidentProgramType.WORK_PROGRAM, ProgramDescription="Home construction project" }
            //};

            //programs.ForEach(p => context.ProgramTypes.Add(p));
            //context.SaveChanges();

            var militaryCampaigns = new List<MilitaryCampaign>
            {
                new MilitaryCampaign { CampaignName="Persian Gulf", Residents = new List<Resident>() },
                new MilitaryCampaign { CampaignName="Afghanistan", Residents = new List<Resident>() },
                new MilitaryCampaign { CampaignName="Vietnam", Residents = new List<Resident>() },
                new MilitaryCampaign { CampaignName="Iraq", Residents = new List<Resident>() },
                new MilitaryCampaign { CampaignName="Bosnia", Residents = new List<Resident>() }
            };

            militaryCampaigns.ForEach(m => context.MilitaryCampaigns.Add(m));
            context.SaveChanges();
        }
    }
}