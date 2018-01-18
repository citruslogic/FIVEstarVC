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

 /*           var events = new List<Event>
            {
                new Event { ResidentID=1, AdmitDate=DateTime.Parse("2005-09-01"), LeaveDate=DateTime.Parse("2005-09-25")},
                new Event { ResidentID=2, AdmitDate=DateTime.Parse("2005-10-05"), LeaveDate=DateTime.Parse("2005-10-15")},
                new Event { ResidentID=3, AdmitDate=DateTime.Parse("2005-11-19"), LeaveDate = null}
            };

            events.ForEach(e => context.Events.Add(e));
            context.SaveChanges();*/

            var militaryCampaigns = new List<MilitaryCampaign>
            {
                new MilitaryCampaign { CampaignName="Gulf War", Residents = new List<Resident>() },
                new MilitaryCampaign { CampaignName="Afghanistan War", Residents = new List<Resident>() },
                new MilitaryCampaign { CampaignName="Vietnam War", Residents = new List<Resident>() }
            };

            militaryCampaigns.ForEach(m => context.MilitaryCampaigns.Add(m));
            context.SaveChanges();
        }
    }
}