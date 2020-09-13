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
using System.Globalization;
using System.Web.Routing;
using System.Threading.Tasks;
using FIVESTARVC.Helpers;

namespace FIVESTARVC.Controllers
{
    [Authorize(Roles = "RTS-Group")]
    //[Authorize]
    public class ProgramEventsController : Controller
    {
        private readonly ResidentContext db = new ResidentContext();

        // GET: ProgramEvents
        [HttpGet]
        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "lname_desc" : "";
            ViewBag.ProgramTypeID = new SelectList(db.ProgramTypes, "ProgramTypeID", "ProgramDescription");

            var programEvents = await db.ProgramEvents.AsNoTracking().Include(p => p.Resident)
                .GroupBy(j => j.Resident)
                .ToListAsync().ConfigureAwait(false);
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
                                   (r.Key.ClearLastName, searchString, CompareOptions.IgnoreCase) >= 0
                                   || CultureInfo.CurrentCulture.CompareInfo.IndexOf
                                   (r.Key.ClearFirstMidName, searchString, CompareOptions.IgnoreCase) >= 0).ToList();
            }

            switch (sortOrder)
            {
                case "name_desc":
                    programEvents = programEvents.OrderByDescending(p => p.Key.ClearLastName).ToList();
                    break;

                default:
                    programEvents = programEvents.SelectMany(p => p.Key.ProgramEvents).OrderBy(i => i.ProgramType.EventType).ThenBy(i => i.ClearStartDate).GroupBy(i => i.Resident).ToList();
                    break;
            }

            int pageSize = 6;
            int pageNumber = (page ?? 1);

            return View(programEvents.ToPagedList(pageNumber, pageSize));
        }

        /*
         * Append as many tracks as desired */
        // GET: ProgramEvents/AddMultiTrack
        [HttpGet]
        public ActionResult AddMultiTrack()
        {
            ViewBag.ProgramTypeID = new SelectList(db.ProgramTypes.Where(t => t.EventType == EnumEventType.TRACK), "ProgramTypeID", "ProgramDescription");

            return PartialView("_ProgramTrack", new TempProgramEvent { ProgramTypeID = 8, StartDate = DateTime.Today });
        }

        [HttpGet]
        public ActionResult Manage(int? id, int? fromPage)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CustomEvent model = new CustomEvent
            {
                ProgramEvents = new List<TempProgramEvent>
            {
                new TempProgramEvent{ ResidentID = id.Value, StartDate = DateTime.Now },
            }
            };

            var admitDischargeGroup = new SelectListGroup { Name = "Admit / Discharge " };
            var otherTrackGroup = new SelectListGroup { Name = "Tracks to Success" };

            ViewBag.ProgramTypeID = new SelectList(db.ProgramTypes.Where(t => t.EventType != EnumEventType.SYSTEM).ToList().OrderBy(i => (int) i.EventType).Select(i => new SelectListItem
            {
                Text = i.ProgramDescription,
                Value = i.ProgramTypeID.ToString(),
                Group = (i.EventType == EnumEventType.ADMISSION || i.EventType == EnumEventType.DISCHARGE) && i.EventType != EnumEventType.SYSTEM ? admitDischargeGroup : otherTrackGroup
            }).OrderBy(i => i.Group.Name).ToList(), "Value", "Text", "Group.Name", -1);

            ViewBag.FromPage = fromPage.GetValueOrDefault(1);
            ViewBag.ResidentID = id;

            model.EnrolledTracks = db.ProgramEvents
                .Include(r => r.Resident)
                .Include(r => r.ProgramType)
                .Where(i => i.ResidentID == id && i.ProgramType.EventType != EnumEventType.SYSTEM)
                .ToList().Select(e => new TempProgramEvent
                {
                    ProgramEventID = e.ProgramEventID,
                    Resident = e.Resident,
                    StartDate = e.ClearStartDate,
                    EndDate = e.ClearEndDate,
                    Completed = e.Completed,
                    ProgramType = e.ProgramType

                }).OrderBy(i => i.ProgramEventID).ToList();

            ViewBag.Fullname = db.Residents.Find(model.ProgramEvents.First().ResidentID).Fullname;
            model.FromPage = fromPage.GetValueOrDefault(1);

            return View(model);
        }

        // POST: ProgramEvents/Manage
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(int ResidentID, CustomEvent model, int? fromPage = 1)
        {
            if (model != null)
            {
                IEnumerable<TempProgramEvent> newPrograms = model.ProgramEvents.Where(s => s.ProgramEventID == 0 && s.ProgramTypeID > 0);

                var idList = model.EnrolledTracks.Select(i => i.ProgramEventID).ToArray();
                var peList = db.ProgramEvents
                    .Join(idList, pe => pe.ProgramEventID, id => id, (pe, id) => pe)
                    .ToList();

                foreach (var track in model.EnrolledTracks)
                {
                    var pe = peList.Single(i => i.ProgramEventID == track.ProgramEventID);
                    pe.ClearEndDate = track.EndDate;
                    pe.Completed = track.Completed;
                    db.Entry(pe).State = EntityState.Modified;
                }

                foreach (TempProgramEvent track in newPrograms)
                {
                    if (track.StartDate.Year < 2012 || (track.EndDate.HasValue && track.EndDate.Value.Year < 2012))
                    {
                        TempData["UserMessage"] = "CRITICAL: Dates for " + track.StartDate.ToShortDateString() + " and " + track.EndDate?.ToShortDateString()
                                + " cannot be before 2012.";

                        return RedirectToAction("Manage", new RouteValueDictionary(
                            new { controller = "ProgramEvents", action = "Manage", Id = ResidentID, FromPage = fromPage }));
                    }

                    if (track.EndDate.HasValue && track.EndDate.Value < track.StartDate)
                    {
                        TempData["UserMessage"] = "CRITICAL: Dates for " + track.StartDate.ToShortDateString() + " and " + track.EndDate?.ToShortDateString()
                            + " -- start date cannot come after the end date.";

                        return RedirectToAction("Manage", new RouteValueDictionary(
                            new { controller = "ProgramEvents", action = "Manage", Id = ResidentID, FromPage = fromPage }));
                    }

                    if (ModelState.IsValid)
                    {
                        db.ProgramEvents.Add(new ProgramEvent
                        {

                            ResidentID = ResidentID,
                            ProgramTypeID = track.ProgramTypeID,
                            ClearStartDate = track.StartDate,
                            ClearEndDate = track.EndDate,
                            Completed = track.Completed
                        });
                    }

                    if (newPrograms.Count() > 1)
                    {
                        TempData["UserMessage"] = db.Residents.Find(ResidentID).Fullname + " has new tracks.  ";
                        db.SaveChanges();

                    }
                    else if (newPrograms.Count() == 1)
                    {
                        TempData["UserMessage"] = db.Residents.Find(ResidentID).Fullname + " has a new track.  ";
                        db.SaveChanges();

                    }
                    else
                    {
                        TempData["UserMessage"] = db.Residents.Find(ResidentID).Fullname + " was not enrolled in any new tracks. ";
                    }

                    var admitDischargeGroup = new SelectListGroup { Name = "Admit / Discharge " };
                    var otherTrackGroup = new SelectListGroup { Name = "Tracks to Success" };

                    ViewBag.ProgramTypeID = new SelectList(db.ProgramTypes.Where(t => t.EventType != EnumEventType.SYSTEM).ToList().OrderBy(i => (int)i.EventType).Select(i => new SelectListItem
                    {
                        Text = i.ProgramDescription,
                        Value = i.ProgramTypeID.ToString(),
                        Group = (i.EventType == EnumEventType.ADMISSION || i.EventType == EnumEventType.DISCHARGE) && i.EventType != EnumEventType.SYSTEM ? admitDischargeGroup : otherTrackGroup
                    }).OrderBy(i => i.Group.Name).ToList(), "Value", "Text", "Group.Name", -1); 
                }

                // We need to know the last track if the resident has been readmitted / discharged. As of now, position of admits/discharge tracks in list is not enforced.
                var resident = db.Residents.Find(ResidentID);
                foreach (var item in newPrograms)
                {
                    if (item != null)
                    {
                        if (item.ProgramTypeID == 1 || item.ProgramTypeID == 2 || item.ProgramTypeID == 3)
                        {
                            resident.IsCurrent = true;
                        }

                        if (item.ProgramTypeID == 4 || item.ProgramTypeID == 5 || item.ProgramTypeID == 6 || item.ProgramTypeID == 7 || item.ProgramTypeID == 13 || item.ProgramTypeID == 2187)
                        {
                            resident.IsCurrent = false;
                        }
                    }
                }
                db.SaveChanges();

            }

            TempData["UserMessage"] = "Track information updated. ";
            return RedirectToAction("Manage", new RouteValueDictionary(
                   new { controller = "ProgramEvents", action = "Manage", Id = ResidentID, FromPage = fromPage }));
        }
        
        // GET: ProgramEvents/Edit/5
        [HttpGet]
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
        [HttpGet]
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
            if (programEvent != null && programEvent.ResidentID > 0)
            {

                db.ProgramEvents.Remove(programEvent);

                db.SaveChanges();

                var events = db.Residents.FirstOrDefault(i => i.ResidentID == programEvent.ResidentID).ProgramEvents;

                foreach (var item in events)
                {
                    if (item != null && item.ProgramType != null)
                    {
                        if (item.ProgramType.EventType == EnumEventType.ADMISSION)
                        {
                            item.Resident.IsCurrent = true;
                        }

                        if (item.ProgramType.EventType == EnumEventType.DISCHARGE)
                        {
                            item.Resident.IsCurrent = false;
                        }
                    }
                }
                db.SaveChanges();
            }

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
