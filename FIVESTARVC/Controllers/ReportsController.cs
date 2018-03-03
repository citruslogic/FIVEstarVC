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
using System.Web.Helpers;

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

            foreach (Resident item in navyQuery)
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
                if (ProgramEvents.ProgramTypeID == 2 //admission
                    || ProgramEvents.ProgramTypeID == 3 //re-admit
                    || ProgramEvents.ProgramTypeID == 1) //emergency shelter
                {
                    count++;
                    continue;
                }
                else if (ProgramEvents.ProgramTypeID == 4 //graduation
                    || ProgramEvents.ProgramTypeID == 5 //discharge
                    || ProgramEvents.ProgramTypeID == 6 //discharge
                    || ProgramEvents.ProgramTypeID == 7)//discharge
                {
                    count--;
                }

                if (ProgramEvents.ProgramTypeID == 5 //discharge
                    || ProgramEvents.ProgramTypeID == 6 //discharge
                    || ProgramEvents.ProgramTypeID == 7)
                {
                    dischargeCount++;
                }
            }

            ViewBag.TotalCount = count;
            ViewBag.DischargeCount = dischargeCount;

            //Finds graduation percent
            float Graduated = DB.ProgramEvents.Count(x => x.ProgramTypeID == 4);
            ViewBag.Graduated = Graduated;

            //Finds number admitted
            float Admitted = DB.ProgramEvents.Count(x => x.ProgramTypeID == 2);
            ViewBag.Admitted = Admitted;

            float gradPercent = (Graduated / Admitted) * 100;

            ViewBag.GraduatedPercent = gradPercent.ToString("n2");

            ViewBag.CumulativeCount = DB.Residents.Count();

            ViewBag.P2I = DB.ProgramEvents.Count(x => x.ProgramTypeID == 10);

            ViewBag.EmergencyShelter = DB.ProgramEvents.Count(x => x.ProgramTypeID == 1);

            ViewBag.VeteransCourt = DB.Residents.Count(x => x.InVetCourt == true);

            //Query to make a list of all admission and discharge events
            var avgProg = DB.ProgramEvents;

            var avgProgStay = (from r in avgProg
                               where r.ProgramTypeID == 1 || r.ProgramTypeID == 2 || r.ProgramTypeID == 3 ||
                                     r.ProgramTypeID == 4 || r.ProgramTypeID == 5 || r.ProgramTypeID == 6 || r.ProgramTypeID == 7
                               select new
                               {
                                   r.ResidentID,
                                   r.ProgramTypeID,
                                   r.StartDate,
                                   r.EndDate
                               }).ToList();

            //Variables to find average length of stay
            double total = 0;
            int numbCount = 0;
            double average = 0;
            double days = 0;

            foreach (var item in avgProgStay)
            {
                DateTime endDate;
                DateTime startDate;
                int resID;

                //Discharge or graduation events
                if (item.ProgramTypeID == 4 || item.ProgramTypeID == 5 || item.ProgramTypeID == 6 || item.ProgramTypeID == 7)
                {
                    endDate = item.StartDate; //Startdate of a discharge event is the "end date" in this sense
                    resID = item.ResidentID;
                    numbCount++;

                    foreach (var startItem in avgProgStay)
                    {
                        if (resID == startItem.ResidentID)
                        {
                            if (startItem.ProgramTypeID == 1 || startItem.ProgramTypeID == 2 || startItem.ProgramTypeID == 3)
                            {
                                startDate = startItem.StartDate;
                                days = (endDate - startDate).TotalDays;
                                total += days;
                            }
                        }
                    }
                }

                average = total / numbCount;
            }


            ViewBag.AvgStay = (int)average;

            return View();
        }

        public ActionResult Historic()
        {
            HistoricData dataModel = new HistoricData();

            



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
                //myExport["Age"] = r.First().
                myExport["Service Branch"] = r.First().ServiceBranch;
                myExport["Vet Court"] = r.First().InVetCourt;
                myExport["Notes"] = r.First().Note;



                var eventids = r.Select(i => i.ProgramTypeID).ToList();

                /* These Event IDs have changed, and the order of the columns 
                 * may not be what is expected.
                 * See CenterInitializer.cs for the ProgramTypeID order. 
                 * - Frank Butler
                 */
                foreach (var eid in eventids)
                {
                    switch (eid)
                    {
                        case 1:
                            myExport["Emergency Shelter"] = "1";
                            break;
                        case 2:
                            myExport["Resident Admission"] = "1";
                            break;
                        case 3:
                            myExport["Re-admit"] = "1";
                            break;
                        case 4:
                            myExport["Resident Graduation"] = "1";
                            break;
                        case 5:
                            myExport["Self Discharge"] = "1";
                            break;
                        case 6:
                            myExport["Discharge for Cause"] = "1";
                            break;
                        case 8:
                            myExport["Work Program"] = "1";
                            break;
                        case 9:
                            myExport["Mental Wellness"] = "1";
                            break;
                        case 10:
                            myExport["P2I"] = "1";
                            break;
                        case 11:
                            myExport["School Program"] = "1";
                            break;
                        case 12:
                            myExport["Financial Program"] = "1";
                            break;
                        case 13:
                            myExport["Depression / Behavioral Program"] = "1";
                            break;
                        case 14:
                            myExport["Substance Abuse Program"] = "1";
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

            return File(filename, "text/csv", "HistoricData.csv");
        }
       

        public ActionResult RenderCampaignGraph()
        {
            var BarChartData = from mc in DB.MilitaryCampaigns
                               select new
                               {
                                   MilitaryCampaign = mc.CampaignName,
                                   ResidentCount = mc.Residents.Count()
                               };

            var CampaignChart = new Chart(width: 600, height: 400)
                .AddTitle("Campaign Participants")
                .AddSeries(chartType: "bar", 
                            legend: "Campaign",
                            xValue: BarChartData.Select(cn => cn.MilitaryCampaign).ToArray(),
                            yValues: BarChartData.Select(c => c.ResidentCount).ToArray())
                            .Write();

            return null;
        }
    }

}