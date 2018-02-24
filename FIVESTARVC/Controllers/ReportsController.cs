using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using FIVESTARVC.DAL;
using FIVESTARVC.Models;
using FIVESTARVC.ViewModels;
using Jitbit.Utils;
using DelegateDecompiler;


namespace FIVESTARVC.Controllers
{
    public class ReportsController : Controller
    {
        private ResidentContext DB = new ResidentContext();

        // GET: Reports
        public ActionResult Index()
        {
            //Variables used in counting current residents
            int nvyCount = 0;
            int armyCount = 0;
            int marineCount = 0;
            int afCount = 0;
            int cgCount = 0;

            //Variables to count cumulative branch types
            ViewBag.NavyCount = DB.Residents.Count(x => x.ServiceBranch == ServiceType.NAVY);
            ViewBag.MarineCount = DB.Residents.Count(x => x.ServiceBranch == ServiceType.MARINES);
            ViewBag.ArmyCount = DB.Residents.Count(x => x.ServiceBranch == ServiceType.ARMY);
            ViewBag.AirForceCount = DB.Residents.Count(x => x.ServiceBranch == ServiceType.AIRFORCE);
            ViewBag.CoastGuardCount = DB.Residents.Count(x => x.ServiceBranch == ServiceType.COASTGUARD);

            //Count current Navy Residents
            var navyQuery = (from Resident in DB.Residents
                             where Resident.ServiceBranch == ServiceType.NAVY
                             select Resident).ToList();

            foreach(Resident item in navyQuery)
            {
                if (item.IsCurrent())
                {
                    ViewBag.CurrentNavy = ++nvyCount;
                }
            }

            //Count current Army Residents
            var armyQuery = (from Resident in DB.Residents
                             where Resident.ServiceBranch == ServiceType.ARMY
                             select Resident).ToList();

            foreach (Resident item in armyQuery)
            {
                if (item.IsCurrent())
                {
                    ViewBag.CurrentArmy = ++armyCount;
                }
            }

            //Count current Marine Residents
            var marineQuery = (from Resident in DB.Residents
                             where Resident.ServiceBranch == ServiceType.MARINES
                             select Resident).ToList();

            foreach (Resident item in marineQuery)
            {
                if (item.IsCurrent())
                {
                    ViewBag.CurrentMarine = ++marineCount;
                }
            }

            //Count current AirForce Residents
            var afQuery = (from Resident in DB.Residents
                           where Resident.ServiceBranch == ServiceType.AIRFORCE
                           select Resident).ToList();

            foreach (Resident item in afQuery)
            {
                if (item.IsCurrent())
                {
                    ViewBag.CurrentAF = ++afCount;
                }
            }

            //Count current CoastGuard Residents
            var cgQuery = (from Resident in DB.Residents
                           where Resident.ServiceBranch == ServiceType.COASTGUARD
                           select Resident).ToList();

            foreach (Resident item in cgQuery)
            {
                if (item.IsCurrent())
                {
                    ViewBag.CurrentCG = ++cgCount;
                }
            }

            //Counts number of current residents, based on events
            var CurrentRes = DB.ProgramEvents;
            int count = 0;
            int dischargeCount = 0;

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

                if (ProgramEvents.ProgramTypeID == 13 //discharge
                    || ProgramEvents.ProgramTypeID == 14 //discharge
                    || ProgramEvents.ProgramTypeID == 15)
                {
                    dischargeCount++;
                }
            }

            ViewBag.TotalCount = count;
            ViewBag.DischargeCount = dischargeCount;
            
            //Finds graduation percent
            float Graduated = DB.ProgramEvents.Count(x => x.ProgramTypeID == 2);
            ViewBag.Graduated = Graduated;

            //Finds number admitted
            float Admitted = DB.ProgramEvents.Count(x => x.ProgramTypeID == 7);
            ViewBag.Admitted = Admitted;

            float gradPercent = (Graduated / Admitted) * 100;
            
            ViewBag.GraduatedPercent = gradPercent.ToString("n2");

            //ViewBag.WorkProgram = DB.ProgramEvents.Count(x => x.ProgramTypeID == 1);

            //ViewBag.MentalWellness = DB.ProgramEvents.Count(x => x.ProgramTypeID == 3);

            ViewBag.P2I = DB.ProgramEvents.Count(x => x.ProgramTypeID == 4);

            ViewBag.EmergencyShelter = DB.ProgramEvents.Count(x => x.ProgramTypeID == 5);

            //ViewBag.SchoolProgram = DB.ProgramEvents.Count(x => x.ProgramTypeID == 6);

            ViewBag.VeteransCourt = DB.Residents.Count(x => x.InVetCourt == true);

            //ViewBag.FinancialProgram = DB.ProgramEvents.Count(x => x.ProgramTypeID == 10);

            return View(); 
        }

        public ActionResult Historic()
        {
            var yearlyEvents = DB.ProgramEvents;
            


            return View();
        }


        public ActionResult DownloadData()
            {
                var residents = DB.Residents;

                var residentProgramType = (from r in residents
                                          join pgm in DB.ProgramEvents on r.ResidentID equals pgm.ResidentID
                                      
                                         select new
                                         {
                                             r.ResidentID,
                                             r.FirstMidName,
                                             r.LastName,
                                             r.Birthdate,
                                             r.ServiceBranch,
                                             r.InVetCourt,
                                             r.Note,
                                             pgm.ProgramTypeID
                                         }).GroupBy(r => r.ResidentID).ToList();



            var myExport = new CsvExport();

                foreach (var r in residentProgramType)
                {
                    myExport.AddRow();
                    myExport["Last Name"] = r.First().LastName;
                    myExport["First Name"] = r.First().FirstMidName;
                    myExport["Birthdate"] = r.First().Birthdate;
                    //myExport["Age"] = r.First().Age;
                    myExport["Service Branch"] = r.First().ServiceBranch;
                    myExport["Vet Court"] = r.First().InVetCourt;
                    myExport["Notes"] = r.First().Note;



                    var eventids = r.Select(i => i.ProgramTypeID).ToList();


                foreach(var eid in eventids)
                {
                    switch(eid) {
                        case 1:
                            myExport["Work Program"] = "1";
                            break;
                        case 2:
                            break;
                        case 3:
                            myExport["Mental Wellness"] = "1";
                            break;
                        case 4:
                            myExport["P2I"] = "1";
                            break;
                        case 5:
                            myExport["Emergency Shelter"] = "1";
                            break;
                        case 6:
                            myExport["School Program"] = "1";
                            break;
                        case 8:
                            myExport["Re-Admit"] = "1";
                            break;
                        case 9:
                            myExport["Financial Program"] = "1";
                            break;
                        case 10:
                            myExport["Depression/Behavioral Program"] = "1";
                            break;
                        case 11:
                            myExport["Substance Abuse Program"] = "1";
                            break;
                        case 12:
                            myExport["Discharge for Cause"] = "1";
                            break;
                        case 13:
                            myExport["Self Discharge"] = "1";
                            break;
                        case 14:
                            myExport["Higher Level of Care"] = "1";
                            break;
                        default:
                            //do something
                            break;
                        
                    }   
                }

                }

                string filepath = Server.MapPath(Url.Content("~/Content/CenterReport.csv"));

                myExport.ExportToFile(filepath);

                string filename = "~\\Content\\CenterReport.csv";
            
                return File(filename,"text/csv", "HistoricData.csv");
            }
        //A very naughty method
    
        //public bool checkEvent(Resident res, int pgmType)
        //{
        //    var pgmEventCheck = db.ProgramEvents;

        //    foreach (var ProgramEvent in pgmEventCheck)
        //    {
        //        if (ProgramEvent.ResidentID == res.ResidentID)
        //        {

        //            if (ProgramEvent.ProgramTypeID == pgmType)
        //            {
        //                return true;
        //            }
        //        }
        //    }

        //    return false;
        //}
    }
}