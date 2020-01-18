﻿using DelegateDecompiler;
using FIVESTARVC.DAL;
using FIVESTARVC.Models;
using FIVESTARVC.ViewModels;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FIVESTARVC.Controllers
{
    [Authorize]
    //[Authorize(Roles = "RTS-Group")]
    public class CenterOverviewController : Controller
    {
        readonly ResidentContext db = new ResidentContext();

        public static List<GenderGroup> GenerateGenderComposition(IEnumerable<Resident> residents)
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
        public async Task<ActionResult> GenderGroupBreakdown()
        {
            var demographicTable = new DemographicTableViewModel();
            List<GenderGroup> genders = new List<GenderGroup>();
            var residents = await db.Residents.AsNoTracking().ToListAsync().ConfigureAwait(false);
            var currentResidents = residents.Where(cur => cur.IsCurrent().Computed());

            genders = GenerateGenderComposition(currentResidents);

            demographicTable.IsCurrent = true;
            demographicTable.GenderComposition = genders;

            return PartialView("_GenderGroupBreakdown", demographicTable);
        }

        public async Task<ActionResult> CumulativeGenderGroupBreakdown()
        {
            var demographicTable = new DemographicTableViewModel();
            IEnumerable<Resident> residents = await db.Residents.Include(i => i.ProgramEvents).AsNoTracking().ToListAsync();

            List<GenderGroup> genders = GenerateGenderComposition(residents);

            demographicTable.GenderComposition = genders;

            return PartialView("_GenderGroupBreakdown", demographicTable);
        }

        public async Task<ActionResult> GetServiceDischargeCounts()
        {
            List<DischargeStatusGroups> serviceDischargeGroups = new List<DischargeStatusGroups>();
            IEnumerable<Resident> residents = await db.Residents.AsNoTracking().ToListAsync().ConfigureAwait(false);

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

        public static List<AgeGroups> GenerateAgeComposition(IEnumerable<Resident> residents, bool cumulative = false)
        {
            if (cumulative)
            {
                return new List<AgeGroups> {
                    new AgeGroups
                    {
                        AgeGroup = "19 - 29",
                        Count = residents.Where(r => !r.IsCurrent() && r.GetAgeAtRelease >= 19 && r.GetAgeAtRelease <= 29).Count() + residents.Where(r => r.IsCurrent() && r.Age >= 19 && r.Age <= 29).Count()

                    },

                    new AgeGroups
                    {
                        AgeGroup = "30 - 39",
                        Count = residents.Where(r => !r.IsCurrent() && r.GetAgeAtRelease >= 30 && r.GetAgeAtRelease <= 39).Count() + residents.Where(r => r.IsCurrent() && r.Age >= 30 && r.Age <= 39).Count()
                    },

                    new AgeGroups
                    {
                        AgeGroup = "40 - 49",
                        Count = residents.Where(r => !r.IsCurrent() && r.GetAgeAtRelease >= 40 && r.GetAgeAtRelease <= 49).Count() + residents.Where(r => r.IsCurrent() && r.Age >= 40 && r.Age <= 49).Count()
                    },

                    new AgeGroups
                    {
                        AgeGroup = "50 - 59",
                        Count = residents.Where(r => !r.IsCurrent() && r.GetAgeAtRelease >= 50 && r.GetAgeAtRelease <= 59).Count() + residents.Where(r => r.IsCurrent() && r.Age >= 50 && r.Age <= 59).Count()
                    },

                    new AgeGroups
                    {
                        AgeGroup = "60 - 69",
                        Count = residents.Where(r => !r.IsCurrent() && r.GetAgeAtRelease >= 60 && r.GetAgeAtRelease <= 69).Count() + residents.Where(r => r.IsCurrent() && r.Age >= 60 && r.Age <= 69).Count()
                    },

                    new AgeGroups
                    {
                        AgeGroup = "> 70",
                        Count = residents.Where(r => !r.IsCurrent() && r.GetAgeAtRelease > 70).Count() + residents.Where(r => r.IsCurrent() && r.Age > 70).Count()
                    }
                };
            }
            else
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

        }


        // GET: AgeGroupBreakdown
        public async Task<ActionResult> AgeGroupBreakdown()
        {
            var demographicTable = new DemographicTableViewModel();
            List<AgeGroups> ageGroups = new List<AgeGroups>();

            IEnumerable<Resident> residents = await db.Residents.AsNoTracking().ToListAsync().ConfigureAwait(false);
            IEnumerable<Resident> currentResidents = residents.Where(cur => cur.IsCurrent().Computed());

            ageGroups = GenerateAgeComposition(residents);

            ViewBag.Sum = ageGroups.Sum(group => group.Count);
            ViewBag.AverageAge = GetAverageAge(currentResidents.ToList(), true).ToString("n2", new CultureInfo("en-US"));

            demographicTable.AgeComposition = ageGroups;
            demographicTable.IsCurrent = true;
            return PartialView("_AgeGroupBreakdown", demographicTable);
        }

        public async Task<ActionResult> CumulativeAgeGroupBreakdown()
        {
            var demographicTable = new DemographicTableViewModel();
            List<AgeGroups> ageGroups = new List<AgeGroups>();

            IEnumerable<Resident> residents = await db.Residents.AsNoTracking().ToListAsync().ConfigureAwait(false);

            ageGroups = GenerateAgeComposition(residents, true);

            ViewBag.Sum = ageGroups.Sum(group => group.Count);
            ViewBag.AverageAge = GetAverageAge(residents.ToList(), false).ToString("n2", new CultureInfo("en-US"));

            demographicTable.IsCurrent = false;
            demographicTable.AgeComposition = ageGroups;

            return PartialView("_AgeGroupBreakdown", demographicTable);
        }
        // GET: GetAverageStay
        public async Task<ActionResult> GetAverageStay()
        {
            //Variables to find average length of stay
            int? total = 0;
            int numbCount = 0;

            var residents = await db.Residents.AsNoTracking().ToListAsync().ConfigureAwait(false);

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
                ViewBag.AvgStay = ((double)total / numbCount).ToString("n2", new CultureInfo("en-US"));

            }

            return PartialView("_AverageStay");
        }

        public static double GetAverageAge(List<Resident> residents, bool currentOnly)
        {
            if (residents != null)
            {
                if (currentOnly) 
                {
                    return residents.Average(dt => (DateTime.Now - dt.ClearBirthdate.GetValueOrDefault().Date).TotalDays) / 360;
                }
                else
                {
                    return residents.Average(dt => (!dt.IsCurrent() ? (dt.GetDischargeDate() - dt.ClearBirthdate.GetValueOrDefault().Date) : (DateTime.Now - dt.ClearBirthdate.GetValueOrDefault().Date)).GetValueOrDefault().TotalDays) / 360;
                }
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