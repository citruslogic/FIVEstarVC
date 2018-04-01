using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using FIVESTARVC.DAL;
using FIVESTARVC.Models;
using FIVESTARVC.ViewModels;
using DelegateDecompiler;
using System.Globalization;

namespace FIVESTARVC.Controllers
{
    public class ProgramEventsController : Controller
    {
        private ResidentContext db = new ResidentContext();

        // GET: ProgramEvents
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "lname_desc" : "";
            ViewBag.ProgramSortParm = sortOrder == "ProgramDescription" ? "ProgramDescription_desc" : "ProgramDescription";
            ViewBag.ProgramTypeID = new SelectList(db.ProgramTypes, "ProgramTypeID", "ProgramDescription");


            var programEvents = db.ProgramEvents.Include(p => p.ProgramType).Where(t => t.ProgramTypeID >= 8).Include(p => p.Resident).ToList();

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            if (!String.IsNullOrEmpty(searchString))
            {
                programEvents = programEvents.Where(r => CultureInfo.CurrentCulture.CompareInfo.IndexOf
                                   (r.Resident.ClearLastName, searchString, CompareOptions.IgnoreCase) >= 0
                                   || CultureInfo.CurrentCulture.CompareInfo.IndexOf
                                   (r.Resident.FirstMidName, searchString, CompareOptions.IgnoreCase) >= 0).ToList();
            }

            switch (sortOrder)
            {
                case "name_desc":
                    programEvents = programEvents.OrderByDescending(p => p.Resident.ClearLastName.Computed()).ToList();
                    break;
                case "ProgramDescription":
                    programEvents = programEvents.OrderBy(p => p.ProgramTypeID).ToList();
                    break;
                case "ProgramDescription_desc":
                    programEvents = programEvents.OrderByDescending(p => p.ProgramTypeID).ToList();
                    break;
                default:
                    programEvents = programEvents.OrderBy(p => p.ResidentID).ToList();
                    break;
            }

            int pageSize = 6;
            int pageNumber = (page ?? 1);

            return View(programEvents.ToPagedList(pageNumber, pageSize));
        }

        // GET: ProgramEvents/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProgramEvent programEvent = db.ProgramEvents.Find(id);
            if (programEvent == null)
            {
                return HttpNotFound();
            }
            return View(programEvent);
        }

        // GET: ProgramEvents/Create
        public ActionResult Create()
        {
            ViewBag.ProgramTypeID = new SelectList(db.ProgramTypes.Where(t => t.ProgramTypeID >= 8), "ProgramTypeID", "ProgramDescription");
            ViewBag.ResidentID = new SelectList(db.Residents, "ResidentID", "ClearLastName");
            return View();
        }

        // POST: ProgramEvents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CustomEvent programEvent)
        {
            if (ModelState.IsValid)
            {
                
                db.ProgramEvents.Add(new ProgramEvent {

                    ResidentID = programEvent.ResidentID,
                    ProgramTypeID = programEvent.ProgramTypeID,
                    ClearStartDate = programEvent.ClearStartDate,
                    Completed = programEvent.Completed

                });
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProgramTypeID = new SelectList(db.ProgramTypes.Where(t => t.ProgramTypeID >= 8), "ProgramTypeID", "ProgramDescription", programEvent.ProgramTypeID);
            ViewBag.ResidentID = new SelectList(db.Residents, "ResidentID", "ClearLastName", programEvent.ResidentID);
            return View(programEvent);
        }

        // GET: ProgramEvents/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProgramEvent programEvent = db.ProgramEvents.Find(id);
            if (programEvent == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProgramTypeID = new SelectList(db.ProgramTypes.Where(t => t.ProgramTypeID >= 8), "ProgramTypeID", "ProgramDescription", programEvent.ProgramTypeID);
            ViewBag.ResidentID = new SelectList(db.Residents, "ResidentID", "ClearLastName", programEvent.ResidentID);
            return View(programEvent);
        }

        // POST: ProgramEvents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var eventToUpdate = db.ProgramEvents
                .Include(c => c.Resident)
                 .Where(c => c.ProgramEventID == id)
                .Single();

            if (ModelState.IsValid)
            {
                if (TryUpdateModel(eventToUpdate, "",
               new string[] { "ProgramEventID", "ProgramTypeID", "ResidentID", "ClearStartDate", "Completed" }))
                {

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.ProgramTypeID = new SelectList(db.ProgramTypes.Where(t => t.ProgramTypeID >= 8), "ProgramTypeID", "ProgramDescription", db.ProgramTypes);
            ViewBag.ResidentID = new SelectList(db.Residents, "ResidentID", "ClearLastName", db.Residents);
            return View(eventToUpdate);
        }

        // GET: ProgramEvents/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProgramEvent programEvent = db.ProgramEvents.Find(id);
            if (programEvent == null)
            {
                return HttpNotFound();
            }
            return View(programEvent);
        }

        // POST: ProgramEvents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProgramEvent programEvent = db.ProgramEvents.Find(id);
            db.ProgramEvents.Remove(programEvent);
            db.SaveChanges();
            return RedirectToAction("Index");
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
