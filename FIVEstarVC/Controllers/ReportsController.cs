using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FIVESTARVC.DAL;
using FIVESTARVC.Models;

namespace FIVESTARVC.Controllers
{
    public class ReportsController : Controller
    {
        private ResidentContext DB = new ResidentContext();

        // GET: Reports
        public ActionResult Index()
        {
            //Variables to count branch types
            ViewBag.NavyCount = DB.Residents.Count(x => x.ServiceBranch == ServiceType.NAVY);
            ViewBag.MarineCount = DB.Residents.Count(x => x.ServiceBranch == ServiceType.MARINES);
            ViewBag.ArmyCount = DB.Residents.Count(x => x.ServiceBranch == ServiceType.ARMY);
            ViewBag.AirForceCount = DB.Residents.Count(x => x.ServiceBranch == ServiceType.AIRFORCE);
            ViewBag.CoastGuardCount = DB.Residents.Count(x => x.ServiceBranch == ServiceType.COASTGUARD);
   
            ViewBag.TotalCount = DB.Residents.Count();

            //ViewBag.Graduated = DB.Events.Count(x => x.LeaveDate.HasValue);

            return View(); //please work v2
        }
    }
}