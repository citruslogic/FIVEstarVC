using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FIVESTARVC.Models;
using FIVESTARVC.DAL;
using FIVESTARVC.ViewModels;
using DelegateDecompiler;

namespace FIVESTARVC.Controllers
{
    public class CenterOverviewController : Controller
    {
        ResidentContext db = new ResidentContext();

        // GET: AgeGroupBreakdown
        public ActionResult AgeGroupBreakdown()
        {
            var AgeGroups = db.Residents.ToList().Where(cur => cur.IsCurrent()).GroupBy(r => r.Age / 10).Select(group => new AgeGroups
            {
                AgeGroup = String.Format("{0} - {1}", group.Key * 10, (group.Key - 1) * 10),
                Count = group.Count()
            });

            ViewBag.Sum = AgeGroups.Sum(group => group.Count);
            ViewBag.AverageAge = GetCurrentAverageAge();
                       
            return PartialView("_AgeGroupBreakdown", AgeGroups.ToList());
        }

        // GET: GetAverageStay
        public ActionResult GetAverageStay()
        {
            //Variables to find average length of stay
            int total = 0;
            int numbCount = 0;

            var residents = db.Residents.ToList();

            foreach (Resident resident in residents)
            {
                if (resident.IsCurrent())
                {
                    continue;
                }
                numbCount++;
                total += resident.DaysInCenter();
            }

            /* Scenario is unlikely but possible. Thanks David! */
            if (numbCount == 0)
            {
                ViewBag.AvgStay = 0;

            }
            else
            {
                ViewBag.AvgStay = total / numbCount;

            }

            return PartialView("_AverageStay");
        }

        public double GetCurrentAverageAge()
        {

            IEnumerable<ReportingResidentViewModel> residentListing = db.Residents.ToList().Where(cur => cur.IsCurrent())
                .Select(r => new ReportingResidentViewModel { ID = r.ResidentID, Age = r.Age.Computed() });

            return Math.Round(residentListing.Average(r => r.Age), 2);
        }

    }
}