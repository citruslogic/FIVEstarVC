using FIVESTARVC.DAL;
using FIVESTARVC.Models;
using FIVESTARVC.ViewModels;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace FIVESTARVC.Controllers
{

    public class HomeController : Controller
    {
        private readonly ResidentContext db = new ResidentContext();

        private const string excelFile = "C:\\webuploads\\import.xlsx";
        private readonly ExcelPackage ep = new ExcelPackage(new FileInfo(excelFile));

        public IEnumerable<Resident> NearestResidents { get; set; }           // Nearest birthdays.

        [Authorize]
        [HttpGet]
        public ActionResult Index()
        {
            var residents = db.Residents.AsNoTracking().OrderByDescending(r => r.ResidentID);
            var topResidents = residents.Take(5).ToList().Select(data => new DashboardData
            {
                ResidentID = data.ResidentID,
                FirstMidName = data.ClearFirstMidName,
                LastName = data.ClearLastName,
                NumDaysInCenter = data.DaysInCenter ?? 0

            });

            ViewBag.pop = residents.ToList().Where(r => r.IsCurrent).Count();
            ViewBag.allpop = residents.ToList().Count;

            /* David Thompson's (dthompson) grad count
             * Move to a central location in code at a more convenient date. Revised by Tytus on 10/26 
              We don't know if higher level of care residents are returning or will graduate. */
            var Graduated = db.Database.SqlQuery<double>(@"select convert(float, count(distinct p.ResidentID))
                                                                            from Person p 
                                                                            join ProgramEvent pe on p.ResidentID = pe.ResidentID
                                                                                where ProgramTypeId = '4'").Single();

            ViewBag.Graduated = Graduated;

            //Finds number admitted
            var Admitted = db.Database.SqlQuery<double>(@"select convert(float, count(distinct p.ResidentID))
                                                                           from Person p 
                                                                           join ProgramEvent pe on p.ResidentID = pe.ResidentID
                                                                                where ProgramTypeId in ('1')").Single();
            ViewBag.Admitted = Admitted;

            var currentResidents = db.Residents.AsNoTracking().ToList().Where(cur => cur.IsCurrent).Count();
            var admittedResidents = db.ProgramEvents.Include(i => i.ProgramType).Where(i => i.ProgramType.ProgramDescription.Equals("Resident Admission", StringComparison.InvariantCultureIgnoreCase)).Count();
            var emergencyShelterResidents = db.ProgramEvents.Include(i => i.ProgramType).Where(i => i.ProgramType.ProgramDescription.Equals("Emergency Shelter", StringComparison.InvariantCultureIgnoreCase)).Count();
            var dischargeHigherLevelOfCare = db.Database.SqlQuery<int>(@"select distinct
                                                                        pe.ResidentID 
                                                                        from ProgramEvent pe
                                                                        where ProgramTypeId = '7' 
	                                                                    ").Count();
            double eligibleDischarges = admittedResidents - dischargeHigherLevelOfCare - emergencyShelterResidents - currentResidents;
            ViewBag.EligibleDischarges = eligibleDischarges;


            if (Admitted > 0)
            {
                //finds grad percent
                double gradPercent = Graduated / eligibleDischarges * 100;
                ViewBag.gradPercent = gradPercent.ToString("0.##"); //Graduation Percentage
            }
            else
            {
                ViewBag.gradPercent = 0;
            }

            /*******************************************/

            /* Get the resident with the nearest birthday */
            FindNearest();
            ViewBag.NearestResidents = NearestResidents;

            return View(topResidents);
        }

        /* Get the resident with the nearest birthday. */
        /* https://www.ict.social/csharp/wpf/course-birthday-reminder-in-csharp-net-wpf-logic-layer */
        private void FindNearest()
        {
            var sortedResidents = db.Residents.AsNoTracking().ToList().Where(i => i.IsCurrent).OrderBy(o => o.RemainingDays);

            if (sortedResidents.Any())
                NearestResidents = sortedResidents.Take(2);
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
