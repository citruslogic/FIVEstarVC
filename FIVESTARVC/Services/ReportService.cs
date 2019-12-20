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
    public class ReportService : IDisposable
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

        public CurrentResidentOverviewViewModel GetCurrentResidentReport()
        {
            var currentResidents = context.Residents
                .AsNoTracking()
                .ToList()
                .Where(i => i.IsCurrent()).Select(i => new CurrentResidentViewModel 
                {
                    LastName = i.ClearLastName,
                    FirstName = i.ClearFirstMidName,
                    Age = i.AgeAtRelease > 0 ? i.AgeAtRelease : i.Age,
                    Campaigns = i.MilitaryCampaigns?.Select(j => j.CampaignName).ToList(),
                    Service = i.ServiceBranch.ToString(),
                    Ethnicity = i.Ethnicity
                }).ToList();

            return new CurrentResidentOverviewViewModel
            {
                CurrentResidents = currentResidents,
                AverageAge = (int) currentResidents.Select(i => i.Age).Average(),
                ArmyCount = currentResidents.Where(i => i.Service == ServiceType.ARMY.ToString()).Count(),
                NavyCount = currentResidents.Where(i => i.Service == ServiceType.NAVY.ToString()).Count(),
                AirForceCount = currentResidents.Where(i => i.Service == ServiceType.AIRFORCE.ToString()).Count(),
                MarineCount = currentResidents.Where(i => i.Service == ServiceType.MARINES.ToString()).Count(),
                CoastGuardCount = currentResidents.Where(i => i.Service == ServiceType.COASTGUARD.ToString()).Count(),

                //Post9_11Count = currentResidents.Where(i => i.Campaigns.Any(j => j == "Persian Gulf - 2001+")).Count(),


                BlackCount = currentResidents.Where(i => i.Ethnicity == EthnicityType.AFAM).Count(),
                CaucCount = currentResidents.Where(i => i.Ethnicity == EthnicityType.CAUCASIAN).Count(),
                HispCount = currentResidents.Where(i => i.Ethnicity == EthnicityType.HISPLATIN).Count(),
                AsianCount = currentResidents.Where(i => i.Ethnicity == EthnicityType.ASIAN_PACIFIC).Count(),
                NativeCount = currentResidents.Where(i => i.Ethnicity == EthnicityType.NATIVE).Count(),
                OtherCount = currentResidents.Where(i => i.Ethnicity == EthnicityType.OTHER).Count()
            };
        }


        public void Dispose()
        {
           context.Dispose();
        }
    }
}