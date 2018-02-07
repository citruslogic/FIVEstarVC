using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FIVESTARVC.DAL;
using FIVESTARVC.Models;
using FIVESTARVC.ViewModels;
using Jitbit.Utils;


namespace FIVESTARVC.Controllers
{
    public class ReportsController : Controller
    {
        private ResidentContext DB = new ResidentContext();

        // GET: Reports
        public ActionResult Index()
        {
            //Variables to count branch types
            ViewBag.NavyCount = DB.Residents.Count(x => x.ServiceBranch == Models.ServiceType.NAVY);
            ViewBag.MarineCount = DB.Residents.Count(x => x.ServiceBranch == Models.ServiceType.MARINES);
            ViewBag.ArmyCount = DB.Residents.Count(x => x.ServiceBranch == Models.ServiceType.ARMY);
            ViewBag.AirForceCount = DB.Residents.Count(x => x.ServiceBranch == Models.ServiceType.AIRFORCE);
            ViewBag.CoastGuardCount = DB.Residents.Count(x => x.ServiceBranch == Models.ServiceType.COASTGUARD);

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

            ViewBag.Graduated = DB.ProgramEvents.Count(x => x.ProgramTypeID == 2);

            ViewBag.WorkProgram = DB.ProgramEvents.Count(x => x.ProgramTypeID == 1);

            ViewBag.MentalWellness = DB.ProgramEvents.Count(x => x.ProgramTypeID == 3);

            ViewBag.P2I = DB.ProgramEvents.Count(x => x.ProgramTypeID == 4);

            ViewBag.EmergencyShelter = DB.ProgramEvents.Count(x => x.ProgramTypeID == 5);

            ViewBag.SchoolProgram = DB.ProgramEvents.Count(x => x.ProgramTypeID == 6);

            ViewBag.VeteransCourt = DB.Residents.Count(x => x.InVetCourt == true);

            ViewBag.FinancialProgram = DB.ProgramEvents.Count(x => x.ProgramTypeID == 10);

            ViewBag.DepressionBehavioralProgram = DB.ProgramEvents.Count(x => x.ProgramTypeID == 11);

            ViewBag.SubstanceAbuseProgram = DB.ProgramEvents.Count(x => x.ProgramTypeID == 12);

            return View(); 
        }

        public ActionResult Historic()
        {
            return View();
        }

        public ActionResult DownloadData()
        {
            HistoricData HB = new HistoricData();

            

            var residents = DB.Residents;

            var myExport = new CsvExport();

            foreach (var Resident in residents)
            {
                myExport.AddRow();
                myExport["Last Name"] = Resident.LastName;
                myExport["First Name"] = Resident.FirstMidName;
                myExport["Birthdate"] = Resident.Birthdate;
                myExport["Service Branch"] = Resident.ServiceBranch;
                myExport["PTSD"] = Resident.HasPTSD;
                myExport["Vet Court"] = Resident.InVetCourt;
                myExport["Notes"] = Resident.Note;
                /*
                myExport["Work Program"] = wp;
                myExport["Mental Wellness"] = Resident.FirstMidName;
                myExport["P2I"] = Resident.Birthdate;
                myExport["Emergency Shelter"] = Resident.ServiceBranch;
                myExport["School Program"] = Resident.HasPTSD;
                myExport["Financial Program"] = Resident.InVetCourt;
                myExport["Readmitted"] = Resident.Note;
                myExport["Depression/Behavioral"] = Resident.Note;
                myExport["Substance Abuse"] = Resident.Note;
                myExport["Discharge for Cause"] = Resident.Note;
                myExport["Self Discharge"] = Resident.Note;
                myExport["Higher Level of Care"] = Resident.Note;
                */
            }

            string filepath = Server.MapPath(Url.Content("~/Content/CenterReport.csv"));

            myExport.ExportToFile(filepath);

            string filename = "~\\Content\\CenterReport.csv";

            return File(filename,"text/csv", "HistoricData.csv");
        }   
    }
}