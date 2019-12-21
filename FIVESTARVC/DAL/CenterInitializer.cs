using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using FIVESTARVC.Models;
using System.Data.Entity.Migrations;

namespace FIVESTARVC.DAL
{
    public class CenterInitializer : CreateDatabaseIfNotExists<ResidentContext>
    {
        protected override void Seed(ResidentContext context)
        {

            var States = new List<StateTerritory>
            {
                new StateTerritory { State = "AL", Region = "East South Central" },
                new StateTerritory { State = "AK", Region = "Pacific" },
                new StateTerritory { State = "AZ", Region = "Mountain" },
                new StateTerritory { State = "AR", Region = "West South Central" },
                new StateTerritory { State = "CA", Region = "Pacific" },
                new StateTerritory { State = "CO", Region = "Mountain" },
                new StateTerritory { State = "CT", Region = "New England" },
                new StateTerritory { State = "DE", Region = "South Atlantic" },
                new StateTerritory { State = "FL", Region = "South Atlantic" },
                new StateTerritory { State = "GA", Region = "South Atlantic" },
                new StateTerritory { State = "HI", Region = "Pacific" },
                new StateTerritory { State = "ID", Region = "Mountain" },
                new StateTerritory { State = "IL", Region = "East South Central" },
                new StateTerritory { State = "IN", Region = "East North Central" },
                new StateTerritory { State = "IA", Region = "West North Central" },
                new StateTerritory { State = "KS", Region = "West North Central" },
                new StateTerritory { State = "KY", Region = "East South Central" },
                new StateTerritory { State = "LA", Region = "West South Central" },
                new StateTerritory { State = "ME", Region = "New England" },
                new StateTerritory { State = "MD", Region = "South Atlantic" },
                new StateTerritory { State = "MA", Region = "New England" },
                new StateTerritory { State = "MI", Region = "East North Central" },
                new StateTerritory { State = "MN", Region = "West North Central" },
                new StateTerritory { State = "MS", Region = "East South Central" },
                new StateTerritory { State = "MO", Region = "West North Central" },
                new StateTerritory { State = "MT", Region = "Mountain" },
                new StateTerritory { State = "NE", Region = "West North Central" },
                new StateTerritory { State = "NV", Region = "Mountain" },
                new StateTerritory { State = "NH", Region = "New England" },
                new StateTerritory { State = "NJ", Region = "Middle Atlantic" },
                new StateTerritory { State = "NM", Region = "Mountain" },
                new StateTerritory { State = "NY", Region = "Middle Atlantic" },
                new StateTerritory { State = "NC", Region = "South Atlantic" },
                new StateTerritory { State = "ND", Region = "West North Central" },
                new StateTerritory { State = "OH", Region = "East North Central" },
                new StateTerritory { State = "OK", Region = "West South Central" },
                new StateTerritory { State = "OR", Region = "Pacific" },
                new StateTerritory { State = "PA", Region = "Middle Atlantic" },
                new StateTerritory { State = "RI", Region = "New England" },
                new StateTerritory { State = "SC", Region = "South Atlantic" },
                new StateTerritory { State = "SD", Region = "West North Central" },
                new StateTerritory { State = "TN", Region = "East South Central" },
                new StateTerritory { State = "TX", Region = "West South Central" },
                new StateTerritory { State = "UT", Region = "Mountain" },
                new StateTerritory { State = "VT", Region = "New England" },
                new StateTerritory { State = "VA", Region = "South Atlantic" },
                new StateTerritory { State = "WA", Region = "Pacific" },
                new StateTerritory { State = "WV", Region = "South Atlantic" },
                new StateTerritory { State = "WI", Region = "East North Central" },
                new StateTerritory { State = "WY", Region = "Mountain" },

                new StateTerritory { State = "American Samoa", Region = "Polynesia" },
                new StateTerritory { State = "Guam", Region = "Micronesia" },
                new StateTerritory { State = "Northern Mariana Islands", Region = "Micronesia"},
                new StateTerritory { State = "Puerto Rico", Region = "Caribbean" },
                new StateTerritory { State = "US Virgin Islands", Region = "Caribbean" }
            };

            States.ForEach(s => context.States.AddOrUpdate(i=> i.State, s));
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
                new MilitaryCampaign { MilitaryCampaignID = 1, CampaignName="Persian Gulf (before 9/11)", Residents = new List<Resident>() },
                new MilitaryCampaign { MilitaryCampaignID = 2, CampaignName="Persian Gulf (after 9/11)", Residents = new List<Resident>() },
                new MilitaryCampaign { MilitaryCampaignID = 3, CampaignName="OEF", Residents = new List<Resident>() },
                new MilitaryCampaign { MilitaryCampaignID = 4, CampaignName="Vietnam", Residents = new List<Resident>() },
                new MilitaryCampaign { MilitaryCampaignID = 5, CampaignName="OIF", Residents = new List<Resident>() },
                new MilitaryCampaign { MilitaryCampaignID = 6, CampaignName="Bosnia", Residents = new List<Resident>() }
            };

            militaryCampaigns.ForEach(m => context.MilitaryCampaigns.AddOrUpdate(i => i.MilitaryCampaignID, m));
            context.SaveChanges();
        }
    }
}