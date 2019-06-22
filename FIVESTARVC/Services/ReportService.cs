using FIVESTARVC.DAL;
using FIVESTARVC.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using FIVESTARVC.Models;
using System.Data;
using System.Data.Entity;



namespace FIVESTARVC.Services
{
    public class ReportService
    {
        private readonly ResidentContext context = new ResidentContext();
        public List<ResidencyReportViewModel> GetResidencyData()
        {
            return context.Residents.Include(i => i.ProgramEvents.Select(j => j.ProgramType)).AsNoTracking().ToList().Select(i => new ResidencyReportViewModel
            {
                LastName = i.ClearLastName,
                FirstName = i.ClearFirstMidName,
                AdmitDates = i.ProgramEvents.Where(j => j.ProgramType.EventType == EnumEventType.ADMISSION).ToList().Select(k => k.ClearStartDate.ToShortDateString()).ToList(),
                DischargeDate = i.ProgramEvents.Where(j => j.ProgramType.EventType == EnumEventType.DISCHARGE).ToList().Select(k => k.ClearStartDate.ToShortDateString()).LastOrDefault(),
                IsCurrent = i.IsCurrent(),
                DaysInResidence = i.ActualDaysStayed.GetValueOrDefault(0),
                MonthsStayed = i.MonthsStayed

            }).ToList();

        }

        public List<ResidentReferralViewModel> GetReferralReport()
        {
            var referralModel = context.Referrals.Include(i => i.Residents)
                .ToList().Select(i => new ResidentReferralViewModel
                {
                    ReferralName = i.ReferralName,
                    CumulCount = i.Residents.Count,
                    CurrentCount = i.Residents.Where(j => j.IsCurrent()).Count()
                }).ToList();

            return referralModel;
        }

    }
}