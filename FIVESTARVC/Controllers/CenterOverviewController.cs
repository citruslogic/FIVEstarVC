using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FIVESTARVC.Models;
using FIVESTARVC.DAL;
using FIVESTARVC.ViewModels;

namespace FIVESTARVC.Controllers
{
    public class CenterOverviewController : Controller
    {
        ResidentContext db = new ResidentContext();

        // GET: AgeGroupBreakdown
        public ActionResult AgeGroupBreakdown()
        {
            var AgeGroups = db.Residents.ToList().GroupBy(r => r.Age / 10).Select(group => new AgeGroups
            {
                AgeGroup = String.Format("{0} - {1}", group.Key * 10, (group.Key - 1) * 10),
                Count = group.Count()
            });

            ViewBag.Sum = AgeGroups.Sum(group => group.Count);
                       
            return PartialView("_AgeGroupBreakdown", AgeGroups.ToList());
        }
       
    }
}