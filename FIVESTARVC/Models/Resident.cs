using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data.Entity;


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

        [Display(Name = "Actual Days In Center")]
        public int? ActualDaysStayed { get; set; }

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
                .Include(i => i.ProgramType)
                .Where(t => t.ResidentID == ResidentID)
                .ToList();

            if (ev != null)
            {
                foreach (var item in ev)
                {
                    if (item != null && item.ProgramType != null)
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
                }
            }

            return current;
        }

        public DateTime? GetAdmitDate()
        {
            var ev = db.ProgramEvents
                .Where(r => r.ResidentID == ResidentID)
                .OrderByDescending(s => s.ProgramEventID)
                .FirstOrDefault(i => i.ProgramType.EventType == EnumEventType.ADMISSION);

            if (ev != null)
            {
                return ev.ClearStartDate;
            }

            return null;
        }

        public DateTime? GetDischargeDate()
        {
            var events = db.ProgramEvents
                .Include(i => i.ProgramType)
                .Where(r => r.ResidentID == ResidentID)
                .OrderByDescending(s => s.ProgramEventID).ToList();

            foreach (ProgramEvent ev in events)
            {
                if (ev.ProgramType?.EventType == EnumEventType.DISCHARGE)
                {
                    return ev.ClearStartDate;
                }
            }

            return null;
        }

        public int? DaysInCenter
        {
            get
            {
                // Resident may not be discharged yet.
                TimeSpan span = GetDischargeDate().HasValue
                    ? GetDischargeDate().Value.Subtract(GetAdmitDate().GetValueOrDefault(DateTime.Today))
                    : DateTime.Now.Date.Subtract(GetAdmitDate().GetValueOrDefault(DateTime.Today));

                return (int?) Math.Abs(span.TotalDays);
            }
        }

    }
}



