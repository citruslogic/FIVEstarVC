namespace FIVESTARVC.Migrations
{
    using FIVESTARVC.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<FIVESTARVC.DAL.ResidentContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(FIVESTARVC.DAL.ResidentContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            var States = new List<StateTerritory>
            {
                new StateTerritory { StateTerritoryID=1, State = "AL", Region = "East South Central" },
                new StateTerritory { StateTerritoryID=2, State = "AK", Region = "Pacific" },
                new StateTerritory { StateTerritoryID=3, State = "AZ", Region = "Mountain" },
                new StateTerritory { StateTerritoryID=4, State = "AR", Region = "West South Central" },
                new StateTerritory { StateTerritoryID=5, State = "CA", Region = "Pacific" },
                new StateTerritory { StateTerritoryID=6, State = "CO", Region = "Mountain" },
                new StateTerritory { StateTerritoryID=7, State = "CT", Region = "New England" },
                new StateTerritory { StateTerritoryID=8, State = "DE", Region = "South Atlantic" },
                new StateTerritory { StateTerritoryID=9, State = "FL", Region = "South Atlantic" },
                new StateTerritory { StateTerritoryID=10, State = "GA", Region = "South Atlantic" },
                new StateTerritory { StateTerritoryID=11, State = "HI", Region = "Pacific" },
                new StateTerritory { StateTerritoryID=12, State = "ID", Region = "Mountain" },
                new StateTerritory { StateTerritoryID=13, State = "IL", Region = "East South Central" },
                new StateTerritory { StateTerritoryID=14, State = "IN", Region = "East North Central" },
                new StateTerritory { StateTerritoryID=15, State = "IA", Region = "West North Central" },
                new StateTerritory { StateTerritoryID=16, State = "KS", Region = "West North Central" },
                new StateTerritory { StateTerritoryID=17, State = "KY", Region = "East South Central" },
                new StateTerritory { StateTerritoryID=18, State = "LA", Region = "West South Central" },
                new StateTerritory { StateTerritoryID=19, State = "ME", Region = "New England" },
                new StateTerritory { StateTerritoryID=20, State = "MD", Region = "South Atlantic" },
                new StateTerritory { StateTerritoryID=21, State = "MA", Region = "New England" },
                new StateTerritory { StateTerritoryID=22, State = "MI", Region = "East North Central" },
                new StateTerritory { StateTerritoryID=23, State = "MN", Region = "West North Central" },
                new StateTerritory { StateTerritoryID=24, State = "MS", Region = "East South Central" },
                new StateTerritory { StateTerritoryID=25, State = "MO", Region = "West North Central" },
                new StateTerritory { StateTerritoryID=26, State = "MT", Region = "Mountain" },
                new StateTerritory { StateTerritoryID=27, State = "NE", Region = "West North Central" },
                new StateTerritory { StateTerritoryID=28, State = "NV", Region = "Mountain" },
                new StateTerritory { StateTerritoryID=29, State = "NH", Region = "New England" },
                new StateTerritory { StateTerritoryID=30, State = "NJ", Region = "Middle Atlantic" },
                new StateTerritory { StateTerritoryID=31, State = "NM", Region = "Mountain" },
                new StateTerritory { StateTerritoryID=32, State = "NY", Region = "Middle Atlantic" },
                new StateTerritory { StateTerritoryID=33, State = "NC", Region = "South Atlantic" },
                new StateTerritory { StateTerritoryID=34, State = "ND", Region = "West North Central" },
                new StateTerritory { StateTerritoryID=35, State = "OH", Region = "East North Central" },
                new StateTerritory { StateTerritoryID=36, State = "OK", Region = "West South Central" },
                new StateTerritory { StateTerritoryID=37, State = "OR", Region = "Pacific" },
                new StateTerritory { StateTerritoryID=38, State = "PA", Region = "Middle Atlantic" },
                new StateTerritory { StateTerritoryID=39, State = "RI", Region = "New England" },
                new StateTerritory { StateTerritoryID=40, State = "SC", Region = "South Atlantic" },
                new StateTerritory { StateTerritoryID=41, State = "SD", Region = "West North Central" },
                new StateTerritory { StateTerritoryID=42, State = "TN", Region = "East South Central" },
                new StateTerritory { StateTerritoryID=43, State = "TX", Region = "West South Central" },
                new StateTerritory { StateTerritoryID=44, State = "UT", Region = "Mountain" },
                new StateTerritory { StateTerritoryID=45, State = "VT", Region = "New England" },
                new StateTerritory { StateTerritoryID=46, State = "VA", Region = "South Atlantic" },
                new StateTerritory { StateTerritoryID=47, State = "WA", Region = "Pacific" },
                new StateTerritory { StateTerritoryID=48, State = "WV", Region = "South Atlantic" },
                new StateTerritory { StateTerritoryID=49, State = "WI", Region = "East North Central" },
                new StateTerritory { StateTerritoryID=50, State = "WY", Region = "Mountain" },

                new StateTerritory { StateTerritoryID=51, State = "American Samoa", Region = "Polynesia" },
                new StateTerritory { StateTerritoryID=52, State = "Guam", Region = "Micronesia" },
                new StateTerritory { StateTerritoryID=53, State = "Northern Mariana Islands", Region = "Micronesia"},
                new StateTerritory { StateTerritoryID=54, State = "Puerto Rico", Region = "Caribbean" },
                new StateTerritory { StateTerritoryID=55, State = "US Virgin Islands", Region = "Caribbean" }
            };

            States.ForEach(s => context.States.AddOrUpdate(i => i.StateTerritoryID, s));
            context.SaveChanges();


            var Rooms = new List<Room>
            {
                //Rooms on the E/S Wing//

               new Room { RoomNumber=102, IsOccupied = true, WingName = "South"},
               new Room { RoomNumber=103, IsOccupied = true, WingName = "South"},
               new Room { RoomNumber=105, IsOccupied = true, WingName = "South"},
               new Room { RoomNumber=106, IsOccupied = true, WingName = "South"},
               new Room { RoomNumber=107, IsOccupied = true, WingName = "South"},
               new Room { RoomNumber=108, IsOccupied = true, WingName = "South"},
               new Room { RoomNumber=109, IsOccupied = false, WingName = "South"},
               new Room { RoomNumber=110, IsOccupied = false, WingName = "South"},
               new Room { RoomNumber=112, IsOccupied = false, WingName = "South"},
               new Room { RoomNumber=114, IsOccupied = false, WingName = "South"},
               new Room { RoomNumber=115, IsOccupied = false, WingName = "South"},
               new Room { RoomNumber=116, IsOccupied = false, WingName = "South"},
               new Room { RoomNumber=117, IsOccupied = false, WingName = "South"},
               new Room { RoomNumber=118, IsOccupied = false, WingName = "South"},
               new Room { RoomNumber=119, IsOccupied = false, WingName = "South"},
               new Room { RoomNumber=120, IsOccupied = false, WingName = "South"},
               new Room { RoomNumber=121, IsOccupied = false, WingName = "South"},
               new Room { RoomNumber=122, IsOccupied = false, WingName = "South"},
               new Room { RoomNumber=123, IsOccupied = false, WingName = "South"},
               new Room { RoomNumber=124, IsOccupied = false, WingName = "South"},
               new Room { RoomNumber=125, IsOccupied = false, WingName = "South"},
               new Room { RoomNumber=126, IsOccupied = false, WingName = "South"},

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

            Rooms.ForEach(m => context.Rooms.AddOrUpdate(i => i.RoomNumber, m));
            context.SaveChanges();

            var programs = new List<ProgramType>
            {
                // RESIDENT ADMISSION TYPES
                new ProgramType { ProgramTypeID=1, ProgramDescription="Emergency Shelter" },
                new ProgramType { ProgramTypeID=2, ProgramDescription="Resident Admission"},
                new ProgramType { ProgramTypeID=3, ProgramDescription="Re-admit"},                                  

                // RESIDENT DISCHARGE TYPES
                new ProgramType { ProgramTypeID=4, ProgramDescription="Resident Graduation" },
                new ProgramType { ProgramTypeID=5, ProgramDescription="Self Discharge"},
                new ProgramType { ProgramTypeID=6, ProgramDescription="Discharge for Cause"},
                new ProgramType { ProgramTypeID=7, ProgramDescription="Higher Level of Care"},                      

                // ENROLLED PROGRAMS
                new ProgramType { ProgramTypeID=8, ProgramDescription="Work Program" },
                new ProgramType { ProgramTypeID=9, ProgramDescription="P2I" },
                new ProgramType { ProgramTypeID=10, ProgramDescription="School Program" },
                new ProgramType { ProgramTypeID=11, ProgramDescription="Financial Program"},
                new ProgramType { ProgramTypeID=12, ProgramDescription="Substance Abuse Program"},                  

                // EMERGENCY RESIDENT DISCHARGE TYPE
                new ProgramType { ProgramTypeID=13, ProgramDescription="Emergency Discharge"}

            };

            programs.ForEach(p => context.ProgramTypes.AddOrUpdate(i => i.ProgramTypeID, p));
            context.SaveChanges();

            var militaryCampaigns = new List<MilitaryCampaign>
            {
                new MilitaryCampaign { MilitaryCampaignID = 1, CampaignName="Persian Gulf", Residents = new List<Resident>() },
                new MilitaryCampaign { MilitaryCampaignID = 2, CampaignName="OEF", Residents = new List<Resident>() },
                new MilitaryCampaign { MilitaryCampaignID = 3, CampaignName="Vietnam", Residents = new List<Resident>() },
                new MilitaryCampaign { MilitaryCampaignID = 4, CampaignName="OIF", Residents = new List<Resident>() },
                new MilitaryCampaign { MilitaryCampaignID = 5, CampaignName="Bosnia", Residents = new List<Resident>() }
            };

            militaryCampaigns.ForEach(m => context.MilitaryCampaigns.AddOrUpdate(i => i.MilitaryCampaignID, m));
            context.SaveChanges();
        }
    }
}
