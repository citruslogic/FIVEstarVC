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

        public static List<Room> MyRoom = new List<Room>();

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
            ViewBag.roomCap = db.Rooms.Where(rm => rm.IsOccupied == false).Count() + " / " + db.Rooms.Count();

            return View(residents.ToList());
        }

        public ActionResult Reports()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}