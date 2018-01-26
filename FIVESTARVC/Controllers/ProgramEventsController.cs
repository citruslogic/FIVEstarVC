using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FIVESTARVC.DAL;
using FIVESTARVC.Models;

namespace FIVESTARVC.Controllers
{
    public class ProgramEventsController : Controller
    {
        private ResidentContext db = new ResidentContext();

        // GET: ProgramEvents
        public ActionResult Index()
        {
            var programEvents = db.ProgramEvents.Include(p => p.ProgramType).Include(p => p.Resident);
            return View(programEvents.ToList());
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
            ViewBag.ProgramTypeID = new SelectList(db.ProgramTypes, "ProgramTypeID", "ProgramDescription");
            ViewBag.ResidentID = new SelectList(db.Residents, "ResidentID", "LastName");
            return View();
        }

        // POST: ProgramEvents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProgramEventID,ResidentID,ProgramTypeID,StartDate,EndDate,Notes,Completed")] ProgramEvent programEvent)
        {
            if (ModelState.IsValid)
            {
                db.ProgramEvents.Add(programEvent);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProgramTypeID = new SelectList(db.ProgramTypes, "ProgramTypeID", "ProgramDescription", programEvent.ProgramTypeID);
            ViewBag.ResidentID = new SelectList(db.Residents, "ResidentID", "LastName", programEvent.ResidentID);
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
            ViewBag.ProgramTypeID = new SelectList(db.ProgramTypes, "ProgramTypeID", "ProgramDescription", programEvent.ProgramTypeID);
            ViewBag.ResidentID = new SelectList(db.Residents, "ResidentID", "LastName", programEvent.ResidentID);
            return View(programEvent);
        }

        // POST: ProgramEvents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProgramEventID,ResidentID,ProgramTypeID,StartDate,EndDate,Notes,Completed")] ProgramEvent programEvent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(programEvent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProgramTypeID = new SelectList(db.ProgramTypes, "ProgramTypeID", "ProgramDescription", programEvent.ProgramTypeID);
            ViewBag.ResidentID = new SelectList(db.Residents, "ResidentID", "LastName", programEvent.ResidentID);
            return View(programEvent);
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
