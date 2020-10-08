using FIVESTARVC.DAL;
using FIVESTARVC.Models;
using FIVESTARVC.ViewModels;
using FIVESTARVC.ViewModels.ResidentDash;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FIVESTARVC.Controllers
{

    public class HomeController : Controller
    {
        private readonly ResidentContext db = new ResidentContext();

        private const string excelFile = "C:\\webuploads\\import.xlsx";
        private readonly ExcelPackage ep = new ExcelPackage(new FileInfo(excelFile));

        private List<ResidentBirthdayViewModel> NearestResidents { get; set; }           // Nearest birthdays.

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var residents = db.Residents.AsNoTracking();
            var tracks = db.ProgramEvents.AsNoTracking();

            var topResidents = await residents.Include(i => i.ProgramEvents.Select(j => j.ProgramType))
                .OrderByDescending(i => i.ResidentID)
                .Take(5)
                .ToListAsync().ConfigureAwait(false);
            var topRecentlyAdmittedList = topResidents.Select(data => new ResidentDashData
            {
                ResidentID = data.ResidentID,
                FirstMidName = data.ClearFirstMidName,
                LastName = data.ClearLastName,
                NumDaysInCenter = data.DaysInCenter ?? 0,
                LastAdmitDate = data.ProgramEvents.Where(i => i.ProgramType.EventType == EnumEventType.ADMISSION).OrderBy(i => i.ClearStartDate.Date).LastOrDefault().GetShortStartDate()
            }).ToList();

            var unorderedDischargedResidents = await tracks.Include(i => i.ProgramType).Include(i => i.Resident).Where(i => i.ProgramType.EventType == EnumEventType.DISCHARGE)
                .ToListAsync().ConfigureAwait(false);
                
            var topReleased = unorderedDischargedResidents
                .OrderByDescending(i => i.ClearStartDate.Date)
                .Take(5)
                .ToList();
            var topRecentlyReleasedList = topReleased.Select(data => new ResidentDashData
            {
                ResidentID = data.ResidentID,
                FirstMidName = data.Resident.ClearFirstMidName,
                LastName = data.Resident.ClearLastName,
                NumDaysInCenter = data.Resident.DaysInCenter ?? 0,
                LastDischargeDate = data.GetShortStartDate()
            }).ToList();
           
           
            /* David Thompson's (dthompson) grad count
             * Move to a central location in code at a more convenient date. Revised by Tytus on 10/26 
              We don't know if higher level of care residents are returning or will graduate. */
            var graduated = db.Database.SqlQuery<double>(@"select convert(float, count(distinct p.ResidentID))
                                                                            from Person p 
                                                                            join ProgramEvent pe on p.ResidentID = pe.ResidentID
                                                                                where ProgramTypeId = '4'").Single();

            //Finds number admitted
            var admitted = await db.ProgramEvents
                .Include(i => i.ProgramType)
                .Where(i => i.ProgramType.ProgramDescription.Equals("Resident Admission", StringComparison.InvariantCultureIgnoreCase))
                .CountAsync().ConfigureAwait(false);

            var emergencyShelterResidents = await db.ProgramEvents.Include(i => i.ProgramType)
                .Where(i => i.ProgramType.ProgramDescription.Equals("Emergency Shelter", StringComparison.InvariantCultureIgnoreCase))
                .CountAsync().ConfigureAwait(false);

            var currentESResidents = await residents
                .Where(i => i.IsCurrent)
                .Include(i => i.ProgramEvents.Select(j => j.ProgramType))
                .CountAsync(i => i.ProgramEvents.Select(j => j.ProgramType).Any(j => j.ProgramTypeID == 1))
                .ConfigureAwait(false);

            var dischargeHigherLevelOfCare = db.Database.SqlQuery<int>(@"select distinct
                                                                        pe.ResidentID 
                                                                        from ProgramEvent pe
                                                                        where ProgramTypeId = '7' 
	                                                                    ").Count();

            var currentResidents = await residents.Where(i => i.IsCurrent).CountAsync().ConfigureAwait(false);
            double eligibleDischarges = admitted - dischargeHigherLevelOfCare - emergencyShelterResidents - currentResidents;

            
            // Finds grad percent
            var gradPercent = admitted > 0 ? (graduated / eligibleDischarges * 100).ToString("0.##", CultureInfo.CurrentCulture) : "0";
            
            /*******************************************/

            /* Get the resident with the nearest birthday */
            await FindNearest(await residents.Where(i => i.IsCurrent).ToListAsync().ConfigureAwait(false)).ConfigureAwait(false);

            var dashboardOverview = new MainDashboardData
            {
                TotalPopulation = await residents.CountAsync().ConfigureAwait(false),
                CurrentPopulation = currentResidents,
                EmergencyShelterCount = currentESResidents,
                Graduated = graduated,
                Admitted = admitted,
                EligibleDischarges = eligibleDischarges,
                GradPercent = gradPercent,
                NearestResidents = NearestResidents,
                TopResidents = topRecentlyAdmittedList,
                TopReleasedResidents = topRecentlyReleasedList
            };

            return View(dashboardOverview);
        }

        /* Get the resident with the nearest birthday. */
        /* https://www.ict.social/csharp/wpf/course-birthday-reminder-in-csharp-net-wpf-logic-layer */
        private async Task FindNearest(List<Resident> currentResidents)
        {
            var residents = await db.Residents.AsNoTracking().Where(i => i.IsCurrent).ToListAsync().ConfigureAwait(false);

            var sortedResidents = currentResidents.Select(i => new ResidentBirthdayViewModel
            {
                FullName = i.Fullname,
                BDateMonthName = i.BDateMonthName,
                Day = i.ClearBirthdate.GetValueOrDefault().Day.ToString(CultureInfo.CurrentCulture),
                RemainingDays = i.RemainingDays
            }).OrderBy(o => o.RemainingDays);

            if (sortedResidents.Any())
                NearestResidents = sortedResidents.Take(2).ToList();
            else
                NearestResidents = null;
        }

        [HttpGet]
        public ActionResult ImportData()
        {
            ImportedListData listData = new ImportedListData();

            var ws = ep.Workbook.Worksheets["Sheet1"];

            var genders = new List<string>();
            for (int rw = 2; rw <= ws.Dimension.End.Row; rw++)
            {
                if (ws.Cells[rw, 1].Value != null)
                    genders.Add(ws.Cells[rw, 1].Value.ToString());
            }

            listData.Genders = genders;

            var firstnames = new List<string>();
            for (int rw = 2; rw <= ws.Dimension.End.Row; rw++)
            {
                if (ws.Cells[rw, 3].Value != null)
                    firstnames.Add(ws.Cells[rw, 3].Value.ToString());
            }

            listData.FirstNames = firstnames;

            return View(listData);
        }

        [HttpGet]
        public ActionResult About()
        {

            return PartialView();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ep.Dispose();
                db.Dispose();
            }
            base.Dispose(disposing);
        }


    }
}
