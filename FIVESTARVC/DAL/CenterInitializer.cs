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

            var Rooms = new List<Room>
            {
                //Rooms on the E/S Wing//

               new Room { RoomNum=102, IsOccupied = true, WingName = "EastSouth"},
               new Room { RoomNum=103, IsOccupied = true, WingName = "EastSouth"},
               new Room { RoomNum=105, IsOccupied = true, WingName = "EastSouth"},
               new Room { RoomNum=106, IsOccupied = true, WingName = "EastSouth"},
               new Room { RoomNum=107, IsOccupied = true, WingName = "EastSouth"},
               new Room { RoomNum=108, IsOccupied = true, WingName = "EastSouth"},
               new Room { RoomNum=109, IsOccupied = false, WingName = "EastSouth"},
               new Room { RoomNum=110, IsOccupied = false, WingName = "EastSouth"},
               new Room { RoomNum=112, IsOccupied = false, WingName = "EastSouth"},
               new Room { RoomNum=114, IsOccupied = false, WingName = "EastSouth"},
               new Room { RoomNum=115, IsOccupied = false, WingName = "EastSouth"},
               new Room { RoomNum=116, IsOccupied = false, WingName = "EastSouth"},
               new Room { RoomNum=117, IsOccupied = false, WingName = "EastSouth"},
               new Room { RoomNum=118, IsOccupied = false, WingName = "EastSouth"},
               new Room { RoomNum=119, IsOccupied = false, WingName = "EastSouth"},
               new Room { RoomNum=120, IsOccupied = false, WingName = "EastSouth"},
               new Room { RoomNum=121, IsOccupied = false, WingName = "EastSouth"},
               new Room { RoomNum=122, IsOccupied = false, WingName = "EastSouth"},
               new Room { RoomNum=123, IsOccupied = false, WingName = "EastSouth"},
               new Room { RoomNum=124, IsOccupied = false, WingName = "EastSouth"},
               new Room { RoomNum=125, IsOccupied = false, WingName = "EastSouth"},
               new Room { RoomNum=126, IsOccupied = false, WingName = "EastSouth"},

               //Rooms on the West Wing//

               new Room { RoomNum=202, IsOccupied = false, WingName = "West"},
               new Room { RoomNum=203, IsOccupied = false, WingName = "West"},
               new Room { RoomNum=204, IsOccupied = false, WingName = "West"},
               new Room { RoomNum=205, IsOccupied = false, WingName = "West"},
               new Room { RoomNum=206, IsOccupied = false, WingName = "West"},
               new Room { RoomNum=207, IsOccupied = false, WingName = "West"},
               new Room { RoomNum=208, IsOccupied = false, WingName = "West"},
               new Room { RoomNum=209, IsOccupied = false, WingName = "West"},

               //Rooms on the North Wing//

               new Room { RoomNum=301, IsOccupied = false, WingName = "North"},
               new Room { RoomNum=303, IsOccupied = false, WingName = "North"},
               new Room { RoomNum=304, IsOccupied = false, WingName = "North"},
               new Room { RoomNum=305, IsOccupied = false, WingName = "North"},
               new Room { RoomNum=306, IsOccupied = false, WingName = "North"},
               new Room { RoomNum=307, IsOccupied = false, WingName = "North"},
               new Room { RoomNum=308, IsOccupied = false, WingName = "North"},
               new Room { RoomNum=310, IsOccupied = false, WingName = "North"},

            };

            Rooms.ForEach(m => context.Rooms.Add(m));
            context.SaveChanges();

            var residents = new List<Resident>
            {
                new Resident { FirstMidName="Carson", Birthdate=DateTime.Parse("3/4/1988"), LastName="Steven", RoomID = 1, ServiceBranch=ServiceType.ARMY },
                new Resident { FirstMidName="Naomi", Birthdate=DateTime.Parse("6/12/1979"), LastName="Wildman", RoomID = 2, ServiceBranch=ServiceType.AIRFORCE },
                new Resident { FirstMidName="Gary", Birthdate=DateTime.Parse("6/24/1980"), LastName="Noonan", RoomID = 3, ServiceBranch=ServiceType.NAVY },
                new Resident { FirstMidName="Steve", Birthdate=DateTime.Parse("1/15/1981"), LastName="Nash", RoomID = 4, ServiceBranch=ServiceType.MARINES },
                new Resident { FirstMidName="Neo", Birthdate=DateTime.Parse("3/8/1974"), LastName="Anderson", RoomID = 5, ServiceBranch=ServiceType.AIRFORCE },
                new Resident { FirstMidName="Charlie", LastName="Brown", Birthdate=DateTime.Parse("12/23/1976"), RoomID = 6, ServiceBranch=ServiceType.NAVY }
            };

            residents.ForEach(r => context.Residents.Add(r));
            context.SaveChanges();



            var programs = new List<ProgramType>
            {
                // RESIDENT ADMISSION TYPES
                new ProgramType { ProgramDescription="Emergency Shelter" },                         // 1
                new ProgramType { ProgramDescription="Resident Admission"},                         // 2
                new ProgramType { ProgramDescription="Re-admit"},                                   // 3

                // RESIDENT DISCHARGE TYPES
                new ProgramType { ProgramDescription="Resident Graduation" },                       // 4
                new ProgramType { ProgramDescription="Self Discharge"},                             // 5
                new ProgramType { ProgramDescription="Discharge for Cause"},                        // 6
                new ProgramType { ProgramDescription="Higher Level of Care"},                       // 7

                // ENROLLED PROGRAMS
                new ProgramType { ProgramDescription="Work Program" },                              // 8
                new ProgramType { ProgramDescription="Mental Wellness" },                           // 9
                new ProgramType { ProgramDescription="P2I" },                                       // 10
                new ProgramType { ProgramDescription="School Program" },                            // 11
                new ProgramType { ProgramDescription="Financial Program"},                          // 12
                new ProgramType { ProgramDescription="Depression / Behavioral Program"},            // 13
                new ProgramType { ProgramDescription="Substance Abuse Program"}                     // 14

            };

            programs.ForEach(p => context.ProgramTypes.Add(p));
            context.SaveChanges();


            var events = new List<ProgramEvent>
            {
                //adding admission event for all residents, the admission doesn't "end."
                new ProgramEvent { StartDate=DateTime.Today, EndDate=null, Completed=true, ResidentID=1, ProgramTypeID=2},
                new ProgramEvent { StartDate=DateTime.Today, EndDate=null, Completed=true, ResidentID=2, ProgramTypeID=2},
                new ProgramEvent { StartDate=DateTime.Today, EndDate=null, Completed=true, ResidentID=3, ProgramTypeID=2},
                new ProgramEvent { StartDate=DateTime.Today, EndDate=null, Completed=true, ResidentID=4, ProgramTypeID=2},
                new ProgramEvent { StartDate=DateTime.Today, EndDate=null, Completed=true, ResidentID=5, ProgramTypeID=2},
                new ProgramEvent { StartDate=DateTime.Today, EndDate=null, Completed=true, ResidentID=6, ProgramTypeID=2},
                //Other events
                new ProgramEvent { StartDate=DateTime.Parse("2005-09-01"), EndDate=DateTime.Parse("2005-09-25"), Completed=true, ResidentID=1, ProgramTypeID=3},
                new ProgramEvent { StartDate=DateTime.Parse("2005-04-02"), EndDate=DateTime.Parse("2006-06-05"), Completed=true, ResidentID=4, ProgramTypeID=10},
                new ProgramEvent { StartDate=DateTime.Parse("2004-05-05"), EndDate=DateTime.Parse("2007-06-20"), Completed=true, ResidentID=5, ProgramTypeID=8},
                new ProgramEvent { StartDate=DateTime.Parse("2003-03-02"), EndDate=DateTime.Parse("2007-07-12"), Completed=true, ResidentID=6, ProgramTypeID=8},
                new ProgramEvent { StartDate=DateTime.Parse("2006-03-22"), EndDate=DateTime.Parse("2008-07-12"), Completed=true, ResidentID=1, ProgramTypeID=13},
                new ProgramEvent { StartDate=DateTime.Parse("2006-03-22"), EndDate=DateTime.Parse("2008-07-12"), Completed=true, ResidentID=2, ProgramTypeID=8},
                new ProgramEvent { StartDate=DateTime.Parse("2005-10-05"), EndDate=DateTime.Parse("2005-10-15"), ResidentID=2},
                new ProgramEvent { StartDate=DateTime.Parse("2005-11-19"), EndDate = null, ResidentID=3}
            };

            events.ForEach(e => context.ProgramEvents.Add(e));
            context.SaveChanges();


            var militaryCampaigns = new List<MilitaryCampaign>
            {
                new MilitaryCampaign { CampaignName="Non-combat", Residents = new List<Resident>() },
                new MilitaryCampaign { CampaignName="Persian Gulf", Residents = new List<Resident>() },
                new MilitaryCampaign { CampaignName="Afghanistan/OEF", Residents = new List<Resident>() },
                new MilitaryCampaign { CampaignName="Vietnam", Residents = new List<Resident>() },
                new MilitaryCampaign { CampaignName="Iraq/OIF", Residents = new List<Resident>() },
                new MilitaryCampaign { CampaignName="Bosnia", Residents = new List<Resident>() }
            };

            militaryCampaigns.ForEach(m => context.MilitaryCampaigns.Add(m));
            context.SaveChanges();

         
        }
    }
}