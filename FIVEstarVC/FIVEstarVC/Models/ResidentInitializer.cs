using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using FIVEstarVC.Models;

namespace FIVEstarVC.Models
{
    public class ResidentInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<FiveStarModel>
    {
        protected override void Seed(FiveStarModel context)
        {
            var residents = new List<Resident>
            {
                new Resident{ ResidentID = 0, LastName="Dunham", FirstName="Chris", RoomNumber=10},
                new Resident{ ResidentID = 1, LastName="Fernandez", FirstName="Therese", RoomNumber=15}

            };

            residents.ForEach(r => context.Residents.Add(r));
            context.SaveChanges();

            var reasontypes = new List<ReasonType>
            {
                new ReasonType{DischargeReason="Graduated"},
                new ReasonType{DischargeReason="Dismissed for Cause"},
                new ReasonType{DischargeReason="Self discharged"}

            };

            reasontypes.ForEach(rt => context.ReasonTypes.Add(rt));
            context.SaveChanges();

            var militarycampaigns = new List<MilitaryCampaign>
            {
                new MilitaryCampaign{ Campaign="Vietnam"},
                new MilitaryCampaign{ Campaign="Desert Storm"}
            };

            militarycampaigns.ForEach(mc => context.MilitaryCampaigns.Add(mc));
            context.SaveChanges();

            var militaryservices = new List<MilitaryService>
            {
                new MilitaryService{ServiceName="Army"},
                new MilitaryService{ServiceName="Marines"}
            };

            militaryservices.ForEach(ms => context.MilitaryServices.Add(ms));
            context.SaveChanges();
            /*
            var resident_militaryservices = new List<Resident_MilitaryService>
            {
                new Resident_MilitaryService{ MilitaryServiceID = militaryservices.Single(s => s.MilitaryServiceID == 0).MilitaryServiceID },
                new Resident_MilitaryService{ MilitaryServiceID = militaryservices.Single(s => s.MilitaryServiceID == 1).MilitaryServiceID }
            };
            
            resident_militaryservices.ForEach(rm => context.Resident_MilitaryService.Add(rm));
            context.SaveChanges();
            */
            var programs = new List<ProgramType>
            {
                new ProgramType{ ProgramDescription = "P2I" },
                new ProgramType{ ProgramDescription = "Mental Wellness"},
                new ProgramType{ ProgramDescription = "Work Track"},
                new ProgramType{ ProgramDescription = "School Track"},
                new ProgramType{ ProgramDescription = "Emergency Shelter"}
            };

            programs.ForEach(p => context.ProgramTypes.Add(p));
            context.SaveChanges();

            var resident_programs = new List<Resident_ProgramEvent>
            {
                new Resident_ProgramEvent{ ResidentID=0, StartDate=DateTime.Parse("2010-09-01"), ProgramTypeID = programs.Single(s => s.ProgramTypeID == 2).ProgramTypeID,
                Completed = false},
                new Resident_ProgramEvent{ ResidentID=1, StartDate=DateTime.Parse("2009-01-10"), EndDate=DateTime.Parse("2009-03-25"), ReasonID=reasontypes.Single(s =>
                s.ReasonID == 1).ReasonID, Completed = true}
            };

        }
    }

}