using FIVESTARVC.DAL;
using FIVESTARVC.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace FIVESTARVC.Controllers
{
    [Authorize(Roles = "RTS-Group")]
    public class RoomsController : Controller
    {
        private ResidentContext db = new ResidentContext();
               
        // GET: Rooms
        public ActionResult Index(string searchString)
        {
            var rooms = db.Rooms.ToList();
            ViewBag.CurrentFilter = searchString;
            

            if (!string.IsNullOrEmpty(searchString))
            {
                var date = DateTime.Parse(searchString);

                rooms = db.RoomLogs
                    .Include(t => t.Resident)
                    .ToList()
                    .Where(t => t.Resident.GetAdmitDate()?.ToShortDateString() == date.ToShortDateString())
                    .Select(t => t.Room)
                    .ToList();
            }

            return View(rooms);
        }

        // GET: Rooms/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var residents = db.RoomLogs
                .Include(t => t.Resident)
                .Include(t => t.Room)
                .Include(t => t.Event)
                .Where(i => i.Room.RoomNumber == id).ToList();

            ViewBag.room = residents.Select(i => i.RoomNumber).Where(i => i.HasValue).First();

            return View(residents);
        }

        // GET: Rooms/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Rooms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RoomNumber,WingName")] Room room)
        {
            if (ModelState.IsValid)
            {
                room.IsOccupied = false;
                if (db.Rooms.Where(rm => rm.RoomNumber == room.RoomNumber).Any())
                {
                    ModelState.AddModelError("RoomNumber", "The room already exists in the system. Choose a new room number.");
                    return View(room);

                }
                else
                {
                    db.Rooms.Add(room);
                    db.SaveChanges();
                    TempData["UserMessage"] = "A new room has been added.  ";

                    return RedirectToAction("Index");
                }
            }

            return View(room);
        }




        // GET: Rooms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = db.Rooms.Find(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            return View(room);
        }

        // POST: Rooms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RoomNumber,IsOccupied,WingName")] Room room)
        {
            if (ModelState.IsValid)
            {
                db.Entry(room).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(room);
        }

        // GET: Rooms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = db.Rooms.Find(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            return View(room);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Room room = db.Rooms.Find(id);

            if (!room.IsOccupied)
            {
                try
                {
                    var logs = db.RoomLogs.Where(s => s.Room.RoomNumber == id).ToList();

                    db.RoomLogs.RemoveRange(logs);
                    db.SaveChanges();

                    db.Rooms.Remove(room);
                    db.Entry(room).State = EntityState.Deleted;
                    db.SaveChanges();
                }
                catch (DataException dex)
                {
                    TempData["UserMessage"] = "Failed to remove the room from your center because it is in-use. " + dex.Message;
                    return RedirectToAction("Index", new { id = id, saveChangesError = true });
                }
            }
            TempData["UserMessage"] = "Removed room " + room.RoomNumber + " from your center.";

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
