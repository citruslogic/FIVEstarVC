using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data.Entity;
using FIVESTARVC.Helpers;
using DelegateDecompiler;
using FIVESTARVC.DAL;

namespace FIVESTARVC.Models
{

    public class Resident : Person
    {
        readonly ResidentContext db = new ResidentContext();

        [Display(Name = "Service Branch")]
        public ServiceType ServiceBranch { get; set; }

        [Display(Name = "NG/Reserves")]
        public NGReserveServiceType? NGReserve { get; set; }

        [Display(Name = "Service Discharge")]
        public MilitaryDischargeType MilitaryDischarge { get; set; }

        [Display(Name = "Non-combat?")]
        public bool IsNoncombat { get; set; }

        [Display(Name = "In Veterans Court?")]
        public bool InVetCourt { get; set; }

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

        [ForeignKey("Referral")]
        [Display(Name = "Referral")]
        public int? ReferralID { get; set; }

        public virtual Referral Referral { get; set; }

        [Display(Name = "Other Referral")]
        public string OptionalReferralDescription { get; set; }

        public bool IsCurrent
        { get
            {
                bool current = false;
                var ev = db.ProgramEvents
                    .AsNoTracking()
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
        }

        public DateTime? GetDischargeDate()
        {
            var ev = db.ProgramEvents
                .AsNoTracking()
                .Include(i => i.ProgramType)
                .Where(r => r.ResidentID == ResidentID)
                .OrderByDescending(s => s.ProgramEventID)
                .FirstOrDefault(e => e.ProgramType.EventType == EnumEventType.DISCHARGE);

            if (ev != null)
            {
                return ev.ClearStartDate;
            } else
            {
                var evWithAdmitEndDate = db.ProgramEvents
                                        .AsNoTracking()
                                        .Include(i => i.ProgramType)
                                        .Where(r => r.ResidentID == ResidentID)
                                        .OrderBy(s => s.ProgramEventID)
                                        .ToList()
                                        .LastOrDefault(e => e.ProgramType.EventType == EnumEventType.ADMISSION 
                                            && e.ClearEndDate != null);
                
                if (evWithAdmitEndDate != null)
                {
                    return evWithAdmitEndDate.ClearEndDate;
                }
            }

            return null;
        }

        public int? GetAgeAtRelease
        {
            get
            {
                if (IsCurrent == false)
                {
                    try
                    {
                        TimeSpan? span = GetDischargeDate() - ClearBirthdate.GetValueOrDefault().Date;

                        if (span.HasValue)
                        {
                            DateTime age = DateTime.MinValue + span.Value;
                            return age.Year - 1;
                        }

                        return null;
                    }
                    catch (ArgumentOutOfRangeException /* ex */)
                    {
                        return null;
                    }
                }
                else
                {
                    return Age;
                }
            }
        }

        public int? DaysInCenter
        {
            get
            {
                bool hasBeenDischarged = false;
                bool hasbeenAdmitted = false;

                DateTime? dischargedDate = null;
                DateTime admittedDate = DateTime.Now;

                var events = db.ProgramEvents
                .AsNoTracking()
                .Include(i => i.ProgramType)
                .Where(r => r.ResidentID == ResidentID)
                .OrderBy(s => s.ProgramEventID)
                .ToList();

                TimeSpan span = TimeSpan.Zero;
                foreach (var ev in events)
                {

                    // admitted for the first time, continue counting days in center:
                    if (ev.ProgramType.EventType == EnumEventType.ADMISSION && hasBeenDischarged == false && hasbeenAdmitted == false)
                    {
                        hasbeenAdmitted = true;
                        admittedDate = ev.ClearStartDate;
                    }

                    // found a discharge event: 
                    if (ev.ProgramType.EventType == EnumEventType.DISCHARGE && ev.ClearStartDate != null && hasbeenAdmitted)
                    {
                        hasBeenDischarged = true;
                        hasbeenAdmitted = false;
                        dischargedDate = ev.ClearStartDate;
                    }

                    // if the resident has been both admitted and discharged we can start counting the difference in their days stayed.
                    if (dischargedDate != null && admittedDate != null && hasBeenDischarged)
                    {
                        span = dischargedDate.Value.Subtract(admittedDate);
                    }

                    // resident was readmitted:
                    if (ev.ProgramType.EventType == EnumEventType.ADMISSION && hasBeenDischarged && dischargedDate.HasValue)
                    {
                        hasBeenDischarged = false;
                        hasbeenAdmitted = true;

                        if (IsCurrent)
                        {
                            // still open
                            span += DateTime.Now.Subtract(ev.ClearStartDate);
                        }
                    }

                    // hasn't been discharged yet...
                    if (IsCurrent && hasBeenDischarged == false && dischargedDate == null)
                    {
                        span = DateTime.Now.Subtract(admittedDate);
                    }
                }

                return (int?)Math.Abs(span.TotalDays);
            }
        }

        public DateTime? GetNextAdmitDate(DateTime? initialDate = null)
        {
            if (initialDate.HasValue)
            {
                return db.ProgramEvents.AsNoTracking().Include(i => i.ProgramType)
                                 .ToList()
                                 .Where(r => r.ResidentID == ResidentID
                                     && r.ProgramType.EventType == EnumEventType.ADMISSION
                                     && r.ClearStartDate > initialDate).FirstOrDefault()?.ClearStartDate;

            }

            return db.ProgramEvents.AsNoTracking().Include(i => i.ProgramType)
                                .ToList()
                               .Where(r => r.ResidentID == ResidentID
                                    && r.ProgramType.EventType == EnumEventType.ADMISSION)
                                    .FirstOrDefault().ClearStartDate;
        }

        public DateTime? GetNextDischargeDate(DateTime? initialDate = null)
        {
            if (initialDate.HasValue)
            {
                return db.ProgramEvents.AsNoTracking().Include(i => i.ProgramType)
                                .ToList()
                                .Where(r => r.ResidentID == ResidentID
                                     && r.ProgramType.EventType == EnumEventType.DISCHARGE
                                     && r.ClearStartDate > initialDate).FirstOrDefault()?.ClearStartDate;

            }

            return db.ProgramEvents.AsNoTracking().Include(i => i.ProgramType)
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
                    }
                    else
                    {
                        numMonthsStayed += Calendar.GetMonths(nextDischargeDate.GetValueOrDefault(), date);
                    }
                }

                return Math.Round(numMonthsStayed, 2);
            }
        }

        [NotMapped]
        public int? FromPage
        {
            get; set;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}



