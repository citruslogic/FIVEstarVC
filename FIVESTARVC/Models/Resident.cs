using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data.Entity;
using FIVESTARVC.Helpers;

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

        [Display(Name = "Note")]
        [StringLength(150)]
        public string Note { get; set; }

        [Display(Name = "Actual Days In Center")]
        public int? ActualDaysStayed { get; set; }

        public virtual ICollection<MilitaryCampaign> MilitaryCampaigns { get; set; }
        public virtual ICollection<ProgramEvent> ProgramEvents { get; set; }

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
                var admitDate = GetAdmitDate().GetValueOrDefault(DateTime.Today);
                var dischargeDate = GetDischargeDate().HasValue ? GetDischargeDate().Value : DateTime.Today;
                // Resident may not be discharged yet.
                TimeSpan span = dischargeDate.Subtract(admitDate);

                return (int?)Math.Abs(span.TotalDays);
            }
        }

        public DateTime GetNextAdmitDate(DateTime? initialDate = null)
        {
            if (initialDate.HasValue)
            {
               return db.ProgramEvents.Include(i => i.ProgramType)
                                .ToList()
                               .Where(r => r.ResidentID == ResidentID
                                    && r.ProgramType.EventType == EnumEventType.ADMISSION
                                    && r.ClearStartDate > initialDate).FirstOrDefault().ClearStartDate;
                
            }

            return db.ProgramEvents.Include(i => i.ProgramType)
                                .ToList()
                               .Where(r => r.ResidentID == ResidentID
                                    && r.ProgramType.EventType == EnumEventType.ADMISSION)
                                    .FirstOrDefault().ClearStartDate;
        }

        public DateTime? GetNextDischargeDate(DateTime? initialDate = null)
        {
            if (initialDate.HasValue)
            {
                return db.ProgramEvents.Include(i => i.ProgramType)
                                .ToList()
                                .Where(r => r.ResidentID == ResidentID
                                     && r.ProgramType.EventType == EnumEventType.DISCHARGE
                                     && r.ClearStartDate > initialDate).FirstOrDefault()?.ClearStartDate;

            }

            return db.ProgramEvents.Include(i => i.ProgramType)
                               .ToList()
                               .Where(r => r.ResidentID == ResidentID
                                    && r.ProgramType.EventType == EnumEventType.DISCHARGE)
                                    .FirstOrDefault()?.ClearStartDate;
        }
        public double MonthsStayed
        {
            get
            {
                double numMonthsStayed = 0.0;
                foreach (var date in db.ProgramEvents.Include(i => i.ProgramType)
                    .ToList()
                    .Where(i => i.ResidentID == ResidentID 
                    && i.ProgramType.EventType == EnumEventType.ADMISSION)
                    .OrderByDescending(i => i.ClearStartDate)
                    .Select(i => i.ClearStartDate))
                {
                    var nextDischargeDate = GetNextDischargeDate(date);
                    if (nextDischargeDate == null)
                    {
                        numMonthsStayed += Calendar.GetMonths(DateTime.Today, date);
                    } else
                    {
                        numMonthsStayed += Calendar.GetMonths(nextDischargeDate.GetValueOrDefault(), date);
                    }
                }

                return Math.Round(numMonthsStayed, 2);
            }
        }

    }
}



