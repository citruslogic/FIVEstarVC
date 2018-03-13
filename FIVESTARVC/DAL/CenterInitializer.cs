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

               new Room { RoomNumber=102, IsOccupied = true, WingName = "EastSouth"},
               new Room { RoomNumber=103, IsOccupied = true, WingName = "EastSouth"},
               new Room { RoomNumber=105, IsOccupied = true, WingName = "EastSouth"},
               new Room { RoomNumber=106, IsOccupied = true, WingName = "EastSouth"},
               new Room { RoomNumber=107, IsOccupied = true, WingName = "EastSouth"},
               new Room { RoomNumber=108, IsOccupied = true, WingName = "EastSouth"},
               new Room { RoomNumber=109, IsOccupied = false, WingName = "EastSouth"},
               new Room { RoomNumber=110, IsOccupied = false, WingName = "EastSouth"},
               new Room { RoomNumber=112, IsOccupied = false, WingName = "EastSouth"},
               new Room { RoomNumber=114, IsOccupied = false, WingName = "EastSouth"},
               new Room { RoomNumber=115, IsOccupied = false, WingName = "EastSouth"},
               new Room { RoomNumber=116, IsOccupied = false, WingName = "EastSouth"},
               new Room { RoomNumber=117, IsOccupied = false, WingName = "EastSouth"},
               new Room { RoomNumber=118, IsOccupied = false, WingName = "EastSouth"},
               new Room { RoomNumber=119, IsOccupied = false, WingName = "EastSouth"},
               new Room { RoomNumber=120, IsOccupied = false, WingName = "EastSouth"},
               new Room { RoomNumber=121, IsOccupied = false, WingName = "EastSouth"},
               new Room { RoomNumber=122, IsOccupied = false, WingName = "EastSouth"},
               new Room { RoomNumber=123, IsOccupied = false, WingName = "EastSouth"},
               new Room { RoomNumber=124, IsOccupied = false, WingName = "EastSouth"},
               new Room { RoomNumber=125, IsOccupied = false, WingName = "EastSouth"},
               new Room { RoomNumber=126, IsOccupied = false, WingName = "EastSouth"},

               //Rooms on the West Wing//

               new Room { RoomNumber=202, IsOccupied = false, WingName = "West"},
               new Room { RoomNumber=203, IsOccupied = false, WingName = "West"},
               new Room { RoomNumber=204, IsOccupied = false, WingName = "West"},
               new Room { RoomNumber=205, IsOccupied = false, WingName = "West"},
               new Room { RoomNumber=206, IsOccupied = false, WingName = "West"},
               new Room { RoomNumber=207, IsOccupied = false, WingName = "West"},
               new Room { RoomNumber=208, IsOccupied = false, WingName = "West"},
               new Room { RoomNumber=209, IsOccupied = false, WingName = "West"},

               //Rooms on the North Wing//

               new Room { RoomNumber=301, IsOccupied = false, WingName = "North"},
               new Room { RoomNumber=303, IsOccupied = false, WingName = "North"},
               new Room { RoomNumber=304, IsOccupied = false, WingName = "North"},
               new Room { RoomNumber=305, IsOccupied = false, WingName = "North"},
               new Room { RoomNumber=306, IsOccupied = false, WingName = "North"},
               new Room { RoomNumber=307, IsOccupied = false, WingName = "North"},
               new Room { RoomNumber=308, IsOccupied = false, WingName = "North"},
               new Room { RoomNumber=310, IsOccupied = false, WingName = "North"},

            };

            Rooms.ForEach(m => context.Rooms.Add(m));
            context.SaveChanges();

            var residents = new List<Resident>
            {
                new Resident { FirstMidName="Carson", Birthdate=DateTime.Parse("3/4/1988"), LastName="Steven", RoomNumber = 102, ServiceBranch=ServiceType.ARMY },
                new Resident { FirstMidName="Naomi", Birthdate=DateTime.Parse("6/12/1979"), LastName="Wildman", RoomNumber = 103, ServiceBranch=ServiceType.AIRFORCE },
                new Resident { FirstMidName="Gary", Birthdate=DateTime.Parse("6/24/1980"), LastName="Noonan", RoomNumber = 105, ServiceBranch=ServiceType.NAVY },
                new Resident { FirstMidName="Steve", Birthdate=DateTime.Parse("1/15/1981"), LastName="Nash", RoomNumber = 106, ServiceBranch=ServiceType.MARINES },
                new Resident { FirstMidName="Neo", Birthdate=DateTime.Parse("3/8/1974"), LastName="Anderson", RoomNumber = 107, ServiceBranch=ServiceType.AIRFORCE },
                new Resident { FirstMidName="Charlie", LastName="Brown", Birthdate=DateTime.Parse("12/23/1976"), RoomNumber = 108, ServiceBranch=ServiceType.NAVY }
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
                new ProgramEvent { StartDate=new DateTime(2012,05,16), EndDate=null, Completed=false, ResidentID=1, ProgramTypeID=2},
                new ProgramEvent { StartDate=new DateTime(2014,06,14), EndDate=null, Completed=false, ResidentID=2, ProgramTypeID=2},
                new ProgramEvent { StartDate=new DateTime(2013,07,4), EndDate=null, Completed=false, ResidentID=3, ProgramTypeID=2},
                new ProgramEvent { StartDate=new DateTime(2012,06,20), EndDate=null, Completed=false, ResidentID=4, ProgramTypeID=2},
                new ProgramEvent { StartDate=new DateTime(2015,07,1), EndDate=null, Completed=false, ResidentID=5, ProgramTypeID=2},
                new ProgramEvent { StartDate=new DateTime(2016,07,13), EndDate=null, Completed=false, ResidentID=6, ProgramTypeID=2},
                //Graduated
                new ProgramEvent { StartDate=new DateTime(2013,05,16), EndDate=null, Completed=false, ResidentID=1, ProgramTypeID=4},
                new ProgramEvent { StartDate=new DateTime(2015,06,14), EndDate=null, Completed=false, ResidentID=2, ProgramTypeID=4},
                new ProgramEvent { StartDate=new DateTime(2014,07,4), EndDate=null, Completed=false, ResidentID=3, ProgramTypeID=4},
                new ProgramEvent { StartDate=new DateTime(2013,06,20), EndDate=null, Completed=false, ResidentID=4, ProgramTypeID=4},
                new ProgramEvent { StartDate=new DateTime(2016,07,1), EndDate=null, Completed=false, ResidentID=5, ProgramTypeID=4},
                new ProgramEvent { StartDate=new DateTime(2017,07,13), EndDate=null, Completed=false, ResidentID=6, ProgramTypeID=4},

                //Other events
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
                new MilitaryCampaign { CampaignName="Noncombat", Residents = new List<Resident>() },
                new MilitaryCampaign { CampaignName="Persian Gulf", Residents = new List<Resident>() },
                new MilitaryCampaign { CampaignName="OEF", Residents = new List<Resident>() },
                new MilitaryCampaign { CampaignName="Vietnam", Residents = new List<Resident>() },
                new MilitaryCampaign { CampaignName="OIF", Residents = new List<Resident>() },
                new MilitaryCampaign { CampaignName="Bosnia", Residents = new List<Resident>() }
            };

            militaryCampaigns.ForEach(m => context.MilitaryCampaigns.Add(m));
            context.SaveChanges();

         
        }
    }
}