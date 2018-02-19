﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
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
        private ResidentContext db = new ResidentContext();

        // GET: Reports
        public ActionResult Index()
        {
            //Variables to count branch types
            ViewBag.NavyCount = DB.Residents.Count(x => x.ServiceBranch == ServiceType.ARMY);
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

        public ActionResult Historic(int year)
        {
            var yearlyEvents = DB.ProgramEvents;
            

            return View(year);
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
                                             r.HasPTSD,
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
                    myExport["Service Branch"] = r.First().ServiceBranch;
                    myExport["PTSD"] = r.First().HasPTSD;
                    myExport["Vet Court"] = r.First().InVetCourt;



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
        //Not a very good method
    
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