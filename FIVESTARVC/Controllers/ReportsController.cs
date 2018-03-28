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
using System.Globalization;
using System.Threading;
using DotNet.Highcharts;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;
using System.Drawing;

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

            ViewBag.GraduatedPercent = gradPercent.ToString("n2"); //Graduation Percentage


            ViewBag.CumulativeCount = DB.Residents.Count();

            ViewBag.P2I = DB.ProgramEvents.Count(x => x.ProgramTypeID == 9);

            ViewBag.EmergencyShelter = DB.ProgramEvents.Count(x => x.ProgramTypeID == 1);

            ViewBag.VeteransCourt = DB.Residents.Count(x => x.InVetCourt == true);



            //Variables to find average length of stay
            int total = 0;
            //int numbCount = 0;
            //double average = 0;
            //double days = 0;

            var residents = DB.Residents.Include(p => p.ProgramEvents).ToList();

            foreach (Resident resident in residents)
            {
                total += resident.DaysInCenter();
            }

            ViewBag.AvgStay = total / residents.Count();
            /*
             * (p => p.ProgramTypeID == 4 || item.ProgramTypeID == 5 || item.ProgramTypeID == 6 || item.ProgramTypeID == 7) 
            //Discharge or graduation events
            if (item.
            {
                endDate = item.ClearStartDate; //Startdate of a discharge event is the "end date" in this sense
                resID = item.ResidentID;
                numbCount++;

                foreach (var startItem in avgProgStay)
                {
                    if (resID == startItem.ResidentID)
                    {
                        if (startItem.ProgramTypeID == 1 || startItem.ProgramTypeID == 2 || startItem.ProgramTypeID == 3)
                        {
                            startDate = startItem.ClearStartDate;
                            days = (endDate - startDate).TotalDays;
                            total += days;
                        }
                    }
                }
            }

            average = total / numbCount;
        }

            */


            return View();
        }
        /* Get the age of all residents that have been in the center. */
        public IEnumerable<ReportingResidentViewModel> GetResidentsAge()
        {

            IEnumerable<ReportingResidentViewModel> residentListing = DB.Residents.ToList()
                .Select(r => new ReportingResidentViewModel { ID = r.ResidentID, Age = r.Age.Computed() });

            return residentListing;
        }
        /* Get the average age of all residents that have been in the center.
         * If you want only the current residents, use IsCurrent() in a Where
           LINQ method. */
        public double GetAverageAge()
        {

            IEnumerable<ReportingResidentViewModel> residentListing = DB.Residents.ToList()
                .Select(r => new ReportingResidentViewModel { ID = r.ResidentID, Age = r.Age.Computed() });

            return residentListing.Average(r => r.Age);
        }

        public ActionResult Historic()
        {

            Highcharts columnChart = new Highcharts("columnchart");

            columnChart.InitChart(new Chart()
            {
                Type = DotNet.Highcharts.Enums.ChartTypes.Line,
                BackgroundColor = new BackColorOrGradient(System.Drawing.Color.AliceBlue),
                Style = "fontWeight: 'bold', fontSize: '17px'",
                BorderColor = System.Drawing.Color.LightBlue,
                BorderRadius = 0,
                BorderWidth = 2

            });

            columnChart.SetTitle(new Title()
            {
                Text = "Historic Data"
            });

            //columnChart.SetSubtitle(new Subtitle()
            //{
            //    Text = "Played 9 Years Together From 2004 To 2012"
            //});

            object[] P2I = p2iRates().Cast<object>().ToArray();
            object[] Admit = admittedRates().Cast<object>().ToArray();
            object[] reAdmit = readmittedRates().Cast<object>().ToArray();
            object[] Graduate = gradRates().Cast<object>().ToArray();
            object[] Emergency = emergRates().Cast<object>().ToArray();
            object[] Discharge = dischargeRates().Cast<object>().ToArray();
            object[] vetCourt = vetCourtRates().Cast<object>().ToArray();
            string[] years = gradYears().ToArray();

            columnChart.SetXAxis(new XAxis()
            {
                Type = AxisTypes.Category,
                Title = new XAxisTitle() { Text = "Years", Style = "fontWeight: 'bold', fontSize: '17px'" },
                Categories = years
            });

            columnChart.SetYAxis(new YAxis()
            {
                Title = new YAxisTitle()
                {
                    Text = "Number of Residents",
                    Style = "fontWeight: 'bold', fontSize: '17px'"
                },
                ShowFirstLabel = true,
                ShowLastLabel = true,
                Min = 0
            });

            columnChart.SetLegend(new Legend
            {
                Enabled = true,
                BorderColor = System.Drawing.Color.CornflowerBlue,
                BorderRadius = 6,
                BackgroundColor = new BackColorOrGradient(ColorTranslator.FromHtml("#FFADD8E6"))
            });

            columnChart.SetSeries(new Series[]
            {
                new Series{

                    Name = "P2I",
                    Data = new Data(P2I)
                },

                new Series{

                    Name = "Admitted",
                    Data = new Data(Admit)
                },

                new Series{

                    Name = "Re-Admitted",
                    Data = new Data(reAdmit)
                },

                new Series{

                    Name = "Graduated",
                    Data = new Data(Graduate)
                },

                new Series{

                    Name = "Emergency Shelter",
                    Data = new Data(Emergency)
                },

                new Series{

                    Name = "Discharged",
                    Data = new Data(Discharge)
                },

                new Series{

                    Name = "Vet Court",
                    Data = new Data(vetCourt)
                }

            }
            );

            return View(columnChart);
        }

        public ActionResult CumulativeHistoric()
        {

            Highcharts columnChart = new Highcharts("columnchart");

            columnChart.InitChart(new Chart()
            {
                Type = DotNet.Highcharts.Enums.ChartTypes.Line,
                BackgroundColor = new BackColorOrGradient(System.Drawing.Color.AliceBlue),
                Style = "fontWeight: 'bold', fontSize: '17px'",
                BorderColor = System.Drawing.Color.LightBlue,
                BorderRadius = 0,
                BorderWidth = 2

            });

            columnChart.SetTitle(new Title()
            {
                Text = "Cumulative Historic Data"
            });

            //columnChart.SetSubtitle(new Subtitle()
            //{
            //    Text = "Played 9 Years Together From 2004 To 2012"
            //});

            object[] P2I = p2iRatesCum().Cast<object>().ToArray();
            object[] Admit = admittedRatesCum().Cast<object>().ToArray();
            object[] reAdmit = readmittedRatesCum().Cast<object>().ToArray();
            object[] Graduate = gradRatesCum().Cast<object>().ToArray();
            object[] Emergency = emergRatesCum().Cast<object>().ToArray();
            object[] Discharge = dischargeRatesCum().Cast<object>().ToArray();
            object[] vetCourt = vetCourtRatesCum().Cast<object>().ToArray();
            string[] years = gradYears().ToArray();

            columnChart.SetXAxis(new XAxis()
            {
                Type = AxisTypes.Category,
                Title = new XAxisTitle() { Text = "Years", Style = "fontWeight: 'bold', fontSize: '17px'" },
                Categories = years
            });

            columnChart.SetYAxis(new YAxis()
            {
                Title = new YAxisTitle()
                {
                    Text = "Number of Residents",
                    Style = "fontWeight: 'bold', fontSize: '17px'"
                },
                ShowFirstLabel = true,
                ShowLastLabel = true,
                Min = 0
            });

            columnChart.SetLegend(new Legend
            {
                Enabled = true,
                BorderColor = System.Drawing.Color.CornflowerBlue,
                BorderRadius = 6,
                BackgroundColor = new BackColorOrGradient(ColorTranslator.FromHtml("#FFADD8E6"))
            });

            columnChart.SetSeries(new Series[]
            {
                new Series{

                    Name = "P2I",
                    Data = new Data(P2I)
                },

                new Series{

                    Name = "Admitted",
                    Data = new Data(Admit)
                },

                new Series{

                    Name = "Re-Admitted",
                    Data = new Data(reAdmit)
                },

                new Series{

                    Name = "Graduated",
                    Data = new Data(Graduate)
                },

                new Series{

                    Name = "Emergency Shelter",
                    Data = new Data(Emergency)
                },

                new Series{

                    Name = "Discharged",
                    Data = new Data(Discharge)
                },

                new Series{

                    Name = "Vet Court",
                    Data = new Data(vetCourt)
                }

            }
            );

            return View(columnChart);
        }

        public List<string> gradYears()
        {
            List<string> gradPercent = new List<string>();

            DateTime currentDate = DateTime.Now;

            int currentYear = currentDate.Year;

            int num = currentYear - 2011;

            int year = 2012;

            for (int x = 0; x < num; x++)
            {
                gradPercent.Add(year.ToString());
                year++;
            }

            gradPercent.ToString().ToArray();

            return gradPercent;
        }

        public List<int> gradRates()
        {
            var events = DB.ProgramEvents.ToList();

            var Query = (from y in events
                         where y.ProgramTypeID == 4
                         group y by y.ClearStartDate.Computed().Year into typeGroup
                         select new
                         {
                             StartDate = typeGroup.Key,
                             Count = typeGroup.Count(),
                         });

            List<int> rates = new List<int>();

            foreach (var t in Query)
            {
                rates.Add(t.Count);
            }

            return rates;
        }

        public List<int> gradRatesCum()
        {
            var events = DB.ProgramEvents.ToList();

            int runningTotal = 0;

            var Query = (from y in events
                         where y.ProgramTypeID == 4
                         group y by y.ClearStartDate.Computed().Year into typeGroup
                         select new
                         {
                             StartDate = typeGroup.Key,
                             Count = typeGroup.Count(),
                         });

            List<int> rates = new List<int>();

            foreach (var t in Query)
            {
                runningTotal = runningTotal + t.Count;

                rates.Add(runningTotal);
            }

            return rates;
        }

        public List<int> admittedRates()
        {
            var events = DB.ProgramEvents.ToList();

            var gradQuery = (from y in events
                             where y.ProgramTypeID == 2
                             group y by y.ClearStartDate.Computed().Year into typeGroup
                             select new
                             {
                                 StartDate = typeGroup.Key,
                                 Count = typeGroup.Count(),
                             });

            List<int> rates = new List<int>();

            foreach (var t in gradQuery)
            {
                rates.Add(t.Count);
            }

            return rates;
        }

        public List<int> admittedRatesCum()
        {
            var events = DB.ProgramEvents.ToList();

            int runningTotal = 0;

            var gradQuery = (from y in events
                             where y.ProgramTypeID == 2
                             group y by y.ClearStartDate.Computed().Year into typeGroup
                             select new
                             {
                                 StartDate = typeGroup.Key,
                                 Count = typeGroup.Count(),
                             });

            List<int> rates = new List<int>();

            foreach (var t in gradQuery)
            {
                runningTotal = runningTotal + t.Count;

                rates.Add(runningTotal);
            }

            return rates;
        }

        public List<int> readmittedRates()
        {
            var events = DB.ProgramEvents.ToList();

            var Query = (from y in events
                         where y.ProgramTypeID == 3
                         group y by y.ClearStartDate.Computed().Year into typeGroup
                         select new
                         {
                             StartDate = typeGroup.Key,
                             Count = typeGroup.Count(),
                         });

            List<int> rates = new List<int>();

            foreach (var t in Query)
            {
                rates.Add(t.Count);
            }

            return rates;
        }

        public List<int> readmittedRatesCum()
        {
            var events = DB.ProgramEvents.ToList();

            int runningTotal = 0;

            var Query = (from y in events
                         where y.ProgramTypeID == 3
                         group y by y.ClearStartDate.Computed().Year into typeGroup
                         select new
                         {
                             StartDate = typeGroup.Key,
                             Count = typeGroup.Count(),
                         });

            List<int> rates = new List<int>();

            foreach (var t in Query)
            {
                runningTotal = runningTotal + t.Count;

                rates.Add(runningTotal);
            }

            return rates;
        }

        public List<int> p2iRates()
        {
            var events = DB.ProgramEvents.ToList();

            int runningTotal = 0;

            var gradQuery = (from y in events
                             where y.ProgramTypeID == 9
                             group y by y.ClearStartDate.Computed().Year into typeGroup
                             select new
                             {
                                 StartDate = typeGroup.Key,
                                 Count = typeGroup.Count(),
                                 runningTotal = runningTotal + typeGroup.Count()
                             });

            List<int> rates = new List<int>();

            foreach (var t in gradQuery)
            {
                rates.Add(t.Count);
            }

            return rates;
        }

        public List<int> p2iRatesCum()
        {
            var events = DB.ProgramEvents.ToList();

            int runningTotal = 0;

            var gradQuery = (from y in events
                             where y.ProgramTypeID == 9
                             group y by y.ClearStartDate.Computed().Year into typeGroup
                             select new
                             {
                                 StartDate = typeGroup.Key,
                                 Count = typeGroup.Count(),
                                 runningTotal = runningTotal + typeGroup.Count()
                             });

            List<int> cumRates = new List<int>();

            foreach (var t in gradQuery)
            {
                runningTotal = runningTotal + t.Count;

                cumRates.Add(runningTotal);
            }

            return cumRates;
        }

        public List<int> emergRates()
        {
            var events = DB.ProgramEvents.ToList();

            var Query = (from y in events
                         where y.ProgramTypeID == 1
                         group y by y.ClearStartDate.Computed().Year into typeGroup
                         select new
                         {
                             StartDate = typeGroup.Key,
                             Count = typeGroup.Count(),
                         });

            List<int> rates = new List<int>();

            foreach (var t in Query)
            {
                rates.Add(t.Count);
            }

            return rates;
        }

        public List<int> emergRatesCum()
        {
            var events = DB.ProgramEvents.ToList();

            int runningTotal = 0;

            var Query = (from y in events
                         where y.ProgramTypeID == 1
                         group y by y.ClearStartDate.Computed().Year into typeGroup
                         select new
                         {
                             StartDate = typeGroup.Key,
                             Count = typeGroup.Count(),
                         });

            List<int> rates = new List<int>();

            foreach (var t in Query)
            {
                runningTotal = runningTotal + t.Count;

                rates.Add(runningTotal);
            }

            return rates;
        }

        public List<int> dischargeRates()
        {
            var events = DB.ProgramEvents.ToList();

            var Query = (from y in events
                         where y.ProgramTypeID == 5 || y.ProgramTypeID == 6 || y.ProgramTypeID == 7
                         group y by y.ClearStartDate.Computed().Year into typeGroup
                         select new
                         {
                             StartDate = typeGroup.Key,
                             Count = typeGroup.Count(),
                         });

            List<int> rates = new List<int>();

            foreach (var t in Query)
            {
                rates.Add(t.Count);
            }

            return rates;
        }

        public List<int> dischargeRatesCum()
        {
            var events = DB.ProgramEvents.ToList();

            int runningTotal = 0;

            var Query = (from y in events
                         where y.ProgramTypeID == 5 || y.ProgramTypeID == 6 || y.ProgramTypeID == 7
                         group y by y.ClearStartDate.Computed().Year into typeGroup
                         select new
                         {
                             StartDate = typeGroup.Key,
                             Count = typeGroup.Count(),
                         });

            List<int> rates = new List<int>();

            foreach (var t in Query)
            {
                runningTotal = runningTotal + t.Count;

                rates.Add(runningTotal);
            }

            return rates;
        }

        public List<int> vetCourtRates()
        {
            var events = DB.ProgramEvents.ToList();
            var queryResults = (from y in events
                                join resident in DB.Residents on y.ResidentID equals resident.ResidentID
                                where y.ProgramTypeID == 2 || y.ProgramTypeID == 1 || y.ProgramTypeID == 3 && resident.InVetCourt.Equals(true)
                                group y by y.ClearStartDate.Computed().Year into typeGroup
                                select new
                                {
                                    VetCourt = typeGroup.Count(),
                                    admtYear = typeGroup.Key
                                });

            List<int> rates = new List<int>();

            foreach (var r in queryResults)
            {
                rates.Add(r.VetCourt);
            }

            return rates;
        }

        public List<int> vetCourtRatesCum()
        {
            var events = DB.ProgramEvents.ToList();

            int runningTotal = 0;

            var queryResults = (from y in events
                                join resident in DB.Residents on y.ResidentID equals resident.ResidentID
                                where y.ProgramTypeID == 2 || y.ProgramTypeID == 1 || y.ProgramTypeID == 3 && resident.InVetCourt.Equals(true)
                                group y by y.ClearStartDate.Computed().Year into typeGroup
                                select new
                                {
                                    VetCourt = typeGroup.Count(),
                                    admtYear = typeGroup.Key
                                }).ToList();

            List<int> rates = new List<int>();

            foreach (var r in queryResults)
            {
                runningTotal = runningTotal + r.VetCourt;

                rates.Add(runningTotal);
            }

            return rates;
        }

        public ActionResult DownloadData()
        {
            var residents = DB.Residents;
            var programs = DB.ProgramTypes;

            var residentProgramType = DB.Residents.Include(p => p.ProgramEvents).ToList()
                .Select(r => new ReportingResidentViewModel
                {
                    ID = r.ResidentID,
                    FirstName = r.FirstMidName,
                    LastName = r.ClearLastName,
                    Birthdate = r.ClearBirthdate,
                    Age = r.Age.Computed(),
                    ServiceType = r.ServiceBranch,
                    InVetCourt = r.InVetCourt,
                    Note = r.Note,
                    ProgramTypeID = r.ProgramEvents.Select(t => t.ProgramTypeID.GetValueOrDefault())

                }).GroupBy(r => r.ID);




            var myExport = new CsvExport();

            foreach (var r in residentProgramType)
            {
                myExport.AddRow();
                myExport["Last Name"] = r.First().LastName;
                myExport["First Name"] = r.First().FirstName;
                myExport["Birthdate"] = r.First().Birthdate;
                myExport["Age"] = r.First().Age;
                myExport["Service Branch"] = FSEnumHelper.GetDescription(r.First().ServiceType);
                myExport["Vet Court"] = r.First().InVetCourt;
                myExport["Notes"] = r.First().Note;



                var eventids = r.SelectMany(i => i.ProgramTypeID).ToList();

                /* (from Resident in DB.Residents
                           where Resident.ServiceBranch == ServiceType.COASTGUARD
                           select Resident).ToList();
                 */
                foreach (var eid in eventids)
                {
                    int eventID = eid;

                    var prgm = (from p in DB.ProgramTypes
                                          where p.ProgramTypeID == eventID
                                          select p.ProgramDescription).ToArray();

                    int testVar = 1;

                    if (prgm.Length < testVar )
                    {
                        break;
                    }

                    String programName = prgm[0];


                    myExport[programName.ToString()] = "1";
                    //switch (eid)
                    //{
                    //    case 1:
                    //        myExport["Emergency Shelter"] = "1";
                    //        break;
                    //    case 2:
                    //        myExport["Resident Admission"] = "1";
                    //        break;
                    //    case 3:
                    //        myExport["Re-admit"] = "1";
                    //        break;
                    //    case 4:
                    //        myExport["Resident Graduation"] = "1";
                    //        break;
                    //    case 5:
                    //        myExport["Self Discharge"] = "1";
                    //        break;
                    //    case 6:
                    //        myExport["Discharge for Cause"] = "1";
                    //        break;
                    //    case 7:
                    //        myExport["Higher Level of Care"] = "1";
                    //        break;
                    //    case 8:
                    //        myExport["Work Program"] = "1";
                    //        break;
                    //    case 9:
                    //        myExport["P2I"] = "1";
                    //        break;
                    //    case 10:
                    //        myExport["School Program"] = "1";
                    //        break;
                    //    case 11:
                    //        myExport["Financial Program"] = "1";
                    //        break;
                    //    case 12:
                    //        myExport["Substance Abuse Program"] = "1";
                    //        break;
                    //    default:
                    //        //do something
                    //        break;

                //}
                }

            }

            string filepath = Server.MapPath(Url.Content("~/Content/CenterReport.csv"));

            myExport.ExportToFile(filepath);

            string filename = "~\\Content\\CenterReport.csv";

            return File(filename, "text/csv", "HistoricData.csv");
        }


        public ActionResult campaigns()
        {
            var BarChartData = from mc in DB.MilitaryCampaigns
                               select new
                               {
                                   MilitaryCampaign = mc.CampaignName,
                                   ResidentCount = mc.Residents.Count()
                               };

            List<string> campaignNames = new List<string>();
            List<int> resCounts = new List<int>();

            foreach (var x in BarChartData)
            {
                campaignNames.Add(x.MilitaryCampaign);
            }

            foreach (var y in BarChartData)
            {
                resCounts.Add(y.ResidentCount);
            }

            Highcharts columnChart = new Highcharts("columnchart");

            columnChart.InitChart(new Chart()
            {
                Type = DotNet.Highcharts.Enums.ChartTypes.Bar,
                BackgroundColor = new BackColorOrGradient(System.Drawing.Color.AliceBlue),
                Style = "fontWeight: 'bold', fontSize: '17px'",
                BorderColor = System.Drawing.Color.LightBlue,
                BorderRadius = 0,
                BorderWidth = 2

            });

            columnChart.SetTitle(new Title()
            {
                Text = "Resident Campaign Data"
            });

            //columnChart.SetSubtitle(new Subtitle()
            //{
            //    Text = "Played 9 Years Together From 2004 To 2012"
            //});
            object[] residentCount = resCounts.Cast<object>().ToArray();
            string[] Campaigns = campaignNames.ToArray();

            columnChart.SetXAxis(new XAxis()
            {
                Type = AxisTypes.Category,
                Title = new XAxisTitle() { Text = "Campaign", Style = "fontWeight: 'bold', fontSize: '17px'" },
                Categories = Campaigns
            });

            columnChart.SetYAxis(new YAxis()
            {
                Title = new YAxisTitle()
                {
                    Text = "Number of Residents",
                    Style = "fontWeight: 'bold', fontSize: '17px'"
                },
                ShowFirstLabel = true,
                ShowLastLabel = true,
                Min = 0
            });

            columnChart.SetLegend(new Legend
            {
                Enabled = true,
                BorderColor = System.Drawing.Color.CornflowerBlue,
                BorderRadius = 6,
                BackgroundColor = new BackColorOrGradient(ColorTranslator.FromHtml("#FFADD8E6"))
            });

            columnChart.SetSeries(new Series[]
            {
                new Series{

                    Name = "Residents",
                    Data = new Data(residentCount)
                },



            }
            );

            return View(columnChart);

        }



    }
}