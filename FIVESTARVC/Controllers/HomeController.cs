﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FIVESTARVC.Models;
using FIVESTARVC.DAL;
using FIVESTARVC.ViewModels;
using DelegateDecompiler;

namespace FIVESTARVC.Controllers
{

    public class HomeController : Controller
    {
        private ResidentContext db = new ResidentContext();
        public IEnumerable<Resident> NearestResidents { get; set; }           // Nearest birthdays.


        public ActionResult Index()
        {


            var residents = db.Residents.Include(r => r.Room).ToList().Select(data => new DashboardData
            {
                ResidentID = data.ResidentID,
                FirstMidName = data.FirstMidName,
                LastName = data.ClearLastName.Computed(),
                RoomNumber = data.RoomNumber.GetValueOrDefault()

            }).OrderByDescending(r => r.ResidentID).Take(5);

            ViewBag.pop = db.Residents.ToList().Where(r => r.IsCurrent()).Count();
            ViewBag.allpop = db.Residents.ToList().Count();

            int roomsOccupied = db.Rooms.Where(rm => rm.IsOccupied == false).Count();
            ViewBag.roomsOccupied = roomsOccupied;

            int roomsMax = db.Rooms.Count();
            ViewBag.roomsMax = roomsMax;

            ViewBag.PercentFilled = roomsOccupied / (double)roomsMax * 100;

            /* David Thompson's (dthompson) grad count */
            //Finds graduation percent
            int Graduated = db.ProgramEvents.Where(t => t.ProgramTypeID == 4).Count();
            ViewBag.Graduated = Graduated;

            //Finds number admitted
            int CurrentResidents = db.Residents.ToList().Count();
            ViewBag.Admitted = CurrentResidents;
            ViewBag.gradPercent = Graduated / (double)CurrentResidents * 100;


            /*******************************************/

            /* Get the resident with the nearest birthday */
            FindNearest();
            ViewBag.NearestResidents = NearestResidents;

            return View(residents);
        }

        /* Get the resident with the nearest birthday. */
        /* https://www.ict.social/csharp/wpf/course-birthday-reminder-in-csharp-net-wpf-logic-layer */
        private void FindNearest()
        {
            var sortedResidents = db.Residents.ToList().OrderBy(o => o.RemainingDays);

            if (sortedResidents.Count() > 0)
                NearestResidents = sortedResidents.Take(2);
            else
                NearestResidents = null;
        }





    }
}
 