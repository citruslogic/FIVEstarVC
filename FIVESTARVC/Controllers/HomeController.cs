using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FIVESTARVC.Models;
using FIVESTARVC.DAL;
using FIVESTARVC.ViewModels;

namespace FIVESTARVC.Controllers
{

    public class HomeController : Controller
    {
        private ResidentContext db = new ResidentContext();

        public ActionResult Index()
        {

            var residents =
                    (from resident in db.Residents
                     join room in db.Rooms on resident.RoomNumber equals room.RoomNumber
                     select new DashboardData
                     {
                         ResidentID = resident.ResidentID,
                         Fullname = resident.FirstMidName + " " + resident.LastName,

                         RoomNumber = room.RoomNumber

                     }).OrderByDescending(r => r.ResidentID).Take(5);

            ViewBag.pop = db.Residents.ToList().Where(r => r.IsCurrent()).Count();
            ViewBag.allpop = db.Residents.ToList().Count();

            int roomsOccupied = db.Rooms.Where(rm => rm.IsOccupied == false).Count();
            ViewBag.roomsOccupied = roomsOccupied;

            int roomsMax = db.Rooms.Count();
            ViewBag.roomsMax = roomsMax;

            ViewBag.PercentFilled = ((double) roomsOccupied / (double) roomsMax) * 100;

            /* David Thompson's (dthompson) grad count */
            //Finds graduation percent
            double Graduated = db.ProgramEvents.Count(x => x.ProgramTypeID == 4);
            ViewBag.Graduated = Graduated;

            //Finds number admitted
            double Admitted = db.ProgramEvents.Count(x => x.ProgramTypeID == 2);
            ViewBag.Admitted = Admitted;

            ViewBag.gradPercent = ((double) Graduated / (double) Admitted) * 100;

            return View(residents.ToList());
        }

       
    }
}