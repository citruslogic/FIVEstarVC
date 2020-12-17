using FIVESTARVC.DAL;
using FIVESTARVC.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using FIVESTARVC.Models;
using System.Data;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.Ajax.Utilities;
using FIVESTARVC.Helpers;
using System.Globalization;

namespace FIVESTARVC.Services
{
    public class ReportService : IDisposable
    {
        private bool isDisposed;
        private readonly ResidentContext context = new ResidentContext();
        public List<ResidencyReportViewModel> GetResidencyData()
        {
            return context.Residents
                .Include(i => i.ProgramEvents.Select(j => j.ProgramType))
                .AsNoTracking().ToList()
                .Select(i => new ResidencyReportViewModel
                {
                    LastName = i.ClearLastName,
                    FirstName = i.ClearFirstMidName,
                    AdmitDates = i.ProgramEvents.Where(j => j.ProgramType.EventType == EnumEventType.ADMISSION).ToList().Select(k => k.ClearStartDate.ToShortDateString()).ToList(),
                    DischargeDate = i.ProgramEvents.Where(j => j.ProgramType.EventType == EnumEventType.DISCHARGE).ToList().Select(k => k.ClearStartDate.ToShortDateString()).LastOrDefault(),
                    IsCurrent = i.IsCurrent,
                    DaysInResidence = i.ActualDaysStayed.GetValueOrDefault(0),
                    MonthsStayed = i.MonthsStayed

                }).ToList();

        }

        public List<ResidentReferralViewModel> GetReferralReport()
        {
            var referralModel = context.Referrals
                .Include(i => i.Residents)
                .ToList().Select(i => new ResidentReferralViewModel
                {
                    ReferralName = i.ReferralName,
                    CumulCount = i.Residents.Count,
                    CurrentCount = i.Residents.Where(j => j.IsCurrent).Count()
                }).ToList();

            return referralModel;
        }

        public CurrentResidentOverviewViewModel GetCurrentResidentReport()
        {
            var currentResidents = context.Residents
                .AsNoTracking()
                .Include(i => i.ProgramEvents.Select(j => j.ProgramType))
                .ToList()
                .Where(i => i.IsCurrent).Select(i => new CurrentResidentViewModel
                {
                    LastName = i.ClearLastName,
                    FirstName = i.ClearFirstMidName,
                    Age = i.AgeAtRelease > 0 ? i.AgeAtRelease : i.Age,
                    Campaigns = i.MilitaryCampaigns?.Select(j => j.CampaignName).ToList(),
                    Service = i.ServiceBranch.ToString(),
                    Ethnicity = i.Ethnicity,
                    IsEmergencyShelter = i.ProgramEvents.Select(j => j.ProgramType).Any(j => j.ProgramTypeID == 1)
                }).ToList();

            var campaignCounts = context.MilitaryCampaigns.ToList().Select(i => new CampaignCountViewModel
            {
                CampaignName = i.CampaignName,
                Count = i.Residents.Where(j => j.IsCurrent).Any() ? i.Residents.Where(j => j.IsCurrent).Count() : 0
            }).ToList();

            return new CurrentResidentOverviewViewModel
            {
                CurrentResidents = currentResidents,
                AverageAge = (int)currentResidents.Select(i => i.Age).Average(),
                ArmyCount = currentResidents.Where(i => i.Service == ServiceType.ARMY.ToString()).Count(),
                NavyCount = currentResidents.Where(i => i.Service == ServiceType.NAVY.ToString()).Count(),
                AirForceCount = currentResidents.Where(i => i.Service == ServiceType.AIRFORCE.ToString()).Count(),
                MarineCount = currentResidents.Where(i => i.Service == ServiceType.MARINES.ToString()).Count(),
                CoastGuardCount = currentResidents.Where(i => i.Service == ServiceType.COASTGUARD.ToString()).Count(),

                Campaigns = campaignCounts,

                BlackCount = currentResidents.Where(i => i.Ethnicity == EthnicityType.AFAM).Count(),
                CaucCount = currentResidents.Where(i => i.Ethnicity == EthnicityType.CAUCASIAN).Count(),
                HispCount = currentResidents.Where(i => i.Ethnicity == EthnicityType.HISPLATIN).Count(),
                AsianCount = currentResidents.Where(i => i.Ethnicity == EthnicityType.ASIAN_PACIFIC).Count(),
                NativeCount = currentResidents.Where(i => i.Ethnicity == EthnicityType.NATIVE).Count(),
                OtherCount = currentResidents.Where(i => i.Ethnicity == EthnicityType.OTHER).Count(),

                Total = currentResidents.Count,
            };
        }

        public CurrentResidentOverviewViewModel ResidentsByTrackType(int ProgramTypeID)
        {
            List<CurrentResidentViewModel> residents = new List<CurrentResidentViewModel>();

            var events = context.ProgramEvents
                            .AsNoTracking()
                            .Include(i => i.Resident)
                            .Include(i => i.ProgramType)
                            .Where(i => i.ProgramTypeID == ProgramTypeID)
                            .ToList();
                            
            residents = events.OrderBy(i => i.Resident.ClearLastName)
                            .Select(i => new CurrentResidentViewModel
                            {
                                LastName = i.Resident.ClearLastName,
                                FirstName = i.Resident.ClearFirstMidName,
                            })
                            .ToList();

            return new CurrentResidentOverviewViewModel
            {
                CurrentResidents = residents,
                TrackType = events.FirstOrDefault().ProgramType.ProgramDescription,
                
                Total = residents.DistinctBy(i => i.LastName + " " + i.FirstName).Count()
            };
        }
        
        public CurrentResidentOverviewViewModel ResidentsByYear(string year = null)
        {
            int yearValue = 0;
            List<CurrentResidentViewModel> residents = new List<CurrentResidentViewModel>();

            if (string.IsNullOrEmpty(year))
            {
                return new CurrentResidentOverviewViewModel
                {
                    CurrentResidents = residents,
                    Total = 0
                };
            }
            else if (int.TryParse(year, out yearValue) != true)
            {
                yearValue = DateTime.Now.Year;
            }

            residents = context.Residents
                .AsNoTracking()
                .Include(i => i.ProgramEvents)
                .ToList()
                .Where(i => i.ProgramEvents.Any(j => j.ClearStartDate.Year == yearValue && j.ProgramType.EventType == EnumEventType.ADMISSION))
                .Select(i => new CurrentResidentViewModel
                {
                    LastName = i.ClearLastName,
                    FirstName = i.ClearFirstMidName,
                    Service = FSEnumHelper.GetDescription(i.ServiceBranch),
                    DateAdmitted = i.ProgramEvents.LastOrDefault(j => j.ClearStartDate.Year == yearValue && j.ProgramType.EventType == EnumEventType.ADMISSION).GetShortStartDate()
                }).ToList();

            return new CurrentResidentOverviewViewModel
            {
                CurrentResidents = residents,
                Total = residents.Count
            };
        }

        public UpToYearResidentAgeViewModel UpToYearResidentAgeReport(bool currentOnly, string year = null)
        {
            int yearValue = 0;
            List<SelectedResident> residents;

            if (string.IsNullOrEmpty(year))
            {
                return new UpToYearResidentAgeViewModel();
            }
            else if (int.TryParse(year, out yearValue) != true)
            {
                yearValue = DateTime.Now.Year;
            }

            var queryEnumerable = context.Residents
                        .AsNoTracking()
                        .Include(i => i.ProgramEvents.Select(j => j.ProgramType))
                        .ToList()
                        .Where(i => i.ProgramEvents.Any(j => j.ClearStartDate.Year <= yearValue && j.ProgramType.EventType == EnumEventType.ADMISSION));

            if (currentOnly)
            {
                residents = queryEnumerable.Where(i => i.IsCurrent).Select(i => new SelectedResident
                {
                    ResidentID = i.ResidentID,
                    LastName = i.ClearLastName,
                    FullName = i.Fullname,
                    Age = i.GetAgeAtRelease,
                    Birthdate = i.ClearBirthdate?.ToShortDateString(),
                    DateDischarged = i.GetNextDischargeDate(i.ProgramEvents.LastOrDefault(j => j.ProgramType.EventType == EnumEventType.ADMISSION)?.ClearStartDate).HasValue
                                ? i.GetNextDischargeDate(i.ProgramEvents.LastOrDefault(j => j.ProgramType.EventType == EnumEventType.ADMISSION)?.ClearStartDate).Value.ToShortDateString()
                                : "No next discharge date.",
                    DateAdmitted = i.ProgramEvents.LastOrDefault(j => j.ProgramType.EventType == EnumEventType.ADMISSION && j.ClearStartDate.Year <= yearValue).ClearStartDate,
                }).OrderBy(i => i.LastName)
                  .ToList();
            } else
            {
                residents = queryEnumerable.Select(i => new SelectedResident
                {
                    ResidentID = i.ResidentID,
                    LastName = i.ClearLastName,
                    FullName = i.Fullname,
                    Age = i.GetAgeAtRelease,
                    Birthdate = i.ClearBirthdate?.ToShortDateString(),
                    DateDischarged = i.GetNextDischargeDate(i.ProgramEvents.LastOrDefault(j => j.ProgramType.EventType == EnumEventType.ADMISSION)?.ClearStartDate).HasValue
                                ? i.GetNextDischargeDate(i.ProgramEvents.LastOrDefault(j => j.ProgramType.EventType == EnumEventType.ADMISSION)?.ClearStartDate).Value.ToShortDateString()
                                : "No next discharge date.",
                    DateAdmitted = i.ProgramEvents.LastOrDefault(j => j.ProgramType.EventType == EnumEventType.ADMISSION && j.ClearStartDate.Year <= yearValue).ClearStartDate
                }).OrderBy(i => i.LastName)
                  .ToList();
            }

            return new UpToYearResidentAgeViewModel
            {
                Residents = residents.GroupBy(i => i.DateAdmitted.Year).ToList(),
                Total = residents.Count
            };
        }

        public async Task<ResidentWPPReportViewModel> GetResidentWPPReportAsync()
        {
            var restrictedTracks = new int[]
            {
                2163,
                2189,
                2190,
                2191,
                2192,
                2193,
                2194,
                2195,
                2196,
                2197,
                2198,
                2199
            };

            var tracks = await context.ProgramEvents
                .AsNoTracking()
                .Where(i => restrictedTracks.Any(j => j == i.ProgramTypeID))
                .Include(i => i.ProgramType)
                .ToListAsync().ConfigureAwait(false);

            var totalWPPTracks = tracks.Count;

            var wppTrackPercentages = tracks.Select(i => new ResidentWPPTrack
            {
                Track = i
            })
            .GroupBy(i => i.Track.ProgramType.ProgramDescription)
            .Select(i => new ResidentWPPTrackAggregate
            {
                TrackName = i.Key,
                Count = decimal.Round(i.Count(), 2),
                Percentage = decimal.Round(i.Count() / totalWPPTracks, 2)
            })
            .GroupBy(i => i.StartDate.ToString("MMMM", new CultureInfo("en-US")))
            .ToList();            

            var wppTrackData = new ResidentWPPReportViewModel
            {
                ResidentWPPTrackPercentages = wppTrackPercentages,
                WPPTotalCountByType = new Dictionary<string, decimal>
                {
                    { "Therapy",  tracks.Count(j => j.ProgramTypeID == 2189 || j.ProgramTypeID == 2190 || j.ProgramTypeID == 2191) / totalWPPTracks
                    },
                    {
                        "Counseling", tracks.Count(j => j.ProgramTypeID == 2194 || j.ProgramTypeID == 2193) / totalWPPTracks
                    },
                    {
                        "Housing", tracks.Count(j => j.ProgramTypeID == 2197 || j.ProgramTypeID == 2198 || j.ProgramTypeID == 2199) / totalWPPTracks
                    },
                    {   "Education", 0 // TODO: Need to find out what else required in addition to financial education
                    },
                    {
                        "Employment Services", tracks.Count(j => j.ProgramTypeID == 2192) / totalWPPTracks
                    }
                
                }
            };

            return wppTrackData;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (disposing)
            {
                context.Dispose();
            }

            isDisposed = true;
        }
    }
}