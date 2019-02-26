using DelegateDecompiler;
using FIVESTARVC.DAL;
using FIVESTARVC.Models;
using FIVESTARVC.ViewModels;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace FIVESTARVC.Controllers
{
    [Authorize]
    //[Authorize(Roles = "RTS-Group")]
    public class CenterOverviewController : Controller
    {
        ResidentContext db = new ResidentContext();

        public List<GenderGroup> GenerateGenderComposition(IEnumerable<Resident> residents)
        {
            return new List<GenderGroup> {
                new GenderGroup
                {
                Gender = "Male",
                Count = residents.Where(r => r.Gender == GenderType.MALE).Count()

                },
                new GenderGroup
                {
                    Gender = "Female",
                    Count = residents.Where(r => r.Gender == GenderType.FEMALE).Count()
                },

                new GenderGroup {
                    Gender = "LGBT",
                    Count = residents.Where(r => r.Gender == GenderType.LGBT).Count()
                }
            };
        }
        // For the current residents.
        // GET: GenderGroupBreakdown
        public ActionResult GenderGroupBreakdown()
        {
            //var GenderGroups = db.Residents.ToList().Where(cur => cur.IsCurrent()).GroupBy(r => r.Gender).Select(group => new GenderGroup
            //{
            //    Gender = FSEnumHelper.GetDescription(group.Key),
            //    Count = group.Count()
            //});

            var demographicTable = new DemographicTableViewModel();
            List<GenderGroup> genders = new List<GenderGroup>();
            IEnumerable<Resident> residents = db.Residents.ToList().Where(cur => cur.IsCurrent());

            genders = GenerateGenderComposition(residents);

            demographicTable.IsCurrent = true;
            demographicTable.GenderComposition = genders;

            return PartialView("_GenderGroupBreakdown", demographicTable);
        }

        public ActionResult CumulativeGenderGroupBreakdown()
        {
            var demographicTable = new DemographicTableViewModel();
            List<GenderGroup> genders = new List<GenderGroup>();
            IEnumerable<Resident> residents = db.Residents.ToList();

            genders = GenerateGenderComposition(residents);

            demographicTable.GenderComposition = genders;

            return PartialView("_GenderGroupBreakdown", demographicTable);
        }

        public ActionResult GetServiceDischargeCounts()
        {
            List<DischargeStatusGroups> serviceDischargeGroups = new List<DischargeStatusGroups>();
            IEnumerable<Resident> residents = db.Residents.ToList();

            serviceDischargeGroups.Add(new DischargeStatusGroups
            {
                StatusName = "Honorable",
                Count = residents.Where(r => r.MilitaryDischarge == MilitaryDischargeType.HONORABLE).Count()
            });

            serviceDischargeGroups.Add(new DischargeStatusGroups
            {
                StatusName = "Dishonorable",
                Count = residents.Where(r => r.MilitaryDischarge == MilitaryDischargeType.DISHONORABLE).Count()
            });

            serviceDischargeGroups.Add(new DischargeStatusGroups
            {
                StatusName = "General Under Honorable",
                Count = residents.Where(r => r.MilitaryDischarge == MilitaryDischargeType.GENHONORABLE).Count()
            });

            serviceDischargeGroups.Add(new DischargeStatusGroups
            {
                StatusName = "Other",
                Count = residents.Where(r => r.MilitaryDischarge == MilitaryDischargeType.OTHER).Count()
            });

            return PartialView("_ServiceDischargeBreakdown", serviceDischargeGroups);

        }

        public List<AgeGroups> GenerateAgeComposition(IEnumerable<Resident> residents)
        {

            return new List<AgeGroups> {
                new AgeGroups
                {
                    AgeGroup = "19 - 29",
                    Count = residents.Where(r => r.Age >= 19 && r.Age <= 29).Count()
                },

                new AgeGroups
                {
                    AgeGroup = "30 - 39",
                    Count = residents.Where(r => r.Age >= 30 && r.Age <= 39).Count()
                },

                new AgeGroups
                {
                    AgeGroup = "40 - 49",
                    Count = residents.Where(r => r.Age >= 40 && r.Age <= 49).Count()
                },

                new AgeGroups
                {
                    AgeGroup = "50 - 59",
                    Count = residents.Where(r => r.Age >= 50 && r.Age <= 59).Count()
                },

                new AgeGroups
                {
                    AgeGroup = "60 - 69",
                    Count = residents.Where(r => r.Age >= 60 && r.Age <= 69).Count()
                },

                new AgeGroups
                {
                    AgeGroup = "> 70",
                    Count = residents.Where(r => r.Age > 70).Count()
                }
            };
        }

        // GET: AgeGroupBreakdown
        public ActionResult AgeGroupBreakdown()
        {
            // Not what was asked for.

            //var AgeGroups = db.Residents.ToList().Where(cur => cur.IsCurrent()).GroupBy(r => r.Age > 70 ? 5 : (r.Age - 18) / 10).Select(group => new AgeGroups
            //{ 
            //    AgeGroup = group.Key == 5 ? "> 70" : string.Format("{0} - {1}", group.Key * 10 + 18, group.Key * 10 + 29),
            //    Count = group.Count()
            //}).OrderByDescending(a => a.AgeGroup);

            var demographicTable = new DemographicTableViewModel();
            List<AgeGroups> ageGroups = new List<AgeGroups>();

            IEnumerable<Resident> residents = db.Residents.ToList().Where(cur => cur.IsCurrent());

            ageGroups = GenerateAgeComposition(residents);

            ViewBag.Sum = ageGroups.Sum(group => group.Count);
            ViewBag.AverageAge = GetCurrentAverageAge();

            demographicTable.AgeComposition = ageGroups;
            demographicTable.IsCurrent = true;
            return PartialView("_AgeGroupBreakdown", demographicTable);
        }

        public ActionResult CumulativeAgeGroupBreakdown()
        {
            // Not what was asked for.

            //var AgeGroups = db.Residents.ToList().Where(cur => cur.IsCurrent()).GroupBy(r => r.Age > 70 ? 5 : (r.Age - 18) / 10).Select(group => new AgeGroups
            //{ 
            //    AgeGroup = group.Key == 5 ? "> 70" : string.Format("{0} - {1}", group.Key * 10 + 18, group.Key * 10 + 29),
            //    Count = group.Count()
            //}).OrderByDescending(a => a.AgeGroup);

            var demographicTable = new DemographicTableViewModel();
            List<AgeGroups> ageGroups = new List<AgeGroups>();

            IEnumerable<Resident> residents = db.Residents.ToList();

            ageGroups = GenerateAgeComposition(residents);

            ViewBag.Sum = ageGroups.Sum(group => group.Count);
            ViewBag.AverageAge = GetCurrentAverageAge();

            demographicTable.IsCurrent = false;
            demographicTable.AgeComposition = ageGroups;

            return PartialView("_AgeGroupBreakdown", demographicTable);
        }
        // GET: GetAverageStay
        public ActionResult GetAverageStay()
        {
            //Variables to find average length of stay
            int? total = 0;
            int numbCount = 0;

            var residents = db.Residents.ToList();

            foreach (Resident resident in residents)
            {
                if (resident == null)
                {
                    continue;
                }

                numbCount++;
                total += resident.DaysInCenter;
            }

            /* Scenario is unlikely but possible. Thanks David! */
            if (numbCount == 0)
            {
                ViewBag.AvgStay = 0;

            }
            else
            {
                ViewBag.AvgStay = ((double)total / numbCount).ToString("n2");

            }

            return PartialView("_AverageStay");
        }

        public double GetCurrentAverageAge()
        {
            if (db.Residents.ToList().Where(cur => cur.IsCurrent()).Any())
            {
                IEnumerable<ReportingResidentViewModel> residentListing = db.Residents.ToList().Where(cur => cur.IsCurrent())
                    .Select(r => new ReportingResidentViewModel { ID = r.ResidentID, Age = r.Age.Computed() });

                return Math.Round(residentListing.Average(r => r.Age), 2);

            }
            else
            {
                return 0.0;
            }

        }

        [HttpPost]
        [ValidateInput(false)]
        public FileResult Export(string report)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                StringReader sr = new StringReader(report);
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();
                return File(stream.ToArray(), "application/pdf", "Report -" + DateTime.Today.ToShortDateString() + ".pdf");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}