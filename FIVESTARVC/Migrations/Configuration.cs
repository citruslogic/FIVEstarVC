namespace FIVESTARVC.Migrations
{
    using FIVESTARVC.Models;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;

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
                new StateTerritory { StateTerritoryID=55, State = "US Virgin Islands", Region = "Caribbean" },
                new StateTerritory { StateTerritoryID=56, State = "Other", Region = "Other" }
            };

            States.ForEach(s => context.States.AddOrUpdate(i => i.StateTerritoryID, s));
            context.SaveChanges();

            var programs = new List<ProgramType>
            {
                // RESIDENT ADMISSION TYPES
                new ProgramType { ProgramTypeID=1, EventType=EnumEventType.ADMISSION, ProgramDescription="Emergency Shelter" },
                new ProgramType { ProgramTypeID=2, EventType=EnumEventType.ADMISSION, ProgramDescription="Resident Admission"},
                new ProgramType { ProgramTypeID=3, EventType=EnumEventType.ADMISSION, ProgramDescription="Re-admit"},                                  

                // RESIDENT DISCHARGE TYPES
                new ProgramType { ProgramTypeID=4, EventType=EnumEventType.DISCHARGE, ProgramDescription="Resident Graduation" },
                new ProgramType { ProgramTypeID=5, EventType=EnumEventType.DISCHARGE, ProgramDescription="Self Discharge"},
                new ProgramType { ProgramTypeID=6, EventType=EnumEventType.DISCHARGE, ProgramDescription="Discharge for Cause"},
                new ProgramType { ProgramTypeID=7, EventType=EnumEventType.DISCHARGE, ProgramDescription="Higher Level of Care"},                      

                // ENROLLED PROGRAMS
                new ProgramType { ProgramTypeID=8, EventType=EnumEventType.TRACK, ProgramDescription="Work Program" },
                new ProgramType { ProgramTypeID=9, EventType=EnumEventType.TRACK, ProgramDescription="P2I" },
                new ProgramType { ProgramTypeID=10,EventType=EnumEventType.TRACK, ProgramDescription="School Program" },
                new ProgramType { ProgramTypeID=11,EventType=EnumEventType.TRACK, ProgramDescription="Financial Program"},
                new ProgramType { ProgramTypeID=12,EventType=EnumEventType.TRACK, ProgramDescription="Substance Abuse Program"},
                                                   
                // EMERGENCY RESIDENT DISCHARGE TYPE
                new ProgramType { ProgramTypeID=13, EventType=EnumEventType.DISCHARGE, ProgramDescription="Emergency Discharge"},

                // SYSTEM MAINTENANCE TYPES
                new ProgramType { ProgramTypeID=14, EventType=EnumEventType.SYSTEM, ProgramDescription="Room change" }
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

            var referrals = new List<Referral>
            {
                new Referral { ReferralID = 1, ReferralName = "Internet/Google" },
                new Referral { ReferralID = 2, ReferralName = "VA" },
                new Referral { ReferralID = 3, ReferralName = "Wounded Warrior" },
                new Referral { ReferralID = 4, ReferralName = "Sulzbacher" },
                new Referral { ReferralID = 5, ReferralName = "Clara White" },
                new Referral { ReferralID = 6, ReferralName = "family/friend" },
                new Referral { ReferralID = 7, ReferralName = "Wekiva Springs" },
                new Referral { ReferralID = 8, ReferralName = "Changing Homelessness" },
                new Referral { ReferralID = 9, ReferralName = "Mental Health Counselor" },
                new Referral { ReferralID = 10, ReferralName = "Other" }
            };

            referrals.ForEach(r => context.Referrals.AddOrUpdate(i => i.ReferralID, r));
            context.SaveChanges();
        }
    }
}
