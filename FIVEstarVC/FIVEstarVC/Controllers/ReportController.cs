using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FIVEstarVC.Models;

namespace FIVEstarVC.Controllers
{
    public class ReportController : Controller
    {
        private FiveStarModel db = new FiveStarModel();

        // GET: Report
        public ViewResult Index(string searchString)
        {
            var residents = from r in db.Residents
                           select r;

            if (!(String.IsNullOrEmpty(searchString) || searchString.Trim().Length == 0))
            {
                residents = residents.Where(r => r.LastName.Contains(searchString)
                                       || r.FirstName.Contains(searchString));
            }
            else
            {
                //return HttpNotFound();
            }

            return View(residents.ToList());
        }
        /*
        // GET: Report/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Resident resident = db.Residents.Find(id);
            if (resident == null)
            {
                return HttpNotFound();
            }
            return View(resident);
        } */
    }
}
