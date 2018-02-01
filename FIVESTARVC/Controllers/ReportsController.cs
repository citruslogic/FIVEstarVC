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

            //Counts number of current residents, based on events
            var CurrentRes = DB.ProgramEvents;
            int count = 0;


            foreach (var ProgramEvents in CurrentRes)
            {
                if (ProgramEvents.ProgramTypeID == 7 //admission
                    || ProgramEvents.ProgramTypeID == 9 //re-admit
                    || ProgramEvents.ProgramTypeID == 5) //emergency shelter
                {
                    count++;
                    continue;
                }
                else if (ProgramEvents.ProgramTypeID == 2 //graduation
                    || ProgramEvents.ProgramTypeID == 13 //discharge
                    || ProgramEvents.ProgramTypeID == 14 //discharge
                    || ProgramEvents.ProgramTypeID == 15)//discharge
                {
                    count--;
                }
            }
            ViewBag.TotalCount = count;

            //ViewBag.TotalCount = DB.ProgramEvents.Count(x => x.ProgramTypeID == 7 && x.EndDate == null);

            ViewBag.Graduated = DB.ProgramEvents.Count(x => x.ProgramTypeID == 2);

            ViewBag.WorkProgram = DB.ProgramEvents.Count(x => x.ProgramTypeID == 1);

            ViewBag.MentalWellness = DB.ProgramEvents.Count(x => x.ProgramTypeID == 3);

            ViewBag.P2I = DB.ProgramEvents.Count(x => x.ProgramTypeID == 4);

            ViewBag.EmergencyShelter = DB.ProgramEvents.Count(x => x.ProgramTypeID == 5);

            ViewBag.SchoolProgram = DB.ProgramEvents.Count(x => x.ProgramTypeID == 6);

            //ViewBag.VeteransCourt = DB.Residents.Count(x => x.ve == 8);

            ViewBag.FinancialProgram = DB.ProgramEvents.Count(x => x.ProgramTypeID == 10);

            ViewBag.DepressionBehavioralProgram = DB.ProgramEvents.Count(x => x.ProgramTypeID == 11);

            ViewBag.SubstanceAbuseProgram = DB.ProgramEvents.Count(x => x.ProgramTypeID == 12);

            return View(); 
        }

        public ActionResult Historic()
        {
            return View();
        }
    }
}