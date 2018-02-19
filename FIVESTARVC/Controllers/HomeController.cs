using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FIVESTARVC.Models;
using FIVESTARVC.DAL;

namespace FIVESTARVC.Controllers
{

    public class HomeController : Controller
    {
        private ResidentContext db = new ResidentContext();

        public ActionResult Index()
        {
            ViewBag.lastFiveResidents = db.Residents.OrderByDescending(r => r.ResidentID).Take(5);

            return View();
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