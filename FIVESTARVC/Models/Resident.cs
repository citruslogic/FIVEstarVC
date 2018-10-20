using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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
        public bool IsCurrent()
        {
            bool current = false;
            var ev = db.ProgramEvents
                .Where(t => t.ResidentID == ResidentID)
                .OrderByDescending(t => t.ProgramEventID).ToList();

            foreach (var item in ev)
            {
                if (item.ProgramType.EventType == EnumEventType.ADMISSION)
                {
                    current = true;
                }

                if (item.ProgramType.EventType == EnumEventType.DISCHARGE)
                {
                    current = false;
                }
            }

            return current;
        }

        public DateTime? GetAdmitDate()
        {
            var events = db.ProgramEvents.Where(r => r.ResidentID == ResidentID)
                .OrderByDescending(s => s.ProgramEventID).ToList();

            foreach (ProgramEvent ev in events)
            {
                if (ev.ProgramType.EventType == EnumEventType.ADMISSION)
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
                if (ev.ProgramType.EventType == EnumEventType.DISCHARGE)
                {
                    return ev.ClearStartDate;
                }
            }

            return null;

        }


        public int DaysInCenter()
        {
            TimeSpan span = GetDischargeDate().HasValue
                ? GetDischargeDate().GetValueOrDefault().Subtract(GetAdmitDate().GetValueOrDefault())
                : DateTime.Now.Subtract(GetAdmitDate().GetValueOrDefault());

            // Resident may not be discharged yet.

            return (int)Math.Abs(span.TotalDays);
        }

    }
}



