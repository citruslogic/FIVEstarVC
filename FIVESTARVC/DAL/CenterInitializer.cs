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
                new Resident { FirstMidName="Carson", LastName="Steven", ServiceBranch=ServiceType.ARMY},
                new Resident { FirstMidName="Naomi", LastName="Wildman", ServiceBranch=ServiceType.AIRFORCE},
                new Resident { FirstMidName="Gary", LastName="Noonan", ServiceBranch=ServiceType.NAVY}
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

               new Room { RoomNum=102, IsOccupied = true, ResidentID = 1},
               new Room { RoomNum=103, IsOccupied = true, ResidentID = 2},
               new Room { RoomNum=105, IsOccupied = true, ResidentID = 3},
               new Room { RoomNum=106, IsOccupied = false},
               new Room { RoomNum=107, IsOccupied = false},
               new Room { RoomNum=108, IsOccupied = false},
               new Room { RoomNum=109, IsOccupied = false},
               new Room { RoomNum=110, IsOccupied = false},
               new Room { RoomNum=112, IsOccupied = false},
               new Room { RoomNum=114, IsOccupied = false},
               new Room { RoomNum=115, IsOccupied = false},
               new Room { RoomNum=116, IsOccupied = false},
               new Room { RoomNum=117, IsOccupied = false},
               new Room { RoomNum=118, IsOccupied = false},
               new Room { RoomNum=119, IsOccupied = false},
               new Room { RoomNum=120, IsOccupied = false},
               new Room { RoomNum=121, IsOccupied = false},
               new Room { RoomNum=122, IsOccupied = false},
               new Room { RoomNum=123, IsOccupied = false},
               new Room { RoomNum=124, IsOccupied = false},
               new Room { RoomNum=125, IsOccupied = false},
               new Room { RoomNum=126, IsOccupied = false},

               //Rooms on the West Wing//

               new Room { RoomNum=202, IsOccupied = false},
               new Room { RoomNum=203, IsOccupied = false},
               new Room { RoomNum=204, IsOccupied = false},
               new Room { RoomNum=205, IsOccupied = false},
               new Room { RoomNum=206, IsOccupied = false},
               new Room { RoomNum=207, IsOccupied = false},
               new Room { RoomNum=208, IsOccupied = false},
               new Room { RoomNum=209, IsOccupied = false},

               //Rooms on the North Wing//

               new Room { RoomNum=301, IsOccupied = false},
               new Room { RoomNum=303, IsOccupied = false},
               new Room { RoomNum=304, IsOccupied = false},
               new Room { RoomNum=305, IsOccupied = false},
               new Room { RoomNum=306, IsOccupied = false},
               new Room { RoomNum=307, IsOccupied = false},
               new Room { RoomNum=308, IsOccupied = false},
               new Room { RoomNum=310, IsOccupied = false},

            };

            Rooms.ForEach(m => context.Rooms.Add(m));
            context.SaveChanges();

        }
    }
}