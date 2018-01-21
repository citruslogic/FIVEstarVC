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
                new Resident { FirstMidName="Carson", LastName="Steven", ServiceBranch=ServiceType.ARMY, Room=102},
                new Resident { FirstMidName="Naomi", LastName="Wildman", ServiceBranch=ServiceType.AIRFORCE, Room=103 },
                new Resident { FirstMidName="Gary", LastName="Noonan", ServiceBranch=ServiceType.NAVY, Room =105 }
            };

            residents.ForEach(r => context.Residents.Add(r));
            context.SaveChanges();



            var programs = new List<ProgramType>
            {
                new ProgramType { ResidentProgramType=ResidentProgramType.WORK_PROGRAM, ProgramDescription="Home construction project" }
            };

            programs.ForEach(p => context.ProgramTypes.Add(p));
            context.SaveChanges();


            var events = new List<ProgramEvent>
            {
                new ProgramEvent { StartDate=DateTime.Parse("2005-09-01"), EndDate=DateTime.Parse("2005-09-25"), Completed=true, ResidentID=1, ProgramTypeID=1},
                new ProgramEvent { StartDate=DateTime.Parse("2005-10-05"), EndDate=DateTime.Parse("2005-10-15"), ResidentID=2},
                new ProgramEvent { StartDate=DateTime.Parse("2005-11-19"), EndDate = null, ResidentID=3}
            };

            events.ForEach(e => context.ProgramEvents.Add(e));
            context.SaveChanges();

 
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

            var Rooms = new List<Room>
            {
                //Rooms on the E/S Wing//

               new Room { RoomNum=102},
               new Room { RoomNum=103},
               new Room { RoomNum=105},
               new Room { RoomNum=106},
               new Room { RoomNum=107},
               new Room { RoomNum=108},
               new Room { RoomNum=109},
               new Room { RoomNum=110},
               new Room { RoomNum=112},
               new Room { RoomNum=114},
               new Room { RoomNum=115},
               new Room { RoomNum=116},
               new Room { RoomNum=117},
               new Room { RoomNum=118},
               new Room { RoomNum=119},
               new Room { RoomNum=120},
               new Room { RoomNum=121},
               new Room { RoomNum=122},
               new Room { RoomNum=123},
               new Room { RoomNum=124},
               new Room { RoomNum=125},
               new Room { RoomNum=126},

               //Rooms on the West Wing//

               new Room { RoomNum=202},
               new Room { RoomNum=203},
               new Room { RoomNum=204},
               new Room { RoomNum=205},
               new Room { RoomNum=206},
               new Room { RoomNum=207},
               new Room { RoomNum=208},
               new Room { RoomNum=209},

               //Rooms on the North Wing//

               new Room { RoomNum=301},
               new Room { RoomNum=303},
               new Room { RoomNum=304},
               new Room { RoomNum=305},
               new Room { RoomNum=306},
               new Room { RoomNum=307},
               new Room { RoomNum=308},
               new Room { RoomNum=310},

            };

            Rooms.ForEach(m => context.Rooms.Add(m));
            context.SaveChanges();

        }
    }
}