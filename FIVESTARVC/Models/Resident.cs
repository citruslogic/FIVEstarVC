using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using FIVESTARVC.DAL;
using FIVESTARVC.Validators;
using DelegateDecompiler;
using System.Globalization;

namespace FIVESTARVC.Models
{
   
    public class Resident : Person
    {

        [Display(Name = "Service Branch")]
        public ServiceType ServiceBranch { get; set; }
        [Display(Name = "Service Discharge")]
        public MilitaryDischargeType MilitaryDischarge { get; set; }
        [Display(Name = "Non-combat?")]
        public Boolean IsNoncombat { get; set; }
        [Display(Name = "In Veterans Court?")]
        public Boolean InVetCourt { get; set; }
        [Display(Name = "Room Number")]
        [ForeignKey("Room")]
        public int? RoomNumber { get; set; }
        [Display(Name = "Note")]
        [StringLength(150)]
        public string Note { get; set; }

        public virtual ICollection<MilitaryCampaign> MilitaryCampaigns { get; set; }
        public virtual ICollection<ProgramEvent> ProgramEvents { get; set; }
        public virtual Room Room { get; set; }

        [ForeignKey("Benefit")]
        public int? BenefitID { get; set; }

        public virtual Benefit Benefit { get; set; }
        

        //[Computed]
        public Boolean IsCurrent()
        {
                var current = db.ProgramEvents;

                int ID = base.ResidentID;

                Boolean internalBool = false;

                foreach (var ProgramEvent in current)
                {
                    if (ID == ProgramEvent.ResidentID)
                    {
                        if (ProgramEvent.ProgramTypeID == 2 //admission
                        || ProgramEvent.ProgramTypeID == 3 //re-admit
                        || ProgramEvent.ProgramTypeID == 1)
                        {
                            internalBool = true;
                        }

                        if (ProgramEvent.ProgramTypeID == 4 //graduation
                        || ProgramEvent.ProgramTypeID == 5 //discharge
                        || ProgramEvent.ProgramTypeID == 6 //discharge
                        || ProgramEvent.ProgramTypeID == 7
                        || ProgramEvent.ProgramTypeID == 13) 
                        {
                            internalBool = false;
                        }
                    }
                }
                return internalBool;
            }

        public DateTime? GetAdmitDate()
        {
            var events = db.ProgramEvents.Where(r => r.ResidentID == ResidentID).OrderByDescending(s => s.ProgramEventID).ToList();

            foreach (ProgramEvent ev in events)
            {
                if (ev.ProgramTypeID == 1 || ev.ProgramTypeID == 2 || ev.ProgramTypeID == 3)
                {
                    return ev.ClearStartDate;
                }
            }

            return null;
        }

        public DateTime? GetDischargeDate()
        {
            var events = db.ProgramEvents.Where(r => r.ResidentID == ResidentID).OrderByDescending(s => s.ProgramEventID).ToList();

            foreach (ProgramEvent ev in events)
            {
                if (ev.ProgramTypeID == 4 || ev.ProgramTypeID == 5
                    || ev.ProgramTypeID == 6 || ev.ProgramTypeID == 7)
                {
                    return ev.ClearStartDate;
                }
            }

            return null;

            }


        public int DaysInCenter()
        {
            TimeSpan span;

            // Resident may not be discharged yet.
            if (GetDischargeDate().HasValue)
            {
                span = GetDischargeDate().GetValueOrDefault().Subtract(GetAdmitDate().GetValueOrDefault());
                
            } else
            {
                span = DateTime.Now.Subtract(GetAdmitDate().GetValueOrDefault());

            }

                return (int) Math.Abs(span.TotalDays);

        }

    }
}

   

