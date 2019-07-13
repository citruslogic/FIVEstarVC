using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList;
using FIVESTARVC.DAL;
using FIVESTARVC.Models;
using FIVESTARVC.ViewModels;
using DelegateDecompiler;
using System.Globalization;
using System.Web.Routing;

namespace FIVESTARVC.Controllers
{
    [Authorize(Roles = "RTS-Group")]
    //[Authorize]
    public class ProgramEventsController : Controller
    {
        private readonly ResidentContext db = new ResidentContext();

        // GET: ProgramEvents
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "lname_desc" : "";
            ViewBag.ProgramTypeID = new SelectList(db.ProgramTypes, "ProgramTypeID", "ProgramDescription");

            var programEvents = db.Residents.AsNoTracking().Include(p => p.ProgramEvents).Where(i => i.ProgramEvents.Count > 0).ToList().OrderBy(i => i.ClearLastName).ToList();
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            if (!string.IsNullOrEmpty(searchString))
            {
                programEvents = programEvents.Where(r => CultureInfo.CurrentCulture.CompareInfo.IndexOf
                                   (r.ClearLastName, searchString, CompareOptions.IgnoreCase) >= 0
                                   || CultureInfo.CurrentCulture.CompareInfo.IndexOf
                                   (r.ClearFirstMidName, searchString, CompareOptions.IgnoreCase) >= 0).ToList();
            }

            switch (sortOrder)
            {
                case "name_desc":
                    programEvents = programEvents.OrderByDescending(p => p.ClearLastName.Computed()).ToList();
                    break;

                default: programEvents = programEvents.OrderBy(p => p.ResidentID).ToList();
                    break;
            }

            int pageSize = 6;
            int pageNumber = (page ?? 1);

            return View(programEvents.ToPagedList(pageNumber, pageSize));
        }

        /*
         * Append as many tracks as desired */
        // GET: ProgramEvents/AddMultiTrack
        public ActionResult AddMultiTrack()
        {
            ViewBag.ProgramTypeID = new SelectList(db.ProgramTypes.Where(t => t.EventType == EnumEventType.TRACK), "ProgramTypeID", "ProgramDescription");

            return PartialView("_ProgramTrack", new TempProgramEvent { ProgramTypeID = 8, StartDate = DateTime.Today });
        }


        // GET: ProgramEvents/Create
        public ActionResult Manage(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CustomEvent model = new CustomEvent();
            model.ProgramEvents = new List<TempProgramEvent>
            {
                new TempProgramEvent{ ResidentID = id.Value, StartDate = DateTime.Now },
            };

            ViewBag.ProgramTypeID = new SelectList(db.ProgramTypes.Where(t => t.EventType == EnumEventType.TRACK), "ProgramTypeID", "ProgramDescription");

            ViewBag.ResidentID = id;
            model.EnrolledTracks = db.ProgramEvents
                .Include(r => r.Resident)
                .Include(r => r.ProgramType)
                .Where(i => i.ResidentID == id && i.ProgramType.EventType == EnumEventType.TRACK)
                .ToList().Select(e => new TempProgramEvent
                {
                    ProgramEventID = e.ProgramEventID,
                    Resident = e.Resident,
                    StartDate = e.ClearStartDate,
                    EndDate = e.ClearEndDate,
                    Completed = e.Completed,
                    ProgramType = e.ProgramType

                }).ToList();
            ViewBag.Fullname = db.Residents.Find(model.ProgramEvents.First().ResidentID).Fullname;

            return View(model);
        }

        // POST: ProgramEvents/Manage
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(int ResidentID, CustomEvent model)
        {

            IEnumerable<TempProgramEvent> newPrograms = model.ProgramEvents.Where(s => s.ProgramEventID == 0 && s.ProgramTypeID > 0);

            foreach (TempProgramEvent track in newPrograms)
            {
                if (track.StartDate.Year < 2012 || (track.EndDate.HasValue && track.EndDate.Value.Year < 2012))
                {
                    TempData["UserMessage"] = "CRITICAL: Dates for " + track.StartDate.ToShortDateString() + " and " + track.EndDate?.ToShortDateString() 
                            + " cannot be before 2012.";

                    return RedirectToAction("Manage", new RouteValueDictionary(
                        new { controller = "ProgramEvents", action = "Manage", Id = ResidentID }));
                }

                if (track.EndDate.HasValue && track.EndDate.Value < track.StartDate)
                {
                    TempData["UserMessage"] = "CRITICAL: Dates for " + track.StartDate.ToShortDateString() + " and " + track.EndDate?.ToShortDateString()
                        + " -- start date cannot come after the end date.";

                    return RedirectToAction("Manage", new RouteValueDictionary(
                        new { controller = "ProgramEvents", action = "Manage", Id = ResidentID }));
                }

                if (ModelState.IsValid)
                {
                    db.ProgramEvents.Add(new ProgramEvent {

                        ResidentID = ResidentID,
                        ProgramTypeID = track.ProgramTypeID,
                        ClearStartDate = track.StartDate,
                        ClearEndDate = track.EndDate,
                        Completed = track.Completed
                    });
                }
                
            }

            db.SaveChanges();
            if (newPrograms.Count() > 1)
            {
                TempData["UserMessage"] = db.Residents.Find(ResidentID).Fullname + " has new events.  ";
            } else
            {
                TempData["UserMessage"] = db.Residents.Find(ResidentID).Fullname + " has a new event.  ";
            }

            ViewBag.ProgramTypeID = new SelectList(db.ProgramTypes.Where(t => t.EventType == EnumEventType.TRACK), "ProgramTypeID", "ProgramDescription");

            return RedirectToAction("Manage", new RouteValueDictionary(
                   new { controller = "ProgramEvents", action = "Manage", Id = ResidentID }));
        }

        [HttpPost]
        public ActionResult ReleaseTrack(int ResidentID, CustomEvent customEvent)
        {
            if (customEvent != null && customEvent.EnrolledTracks?.Count > 0)
            {
                try
                {
                    var idList = customEvent.EnrolledTracks.Select(i => i.ProgramEventID).ToArray();
                    var peList = db.ProgramEvents
                        .Join(idList, pe => pe.ProgramEventID, id => id, (pe, id) => pe)
                        .ToList();

                    foreach (var track in customEvent.EnrolledTracks)
                    {
                        var pe = peList.Single(i => i.ProgramEventID == track.ProgramEventID);
                        pe.ClearEndDate = track.EndDate;
                        pe.Completed = track.Completed;
                        db.Entry(pe).State = EntityState.Modified;
                    }

                    db.SaveChanges();
                    return RedirectToAction("Manage", new RouteValueDictionary(
                        new { controller = "ProgramEvents", action = "Manage", Id = ResidentID }));

                } catch (Exception e)
                {
                    return Json("Resident's tracks could not be updated. ERROR: " + e.Message);
                }
               
            }
            return Json("Resident's tracks could not be updated.");
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

            ViewBag.AdmissionType = programEvent.ProgramType.EventType.ToString();
            ViewBag.ProgramTypeID = new SelectList(db.ProgramTypes.Where(i => i.EventType == programEvent.ProgramType.EventType), "ProgramTypeID", "ProgramDescription", programEvent.ProgramTypeID);
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
               new string[] { "ProgramEventID", "ProgramTypeID", "ResidentID", "ClearStartDate", "ClearEndDate", "Completed" }))
                {

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.ProgramTypeID = new SelectList(db.ProgramTypes, "ProgramTypeID", "ProgramDescription", db.ProgramTypes);
            ViewBag.ResidentID = new SelectList(db.Residents, "ResidentID", "ClearLastName", db.Residents);
            return View(eventToUpdate);
        }

        [HttpGet]
        public ActionResult ViewQuickEvent()
        {
            return PartialView("ViewQuickEvent", new ProgramType());
        }

        /*
         * Save the Quick Event triggered on /Residents/Index
         * 
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewQuickEvent(ProgramType model)
        {
            ProgramType type = new ProgramType
            {
                EventType = EnumEventType.TRACK,
                ProgramDescription = model.ProgramDescription
            };
            
            if (ModelState.IsValid)
            {
                db.ProgramTypes.Add(type);
                db.SaveChanges();
                TempData["UserMessage"] = "A new track has been added.  ";

            }

            return RedirectToAction("Index", "Residents");

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
