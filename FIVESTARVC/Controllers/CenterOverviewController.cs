﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FIVESTARVC.Models;
using FIVESTARVC.DAL;
using FIVESTARVC.ViewModels;
using DelegateDecompiler;

namespace FIVESTARVC.Controllers
{
    public class CenterOverviewController : Controller
    {
        ResidentContext db = new ResidentContext();

        // For the current residents.
        // GET: GenderGroupBreakdown
        public ActionResult GenderGroupBreakdown()
        {
            //var GenderGroups = db.Residents.ToList().Where(cur => cur.IsCurrent()).GroupBy(r => r.Gender).Select(group => new GenderGroup
            //{
            //    Gender = FSEnumHelper.GetDescription(group.Key),
            //    Count = group.Count()
            //});

            List<GenderGroup> genders = new List<GenderGroup>();
            IEnumerable<Resident> residents = db.Residents.ToList().Where(cur => cur.IsCurrent());

            genders.Add(new GenderGroup
            {
                Gender = "Male",
                Count = residents.Where(r => r.Gender == GenderType.MALE).Count()
            });

            genders.Add(new GenderGroup
            {
                Gender = "Female",
                Count = residents.Where(r => r.Gender == GenderType.FEMALE).Count()
            });

            genders.Add(new GenderGroup
            {
                Gender = "LGBT",
                Count = residents.Where(r => r.Gender == GenderType.LGBT).Count()
            });

            return PartialView("_GenderGroupBreakdown", genders.ToList());
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

            List<AgeGroups> ageGroups = new List<AgeGroups>();

            IEnumerable<Resident> residents = db.Residents.ToList().Where(cur => cur.IsCurrent());

            ageGroups.Add(new AgeGroups
            {
                AgeGroup = "19 - 29",
                Count = residents.Where(r => r.Age >= 19 && r.Age <= 29).Count()
            });

            ageGroups.Add(new AgeGroups
            {
                AgeGroup = "30 - 39",
                Count = residents.Where(r => r.Age >= 30 && r.Age <= 39).Count()
            });

            ageGroups.Add(new AgeGroups
            {
                AgeGroup = "40 - 49",
                Count = residents.Where(r => r.Age >= 40 && r.Age <= 49).Count()
            });

            ageGroups.Add(new AgeGroups
            {
                AgeGroup = "50 - 59",
                Count = residents.Where(r => r.Age >= 50 && r.Age <= 59).Count()
            });

            ageGroups.Add(new AgeGroups
            {
                AgeGroup = "60 - 69",
                Count = residents.Where(r => r.Age >= 60 && r.Age <= 69).Count()
            });

            ageGroups.Add(new AgeGroups
            {
                AgeGroup = "> 70",
                Count = residents.Where(r => r.Age > 70).Count()
            });

            ViewBag.Sum = ageGroups.Sum(group => group.Count);
            ViewBag.AverageAge = GetCurrentAverageAge();
                       
            return PartialView("_AgeGroupBreakdown", ageGroups.ToList());
        }

        // GET: GetAverageStay
        public ActionResult GetAverageStay()
        {
            //Variables to find average length of stay
            int total = 0;
            int numbCount = 0;

            var residents = db.Residents.ToList();

            foreach (Resident resident in residents)
            {
                //if (resident.IsCurrent())
                //{
                //    continue;
                //}
                numbCount++;
                total += resident.DaysInCenter();
            }

            /* Scenario is unlikely but possible. Thanks David! */
            if (numbCount == 0)
            {
                ViewBag.AvgStay = 0;

            }
            else
            {
                ViewBag.AvgStay = total / numbCount;

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

            } else
            {
                return 0.0;
            }
               
        }

    }
}