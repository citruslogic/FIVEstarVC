using DelegateDecompiler;
using dotless.Core.Parser.Infrastructure;
using DotNet.Highcharts;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;
using FIVESTARVC.DAL;
using FIVESTARVC.Models;
using FIVESTARVC.Services;
using FIVESTARVC.ViewModels;
using Jitbit.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FIVESTARVC.Controllers
{
    /*
     * ~~~~~~~~~~READ ME~~~~~~~~~~~~
     * 
     * This is the reports controller which handles most of the counts of data, graphs, and general reports
     * found in the application (most of which in the 'Reports' tab). This controller can look like a mess
     * when first opened so I would reccomend hitting Ctrl+M+O if you are using visual studio to collapse all
     * the action results and make the controller easier to navigate. My general organizational scheme is as follows:
     * The index action result is where almost all of the aggregate counts are for things like current residents or
     * event counts. After that action result (going top to bottom), is two methods for finding an individual resident's
     * age and the average residents age. Following this is all the action results I used to create the charts that are 
     * found on the reports page. The name of the action results indicates if they are cumulative or not. Each chart has
     * a regular and cumulative version. Following these action results is the DownloadData() method which handles exporting
     * the residents database to an excel compatible file. After this is the last action result which returns a chart
     * that displays current residents campaign participation data.
     * 
     * You'll notice that all the queries looking for a certain event type (graduation, admission, work program, etc.)
     * are looking for specific event ID's rather than names of events. This can make findinig events annoying if
     * those ID to Event Name pairings aren't known. To find a list of these look at the ProgramType table.
     * 
     * There will be some sparse comments spread out throughout this controller following this one, but this should
     * serve to inform anyone in the future what each of these action results are for. Namely, the first two chart action
     * results, gradRates() and gradRatesCum(), will be commented to explain how these action results work. All the following
     * chart action results will be uncommented but work the same (only differences being the database queries and chart names).
     * 
     * Last edited 4/19/2018
     */

    [Authorize]
    public class ReportsController : Controller
    {
        private readonly ResidentContext DB = new ResidentContext();

        // GET: Reports
        [HttpGet]
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
                if (item.IsCurrent)
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
                if (item.IsCurrent)
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
                if (item.IsCurrent)
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
                if (item.IsCurrent)
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
                if (item.IsCurrent)
                {
                    ViewBag.CurrentCG = ++cgCount;
                }
            }

            //Counts number of residents by increasing count by 1 for every admit event, and decreasing for any discharge event
            double DischargeCount = 0.0;

            int TotalCount; // Current Resident Count
            ViewBag.TotalCount = TotalCount = DB.Residents.AsNoTracking().ToList().Where(cur => cur.IsCurrent).Count();

            //Helps find graduation percent - see eligibleDischarge below.
            var admittedResidents = DB.ProgramEvents.Include(i => i.ProgramType).Where(i => i.ProgramType.ProgramDescription.Equals("Resident Admission", StringComparison.InvariantCultureIgnoreCase)).Count();
            var emergencyShelterResidents = DB.ProgramEvents.Include(i => i.ProgramType).Where(i => i.ProgramType.ProgramDescription.Equals("Emergency Shelter", StringComparison.InvariantCultureIgnoreCase)).Count();

            int Graduated;
            ViewBag.Graduated = Graduated = DB.Database.SqlQuery<int>(@"select distinct
                                                                            pe.ResidentID 
                                                                            from ProgramEvent pe
                                                                            where ProgramTypeId = '4' 
	                                                                        ").Count();

            //var Graduated = DB.ProgramEvents.ToList().Count(i => i.ProgramTypeID == 4);

            int SelfDischarge;
            ViewBag.SelfDischarge = SelfDischarge = DB.Database.SqlQuery<int>(@"select distinct
                                                                                    pe.ResidentID 
                                                                                    from ProgramEvent pe
                                                                                    where ProgramTypeId = '5' 
	                                                                                ").Count();

            int DischargeForCause;
            ViewBag.DischargeForCause = DischargeForCause = DB.Database.SqlQuery<int>(@"select distinct
                                                                                        pe.ResidentID 
                                                                                        from ProgramEvent pe
                                                                                        where ProgramTypeId = '6' 
	                                                                                    ").Count();

            int DischargeHigherLevelOfCare;
            ViewBag.DischargeHigherLevelOfCare = DischargeHigherLevelOfCare = DB.Database.SqlQuery<int>(@"select distinct
                                                                                        pe.ResidentID 
                                                                                        from ProgramEvent pe
                                                                                        where ProgramTypeId = '7' 
	                                                                                    ").Count();

            int EmergencyDischarge;
            ViewBag.EmergencyDischarge = EmergencyDischarge = DB.Database.SqlQuery<int>(@"select distinct
                                                                                        pe.ResidentID 
                                                                                        from ProgramEvent pe
                                                                                        where ProgramTypeId = '13' 
	                                                                                    ").Count();

            ViewBag.DischargeCount = DischargeCount = Graduated + SelfDischarge + DischargeForCause + DischargeHigherLevelOfCare + EmergencyDischarge;

            //Finds number admitted
            //Counts cumulative residents
            int cumulativeResidents;
            ViewBag.CumulativeCount = cumulativeResidents = DB.Residents.Count();
            ViewBag.MinusHLCEmShelCurrent = cumulativeResidents - DischargeHigherLevelOfCare - emergencyShelterResidents - TotalCount;

            ViewBag.Admitted = cumulativeResidents;

            // actual residents who are eligible discharges - refer to notes for the proper count formula Inga used. 
            double eligibleDischarges = admittedResidents - DischargeHigherLevelOfCare - emergencyShelterResidents - TotalCount;


            if (ViewBag.CumulativeCount > 0)
            {
                //finds grad percent
                ViewBag.GraduatedPercent = (Graduated / eligibleDischarges * 100).ToString("0.##"); ; //Graduation Percentage
                ViewBag.SelfDischargePercent = (SelfDischarge / DischargeCount * 100).ToString("0.##");
                ViewBag.DischargeForCausePercent = (DischargeForCause / DischargeCount * 100).ToString("0.##");
                ViewBag.DischargeHigherLevelOfCarePercent = (DischargeHigherLevelOfCare / DischargeCount * 100).ToString("0.##");
                ViewBag.EmergencyDischargePercent = (EmergencyDischarge / DischargeCount * 100).ToString("0.##");
            }
            else
            {
                ViewBag.GraduatedPercent = 0;
            }

            //Finds cumulative P2I count
            ViewBag.P2I = DB.ProgramEvents.Count(x => x.ProgramTypeID == 10);

            //Finds cumulative emergency shelter counts
            ViewBag.EmergencyShelter = DB.ProgramEvents.Count(x => x.ProgramTypeID == 1);

            //Finds cumulative veterans court counts
            ViewBag.VeteransCourt = DB.Residents.Count(x => x.InVetCourt == true);

            return View();
        }

        [HttpGet]
        public ActionResult gradRates()
        {
            var events = DB.ProgramEvents.ToList();

            //Query selects all events with an ID of 4 (graduation events) and groups them by year. Results stored in 'Query' variable
            var Query = (from y in events
                         where y.ProgramTypeID == 4
                         group y by y.ClearStartDate.Year into typeGroup
                         orderby typeGroup.Key ascending
                         select new ChartData
                         {
                             Year = typeGroup.Key,
                             Count = typeGroup.Count(),
                         }).ToList();

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
                Text = "Graduations by Year"
            });

            //Sending query results to separate lists, then back to arrays, as seen below was how I was able to format the data in a way HighCharts would accept
            List<int> metricCounts = new List<int>();
            List<string> years = new List<string>();

            //This loop takes the data from the query result and sends it to two lists for the X axis and Y axis 
            foreach (var r in Query)
            {
                metricCounts.Add(r.Count);
                years.Add(r.Year.ToString());
            }

            string[] chartYears = years.Cast<string>().ToArray();
            object[] metric = metricCounts.Cast<object>().ToArray();

            columnChart.SetXAxis(new XAxis()
            {
                Type = AxisTypes.Category,
                Title = new XAxisTitle() { Text = "Years", Style = "fontWeight: 'bold', fontSize: '17px'" },
                Categories = chartYears
            });

            columnChart.SetYAxis(new YAxis()
            {
                Title = new YAxisTitle()
                {
                    Text = "Graduations",
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

                    Name = "Graduated",
                    Data = new Data(metric)
                },

            }
            );

            return View("DisplayChart", columnChart);


        }

        [HttpGet]
        public ActionResult gradRatesCum()
        {
            var events = DB.ProgramEvents.ToList();

            //Cumulative charts work the same, but this running total variable is used to keep counts cumulatively
            int runningTotal = 0;

            var Query = (from y in events
                         where y.ProgramTypeID == 4
                         group y by y.ClearStartDate.Year into typeGroup
                         orderby typeGroup.Key ascending
                         select new ChartData
                         {
                             Year = typeGroup.Key,
                             Count = typeGroup.Count(),
                         });

            List<int> rates = new List<int>();

            //Right here is where the running total is calculated and added to a list
            foreach (var t in Query)
            {
                runningTotal = runningTotal + t.Count;

                rates.Add(runningTotal);
            }

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
                Text = "Cumulative Graduations by Year"
            });

            //Years are pulled from the query result for a list that will be the X axis
            List<string> years = new List<string>();

            foreach (var r in Query)
            {
                years.Add(r.Year.ToString());
            }

            string[] chartYears = years.Cast<string>().ToArray();
            object[] metric = rates.Cast<object>().ToArray();

            columnChart.SetXAxis(new XAxis()
            {
                Type = AxisTypes.Category,
                Title = new XAxisTitle() { Text = "Years", Style = "fontWeight: 'bold', fontSize: '17px'" },
                Categories = chartYears
            });

            columnChart.SetYAxis(new YAxis()
            {
                Title = new YAxisTitle()
                {
                    Text = "Graduations",
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

                    Name = "Graduated",
                    Data = new Data(metric)
                },

            }
            );

            return View("DisplayChart", columnChart);
        }

        [HttpGet]
        public ActionResult admittedRates()
        {
            var events = DB.ProgramEvents.ToList();

            var Query = (from y in events
                         where y.ProgramTypeID == 2
                         group y by y.ClearStartDate.Year into typeGroup
                         orderby typeGroup.Key ascending
                         select new ChartData
                         {
                             Year = typeGroup.Key,
                             Count = typeGroup.Count(),
                         });

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
                Text = "Admissions by Year"
            });

            List<int> metricCounts = new List<int>();
            List<string> years = new List<string>();

            foreach (var r in Query)
            {
                metricCounts.Add(r.Count);
                years.Add(r.Year.ToString());
            }

            string[] chartYears = years.Cast<string>().ToArray();
            object[] metric = metricCounts.Cast<object>().ToArray();

            columnChart.SetXAxis(new XAxis()
            {
                Type = AxisTypes.Category,
                Title = new XAxisTitle() { Text = "Years", Style = "fontWeight: 'bold', fontSize: '17px'" },
                Categories = chartYears
            });

            columnChart.SetYAxis(new YAxis()
            {
                Title = new YAxisTitle()
                {
                    Text = "Admissions",
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

                    Name = "Admitted",
                    Data = new Data(metric)
                },

            }
            );

            return View("DisplayChart", columnChart);
        }

        [HttpGet]
        public ActionResult admittedRatesCum()
        {
            var events = DB.ProgramEvents.ToList();

            int runningTotal = 0;

            var gradQuery = (from y in events
                             where y.ProgramTypeID == 2
                             group y by y.ClearStartDate.Year into typeGroup
                             orderby typeGroup.Key ascending
                             select new ChartData
                             {
                                 Year = typeGroup.Key,
                                 Count = typeGroup.Count(),
                             });

            List<int> rates = new List<int>();

            foreach (var t in gradQuery)
            {
                runningTotal = runningTotal + t.Count;

                rates.Add(runningTotal);
            }

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
                Text = "Cumulative Admissions by Year"
            });

            List<int> metricCounts = new List<int>();
            List<string> years = new List<string>();

            foreach (var r in gradQuery)
            {
                //metricCounts.Add(r.Count);
                years.Add(r.Year.ToString());
            }

            string[] chartYears = years.Cast<string>().ToArray();
            object[] metric = rates.Cast<object>().ToArray();

            columnChart.SetXAxis(new XAxis()
            {
                Type = AxisTypes.Category,
                Title = new XAxisTitle() { Text = "Years", Style = "fontWeight: 'bold', fontSize: '17px'" },
                Categories = chartYears
            });

            columnChart.SetYAxis(new YAxis()
            {
                Title = new YAxisTitle()
                {
                    Text = "Admissions",
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

                    Name = "Admitted",
                    Data = new Data(metric)
                },

            }
            );

            return View("DisplayChart", columnChart);
        }

        [HttpGet]
        public ActionResult readmittedRates()
        {
            var events = DB.ProgramEvents.ToList();

            var Query = (from y in events
                         where y.ProgramTypeID == 3
                         group y by y.ClearStartDate.Year into typeGroup
                         orderby typeGroup.Key ascending
                         select new ChartData
                         {
                             Year = typeGroup.Key,
                             Count = typeGroup.Count(),
                         });

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
                Text = "Re-Admissions by Year"
            });

            List<int> metricCounts = new List<int>();
            List<string> years = new List<string>();

            foreach (var r in Query)
            {
                metricCounts.Add(r.Count);
                years.Add(r.Year.ToString());
            }

            string[] chartYears = years.Cast<string>().ToArray();
            object[] metric = metricCounts.Cast<object>().ToArray();

            columnChart.SetXAxis(new XAxis()
            {
                Type = AxisTypes.Category,
                Title = new XAxisTitle() { Text = "Years", Style = "fontWeight: 'bold', fontSize: '17px'" },
                Categories = chartYears
            });

            columnChart.SetYAxis(new YAxis()
            {
                Title = new YAxisTitle()
                {
                    Text = "Re-Admissions",
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

                    Name = "Re-Admitted",
                    Data = new Data(metric)
                },

            }
            );

            return View("DisplayChart", columnChart);
        }

        [HttpGet]
        public ActionResult readmittedRatesCum()
        {
            var events = DB.ProgramEvents.ToList();

            int runningTotal = 0;

            var Query = (from y in events
                         where y.ProgramTypeID == 3
                         group y by y.ClearStartDate.Year into typeGroup
                         orderby typeGroup.Key ascending
                         select new ChartData
                         {
                             Year = typeGroup.Key,
                             Count = typeGroup.Count(),
                         });

            List<int> rates = new List<int>();

            foreach (var t in Query)
            {
                runningTotal = runningTotal + t.Count;

                rates.Add(runningTotal);
            }

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
                Text = "Cumulative Re-Admissions by Year"
            });

            //List<int> metricCounts = new List<int>();
            List<string> years = new List<string>();

            foreach (var r in Query)
            {
                //metricCounts.Add(r.Count);
                years.Add(r.Year.ToString());
            }

            string[] chartYears = years.Cast<string>().ToArray();
            object[] metric = rates.Cast<object>().ToArray();

            columnChart.SetXAxis(new XAxis()
            {
                Type = AxisTypes.Category,
                Title = new XAxisTitle() { Text = "Years", Style = "fontWeight: 'bold', fontSize: '17px'" },
                Categories = chartYears
            });

            columnChart.SetYAxis(new YAxis()
            {
                Title = new YAxisTitle()
                {
                    Text = "Re-Admissions",
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

                    Name = "Re-Admitted",
                    Data = new Data(metric)
                },

            }
            );

            return View("DisplayChart", columnChart);
        }

        [HttpGet]
        public ActionResult p2iRates()
        {
            var events = DB.ProgramEvents.ToList();

            var gradQuery = (from y in events
                             where y.ProgramTypeID == 10
                             group y by y.ClearStartDate.Year into typeGroup
                             orderby typeGroup.Key ascending
                             select new ChartData
                             {
                                 Year = typeGroup.Key,
                                 Count = typeGroup.Count(),
                             });

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
                Text = "P2I Residents by Year"
            });

            List<int> metricCounts = new List<int>();
            List<string> years = new List<string>();

            foreach (var r in gradQuery)
            {
                metricCounts.Add(r.Count);
                years.Add(r.Year.ToString());
            }

            string[] chartYears = years.Cast<string>().ToArray();
            object[] metric = metricCounts.Cast<object>().ToArray();

            columnChart.SetXAxis(new XAxis()
            {
                Type = AxisTypes.Category,
                Title = new XAxisTitle() { Text = "Years", Style = "fontWeight: 'bold', fontSize: '17px'" },
                Categories = chartYears
            });

            columnChart.SetYAxis(new YAxis()
            {
                Title = new YAxisTitle()
                {
                    Text = "P2I",
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
                    Data = new Data(metric)
                },

            }
            );

            return View("DisplayChart", columnChart);
        }

        [HttpGet]
        public ActionResult p2iRatesCum()
        {
            var events = DB.ProgramEvents.ToList();

            int runningTotal = 0;

            var gradQuery = (from y in events
                             where y.ProgramTypeID == 10
                             group y by y.ClearStartDate.Year into typeGroup
                             orderby typeGroup.Key ascending
                             select new ChartData
                             {
                                 Year = typeGroup.Key,
                                 Count = typeGroup.Count(),
                             });

            List<int> rates = new List<int>();

            foreach (var t in gradQuery)
            {
                runningTotal = runningTotal + t.Count;

                rates.Add(runningTotal);
            }

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
                Text = "Cumulative P2I Residents by Year"
            });

            //List<int> metricCounts = new List<int>();
            List<string> years = new List<string>();

            foreach (var r in gradQuery)
            {
                //metricCounts.Add(r.Count);
                years.Add(r.Year.ToString());
            }

            string[] chartYears = years.Cast<string>().ToArray();
            object[] metric = rates.Cast<object>().ToArray();

            columnChart.SetXAxis(new XAxis()
            {
                Type = AxisTypes.Category,
                Title = new XAxisTitle() { Text = "Years", Style = "fontWeight: 'bold', fontSize: '17px'" },
                Categories = chartYears
            });

            columnChart.SetYAxis(new YAxis()
            {
                Title = new YAxisTitle()
                {
                    Text = "P2I",
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
                    Data = new Data(metric)
                },

            }
            );

            return View("DisplayChart", columnChart);
        }

        public ActionResult emergRates()
        {
            var events = DB.ProgramEvents.ToList();

            var Query = (from y in events
                         where y.ProgramTypeID == 1
                         group y by y.ClearStartDate.Year into typeGroup
                         orderby typeGroup.Key ascending
                         select new ChartData
                         {
                             Year = typeGroup.Key,
                             Count = typeGroup.Count(),
                         });

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
                Text = "Emergency Shelters by Year"
            });

            List<int> metricCounts = new List<int>();
            List<string> years = new List<string>();

            foreach (var r in Query)
            {
                metricCounts.Add(r.Count);
                years.Add(r.Year.ToString());
            }

            string[] chartYears = years.Cast<string>().ToArray();
            object[] metric = metricCounts.Cast<object>().ToArray();

            columnChart.SetXAxis(new XAxis()
            {
                Type = AxisTypes.Category,
                Title = new XAxisTitle() { Text = "Years", Style = "fontWeight: 'bold', fontSize: '17px'" },
                Categories = chartYears
            });

            columnChart.SetYAxis(new YAxis()
            {
                Title = new YAxisTitle()
                {
                    Text = "Emergency Shelters",
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

                    Name = "Emergency Shelter",
                    Data = new Data(metric)
                },

            }
            );

            return View("DisplayChart", columnChart);
        }

        public ActionResult emergRatesCum()
        {
            var events = DB.ProgramEvents.ToList();

            int runningTotal = 0;

            var Query = (from y in events
                         where y.ProgramTypeID == 1
                         group y by y.ClearStartDate.Year into typeGroup
                         orderby typeGroup.Key ascending
                         select new ChartData
                         {
                             Year = typeGroup.Key,
                             Count = typeGroup.Count(),
                         });

            List<int> rates = new List<int>();

            foreach (var t in Query)
            {
                runningTotal = runningTotal + t.Count;

                rates.Add(runningTotal);
            }

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
                Text = "Cumulative Emergency Shelters by Year"
            });

            //List<int> metricCounts = new List<int>();
            List<string> years = new List<string>();

            foreach (var r in Query)
            {
                //metricCounts.Add(r.Count);
                years.Add(r.Year.ToString());
            }

            string[] chartYears = years.Cast<string>().ToArray();
            object[] metric = rates.Cast<object>().ToArray();

            columnChart.SetXAxis(new XAxis()
            {
                Type = AxisTypes.Category,
                Title = new XAxisTitle() { Text = "Years", Style = "fontWeight: 'bold', fontSize: '17px'" },
                Categories = chartYears
            });

            columnChart.SetYAxis(new YAxis()
            {
                Title = new YAxisTitle()
                {
                    Text = "Emergency Shelters",
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

                    Name = "Emergency Shelter",
                    Data = new Data(metric)
                },

            }
            );

            return View("DisplayChart", columnChart);
        }

        public ActionResult dischargeRates()
        {
            var events = DB.ProgramEvents.ToList();

            var Query = (from y in events
                         where y.ProgramType.EventType == EnumEventType.DISCHARGE
                         group y by y.ClearStartDate.Year into typeGroup
                         orderby typeGroup.Key ascending
                         select new ChartData
                         {
                             Year = typeGroup.Key,
                             Count = typeGroup.Count(),
                         });

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
                Text = "Discharges by Year"
            });

            List<int> metricCounts = new List<int>();
            List<string> years = new List<string>();

            foreach (var r in Query)
            {
                metricCounts.Add(r.Count);
                years.Add(r.Year.ToString());
            }

            string[] chartYears = years.Cast<string>().ToArray();
            object[] metric = metricCounts.Cast<object>().ToArray();

            columnChart.SetXAxis(new XAxis()
            {
                Type = AxisTypes.Category,
                Title = new XAxisTitle() { Text = "Years", Style = "fontWeight: 'bold', fontSize: '17px'" },
                Categories = chartYears
            });

            columnChart.SetYAxis(new YAxis()
            {
                Title = new YAxisTitle()
                {
                    Text = "Discharges",
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

                    Name = "Discharge",
                    Data = new Data(metric)
                },

            }
            );

            return View("DisplayChart", columnChart);
        }

        public ActionResult dischargeRatesCum()
        {
            var events = DB.ProgramEvents.ToList();

            int runningTotal = 0;

            var Query = (from y in events
                         where y.ProgramType.EventType == EnumEventType.DISCHARGE
                         group y by y.ClearStartDate.Year into typeGroup
                         orderby typeGroup.Key ascending
                         select new ChartData
                         {
                             Year = typeGroup.Key,
                             Count = typeGroup.Count(),
                         });

            List<int> rates = new List<int>();

            foreach (var t in Query)
            {
                runningTotal = runningTotal + t.Count;

                rates.Add(runningTotal);
            }

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
                Text = "Cumulative Discharges by Year"
            });

            //List<int> metricCounts = new List<int>();
            List<string> years = new List<string>();

            foreach (var r in Query)
            {
                //metricCounts.Add(r.Count);
                years.Add(r.Year.ToString());
            }

            string[] chartYears = years.Cast<string>().ToArray();
            object[] metric = rates.Cast<object>().ToArray();

            columnChart.SetXAxis(new XAxis()
            {
                Type = AxisTypes.Category,
                Title = new XAxisTitle() { Text = "Years", Style = "fontWeight: 'bold', fontSize: '17px'" },
                Categories = chartYears
            });

            columnChart.SetYAxis(new YAxis()
            {
                Title = new YAxisTitle()
                {
                    Text = "Discharges",
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

                    Name = "Discharges",
                    Data = new Data(metric)
                },

            }
            );

            return View("DisplayChart", columnChart);
        }

        public ActionResult vetCourtRates()
        {
            var events = DB.ProgramEvents.ToList();

            var Query = (from y in events
                         join resident in DB.Residents on y.ResidentID equals resident.ResidentID
                         where y.ProgramType.EventType == EnumEventType.ADMISSION && resident.InVetCourt.Equals(true)
                         group y by y.ClearStartDate.Year into typeGroup
                         orderby typeGroup.Key ascending
                         select new ChartData
                         {
                             Count = typeGroup.Count(),
                             Year = typeGroup.Key
                         });

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
                Text = "VetCourt Residents by Year"
            });

            List<int> metricCounts = new List<int>();
            List<string> years = new List<string>();

            foreach (var r in Query)
            {
                metricCounts.Add(r.Count);
                years.Add(r.Year.ToString());
            }

            string[] chartYears = years.Cast<string>().ToArray();
            object[] metric = metricCounts.Cast<object>().ToArray();

            columnChart.SetXAxis(new XAxis()
            {
                Type = AxisTypes.Category,
                Title = new XAxisTitle() { Text = "Years", Style = "fontWeight: 'bold', fontSize: '17px'" },
                Categories = chartYears
            });

            columnChart.SetYAxis(new YAxis()
            {
                Title = new YAxisTitle()
                {
                    Text = "VetCourt",
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

                    Name = "VetCourt",
                    Data = new Data(metric)
                },

            }
            );

            return View("DisplayChart", columnChart);
        }

        public ActionResult vetCourtRatesCum()
        {
            var events = DB.ProgramEvents.ToList();

            int runningTotal = 0;

            var queryResults = (from y in events
                                join resident in DB.Residents on y.ResidentID equals resident.ResidentID
                                where y.ProgramType.EventType == EnumEventType.ADMISSION && resident.InVetCourt.Equals(true)
                                group y by y.ClearStartDate.Year into typeGroup
                                orderby typeGroup.Key ascending
                                select new ChartData
                                {
                                    Count = typeGroup.Count(),
                                    Year = typeGroup.Key
                                }).ToList();

            List<int> rates = new List<int>();

            foreach (var r in queryResults)
            {
                runningTotal = runningTotal + r.Count;

                rates.Add(runningTotal);
            }

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
                Text = "Cumulative VetCourt Residents by Year"
            });

            //List<int> metricCounts = new List<int>();
            List<string> years = new List<string>();

            foreach (var r in queryResults)
            {
                //metricCounts.Add(r.Count);
                years.Add(r.Year.ToString());
            }

            string[] chartYears = years.Cast<string>().ToArray();
            object[] metric = rates.Cast<object>().ToArray();

            columnChart.SetXAxis(new XAxis()
            {
                Type = AxisTypes.Category,
                Title = new XAxisTitle() { Text = "Years", Style = "fontWeight: 'bold', fontSize: '17px'" },
                Categories = chartYears
            });

            columnChart.SetYAxis(new YAxis()
            {
                Title = new YAxisTitle()
                {
                    Text = "VetCourt",
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

                    Name = "VetCourt",
                    Data = new Data(metric)
                },

            }
            );

            return View("DisplayChart", columnChart);
        }

        public ActionResult DownloadData()
        {
            var residents = DB.Residents;

            //Big nasty looking query that selects all the data that gets spit out to the excel sheet
            var residentProgramType = DB.Residents.Include(p => p.ProgramEvents).ToList()
                .Select(r => new ReportingResidentViewModel
                {
                    ID = r.ResidentID,
                    FirstName = r.ClearFirstMidName,
                    LastName = r.ClearLastName,
                    Birthdate = r.ClearBirthdate.GetValueOrDefault(),
                    Age = r.Age.Computed(),
                    ServiceType = r.ServiceBranch,
                    InVetCourt = r.InVetCourt,
                    Gender = r.Gender,
                    Religion = r.Religion,
                    StateTerritory = r.StateTerritory,
                    Ethnicity = r.Ethnicity,
                    Note = r.Note,
                    ProgramTypeID = r.ProgramEvents.Select(t => t.ProgramTypeID.GetValueOrDefault()),
                }).GroupBy(r => r.ID);




            var myExport = new CsvExport();

            //Each pass of this foreach loop inserts all the data from a single resident into the excel sheet
            foreach (var r in residentProgramType)
            {
                //All the non-events related data
                myExport.AddRow();
                myExport["Last Name"] = r.First().LastName;
                myExport["First Name"] = r.First().FirstName;
                myExport["Birthdate"] = r.First().Birthdate;
                myExport["Age"] = r.First().Age;
                myExport["Gender"] = FSEnumHelper.GetDescription(r.First().Gender);
                myExport["Religion"] = FSEnumHelper.GetDescription(r.First().Religion);
                myExport["Ethnicity"] = FSEnumHelper.GetDescription(r.First().Ethnicity);
                myExport["State/Territory"] = r.First().StateTerritory.State;
                myExport["Region"] = r.First().StateTerritory.Region;
                myExport["Service Branch"] = FSEnumHelper.GetDescription(r.First().ServiceType);
                myExport["Vet Court"] = r.First().InVetCourt;
                myExport["Notes"] = r.First().Note;



                var eventids = r.SelectMany(i => i.ProgramTypeID).ToList();

                //This foreach is used to display all the events a resident is in
                foreach (var eid in eventids)
                {
                    int eventID = eid;

                    //Query to the database to grab the name of the program
                    var prgm = (from p in DB.ProgramTypes
                                where p.ProgramTypeID == eventID
                                select p.ProgramDescription).ToArray();

                    int testVar = 1;

                    //Breaks the loop if a resident is in no programs
                    if (prgm.Length < testVar)
                    {
                        break;
                    }

                    //Grabs the name of a matching program and adds it to the excel sheet with a '1' to indicate a resident is in it
                    string programName = prgm[0];


                    myExport[programName.ToString()] = "1";

                }

            }

            //The following lines are how the excel sheet is saved and opened when this action result is invoked
            string filepath = Server.MapPath(Url.Content("~/Content/CenterReport.csv"));

            myExport.ExportToFile(filepath);

            string filename = "~\\Content\\CenterReport.csv";

            return File(filename, "text/csv", "HistoricData.csv");
        }

        [HttpGet]
        public ActionResult campaigns()
        {
            var BarChartData = (from mc in DB.MilitaryCampaigns
                                select new
                                {
                                    MilitaryCampaign = mc.CampaignName,
                                    ResidentCount = mc.Residents.Count
                                }).ToList();

            var residentNonCombatCount = DB.Residents.ToList().Where(i => i.MilitaryCampaigns == null || i.MilitaryCampaigns.Count < 1).ToList().Count;
            BarChartData.Add(new
            {
                MilitaryCampaign = "Non-Combat",
                ResidentCount = residentNonCombatCount

            });

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
                BackgroundColor = new BackColorOrGradient(System.Drawing.Color.GhostWhite),
                Style = "fontWeight: 'bold', fontSize: '17px', crop: false, overflow: 'none'",
                BorderColor = System.Drawing.Color.LightBlue,
                BorderRadius = 0,
                BorderWidth = 2,
                Height = 800

            });

            columnChart.SetTitle(new Title()
            {
                Text = "Current Resident Campaign Data"
            });

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
            });

            return View("DisplayChart", columnChart);
        }
        [HttpGet]
        public ActionResult DaysInResidenceReport()
        {
            using (ReportService residencyReport = new ReportService())
            {
                var daysInResidence = residencyReport.GetResidencyData();

                return View(daysInResidence);
            };
        }

        [HttpGet]
        public ActionResult ResidentReferralsReport()
        {
            using (ReportService residentReferralsReport = new ReportService())
            {
                var report = residentReferralsReport.GetReferralReport();
                return View("ResidentReferralsReport", report);
            };
        }

        [HttpGet]
        public ActionResult CurrentResidentReport()
        {
            using (ReportService currentResidentReport = new ReportService()) 
            {
                return View("CurrentResidentReport", currentResidentReport.GetCurrentResidentReport());
            };
        }

        [HttpGet]
        public async Task<ActionResult> ResidentsByYearReport(string yearFilter)
        {
            ViewBag.YearFilter = yearFilter;
            using (ReportService residentsByYear = new ReportService())
            {
                return View("ResidentsByYearReport", await Task.Run(() => residentsByYear.ResidentsByYear(yearFilter)).ConfigureAwait(false));
            }
        }

        [HttpGet]
        public async Task<ActionResult> UpToYearResidentAgeReport(string yearFilter)
        {
            ViewBag.YearFilter = yearFilter;
            using (ReportService residentsByYear = new ReportService())
            {
                return View("UpToYearResidentAgeReport", await Task.Run(() => residentsByYear.UpToYearResidentAgeReport(yearFilter)).ConfigureAwait(false));
            }
        }

        [HttpGet]
        public async Task<ActionResult> ResidentsByAdmittanceTypeReport(int ProgramTypeID = 1)
        {
            ViewBag.ProgramTypeID = new SelectList(DB.ProgramTypes.Where(i => i.EventType == EnumEventType.ADMISSION).ToList(), "ProgramTypeID", "ProgramDescription");
            using (ReportService reportService = new ReportService())
            {
                return View(await Task.Run(() => reportService.ResidentsByTrackType(ProgramTypeID)).ConfigureAwait(false));
            }
        }

        [HttpGet]
        public async Task<ActionResult> ResidentsByDischargeTypeReport(int ProgramTypeID = 4)
        {
            ViewBag.ProgramTypeID = new SelectList(DB.ProgramTypes.Where(i => i.EventType == EnumEventType.DISCHARGE).ToList(), "ProgramTypeID", "ProgramDescription");
            using (ReportService reportService = new ReportService())
            {
                return View(await Task.Run(() => reportService.ResidentsByTrackType(ProgramTypeID)).ConfigureAwait(false));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DB.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}