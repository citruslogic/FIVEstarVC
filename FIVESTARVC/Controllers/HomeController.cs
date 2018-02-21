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
                     join room in db.Rooms on resident.RoomID equals room.RoomID
                     where room.RoomID == resident.RoomID
                     select new DashboardData
                     {
                         ResidentID = resident.ResidentID,
                         LastName = resident.LastName,
                         FirstMidName = resident.FirstMidName,
                         RoomNumber = room.RoomNum

                     }).OrderByDescending(r => r.ResidentID).Take(5);



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