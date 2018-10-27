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

namespace FIVESTARVC.Controllers
{
    //[Authorize(Roles = "RTS-Group")]
    [Authorize]
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


            //var programEvents = db.ProgramEvents.Include(p => p.ProgramType).Where(t => t.ProgramTypeID >= 8).Include(p => p.Resident).ToList();
            var programEvents = db.Residents.Include(p => p.ProgramEvents).ToList();
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
                                   (r.ClearLastName, searchString, CompareOptions.IgnoreCase) >= 0
                                   || CultureInfo.CurrentCulture.CompareInfo.IndexOf
                                   (r.ClearFirstMidName, searchString, CompareOptions.IgnoreCase) >= 0).ToList();
            }

            switch (sortOrder)
            {
                case "name_desc":
                    programEvents = programEvents.OrderByDescending(p => p.ClearLastName.Computed()).ToList();
                    break;
                case "ProgramDescription":
                    programEvents = programEvents.OrderBy(p => p.ProgramEvents).ToList();
                    break;
                case "ProgramDescription_desc":
                    programEvents = programEvents.OrderByDescending(p => p.ProgramEvents).ToList();
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

        /*
         * Append as many tracks as desired */
        // GET: ProgramEvents/AddMultiTrack
        public ActionResult AddMultiTrack()
        {
            ViewBag.ProgramTypeID = new SelectList(db.ProgramTypes.Where(t => t.EventType == EnumEventType.TRACK), "ProgramTypeID", "ProgramDescription");

            return PartialView("_ProgramTrack", new TempProgramEvent());
        }


        // GET: ProgramEvents/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CustomEvent model = new CustomEvent();
            model.programEvents = new List<TempProgramEvent>
            {
                new TempProgramEvent{ ResidentID = id.Value, StartDate = DateTime.Now },
            };

            ViewBag.ProgramTypeID = new SelectList(db.ProgramTypes.Where(t => t.EventType == EnumEventType.TRACK), "ProgramTypeID", "ProgramDescription");

            ViewBag.ResidentID = id;
            ViewBag.Fullname = db.Residents.Find(model.programEvents.First().ResidentID).Fullname;

            return View(model);
        }

        // POST: ProgramEvents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int ResidentID, CustomEvent model)
        {

            IEnumerable<TempProgramEvent> newPrograms = model.programEvents.Where(s => s.ProgramEventID == 0);

            foreach (TempProgramEvent track in newPrograms)
            {
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


            return RedirectToAction("Index", "Residents"); ;


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
