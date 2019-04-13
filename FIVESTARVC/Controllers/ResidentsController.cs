using DelegateDecompiler;
using FIVESTARVC.DAL;
using FIVESTARVC.Models;
using FIVESTARVC.Services;
using FIVESTARVC.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace FIVESTARVC.Controllers
{
    //[Authorize]
    [Authorize(Roles = "RTS-Group")]
    public class ResidentsController : Controller
    {
        private ResidentContext db = new ResidentContext();
        private ResidentService residentService = new ResidentService();

        // GET: Residents
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.BranchSortParm = sortOrder == "ServiceBranch" ? "ServiceBranch_desc" : "ServiceBranch";
            ViewBag.ProgramTypeID = new SelectList(db.ProgramTypes, "ProgramTypeID", "ProgramDescription");

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var residents = residentService.GetIndex(searchString, currentFilter, sortOrder, page, db);

            int pageSize = 6;
            int pageNumber = (page ?? 1);
            return View(residents.ToPagedList(pageNumber, pageSize));
        }

        // GET: Residents/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var dateFirstAdmitted = db.ProgramEvents
                                            .Include(r => r.Resident).Where(r => r.ResidentID == id)
                                            .Include(t => t.ProgramType).Where(p => p.ProgramTypeID == 2 || p.ProgramTypeID == 1).ToList()
                                            .OrderBy(d => d.ClearStartDate.Computed())
                                            .Select(s => s.ClearStartDate.Computed())
                                            .FirstOrDefault().ToLongDateString();

            if (!dateFirstAdmitted.Contains("0001"))
            {
                ViewBag.DateFirstAdmitted = dateFirstAdmitted;
            } else
            {
                // draw from the re-admittance date, ask for a readmission if there are no admittance dates. 
                ViewBag.DateFirstAdmitted = db.ProgramEvents
                                            .Include(r => r.Resident).Where(r => r.ResidentID == id)
                                            .Include(t => t.ProgramType).Where(p => p.ProgramTypeID == 3).ToList()
                                            .OrderBy(d => d.ClearStartDate.Computed())
                                            .Select(s => s.ClearStartDate.Computed())
                                            .FirstOrDefault().ToLongDateString() + " (readmit)";
            }

            var resident = residentService.GetDetails(id, db);
            var assignedCampaigns = residentService.PopulateAssignedCampaignData(resident, db);

            if (resident == null)
            {
                return HttpNotFound();
            }

            if (resident.RoomNumber != null)
            {
                ViewBag.room = resident.RoomNumber;
            }
            else
            {
                ViewBag.room = "No Room Assigned";
            }

            ViewBag.Campaigns = assignedCampaigns;

            return View(resident);
        }

        // GET: Residents/Create
        public ActionResult Create()
        {
            ResidentIncomeModel residentIncomeModel = new ResidentIncomeModel();

            ViewBag.StateTerritoryID = new SelectList(db.States, "StateTerritoryID", "State", residentIncomeModel.StateTerritoryID);

            ViewBag.AdmissionType = new SelectList(db.ProgramTypes
                .Where(t => t.EventType == EnumEventType.ADMISSION), "ProgramTypeID", "ProgramDescription", 2);

            var rooms = new List<int?> { null };
            rooms.AddRange(db.Rooms.Where(rm => rm.IsOccupied == false).Select(i => (int?)i.RoomNumber).ToList());

            ViewBag.RoomNumber = new SelectList(rooms);

            var allMilitaryCampaigns = db.MilitaryCampaigns;
            var viewModel = new List<AssignedCampaignData>();

            foreach (var militaryCampaign in allMilitaryCampaigns)
            {
                viewModel.Add(new AssignedCampaignData
                {
                    MilitaryCampaignID = militaryCampaign.MilitaryCampaignID,
                    MilitaryCampaign = militaryCampaign.CampaignName,
                    Assigned = false
                });
            }

            ViewBag.Campaigns = viewModel;

            return View();
        }

        // POST: Residents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ResidentIncomeModel residentIncomeModel, string[] selectedCampaigns,
            int AdmissionType, int? RoomNumber)
        {
            TempData["Duplicate"] = null;
            ViewBag.AdmissionType = new SelectList(db.ProgramTypes
                .Where(t => t.EventType == EnumEventType.ADMISSION), "ProgramTypeID", "ProgramDescription", AdmissionType);

            var rooms = new List<int?> { null };
            rooms.AddRange(db.Rooms.Where(rm => rm.IsOccupied == false).Select(i => (int?)i.RoomNumber).ToList());

            ViewBag.RoomNumber = new SelectList(rooms);
            ViewBag.StateTerritoryID = new SelectList(db.States, "StateTerritoryID", "State", residentIncomeModel.StateTerritoryID);

            var allMilitaryCampaigns = db.MilitaryCampaigns;
            var viewModel = new List<AssignedCampaignData>();

            foreach (var militaryCampaign in allMilitaryCampaigns)
            {
                viewModel.Add(new AssignedCampaignData
                {
                    MilitaryCampaignID = militaryCampaign.MilitaryCampaignID,
                    MilitaryCampaign = militaryCampaign.CampaignName,
                    Assigned = false
                });
            }

            ViewBag.Campaigns = viewModel;

            Resident resident = new Resident
            {
                ClearFirstMidName = residentIncomeModel.FirstMidName,
                ClearLastName = residentIncomeModel.LastName,
                Gender = residentIncomeModel.Gender,
                Ethnicity = residentIncomeModel.Ethnicity,
                Religion = residentIncomeModel.Religion,
                ClearBirthdate = residentIncomeModel.Birthdate,
                ServiceBranch = residentIncomeModel.ServiceBranch,
                MilitaryDischarge = residentIncomeModel.DischargeStatus,
                InVetCourt = residentIncomeModel.InVetCourt,
                IsNoncombat = residentIncomeModel.IsNoncombat,
                RoomNumber = RoomNumber,
                StateTerritoryID = residentIncomeModel.StateTerritoryID,
                Note = residentIncomeModel.Note,
                MilitaryCampaigns = new List<MilitaryCampaign>(),
                ProgramEvents = new List<ProgramEvent>(),
                Benefit = new Benefit(),
            };


            Benefit benefit = new Benefit
            {
                DisabilityPercentage = residentIncomeModel.DisabilityPercentage,
                DisabilityAmount = residentIncomeModel.DisabilityAmount,
                SSI = residentIncomeModel.SSI,
                SSDI = residentIncomeModel.SSDI,
                FoodStamp = residentIncomeModel.FoodStamp,
                OtherDescription = residentIncomeModel.OtherDescription,
                Other = residentIncomeModel.Other,
                TotalBenefitAmount = residentIncomeModel.TotalBenefitAmount
            };

            try
            {
                if (ModelState.IsValid)
                {
                    db.Residents.Add(resident);

                    var admitEvent = new ProgramEvent
                    {
                        ProgramTypeID = AdmissionType,
                        ClearStartDate = residentIncomeModel.AdmitDate,

                    };

                    resident.ProgramEvents.Add(admitEvent);

                    Room room = db.Rooms.Find(resident.RoomNumber);

                    if (room != null)
                    {
                        if (room.IsOccupied == false)
                        {
                            room.IsOccupied = true;
                        }
                        else
                        {
                            throw new DataException("The room is already occupied and cannot be allocated.");
                        }

                        RoomLog log = db.RoomLogs.Add(new RoomLog
                        {
                            Resident = resident,
                            Room = resident.Room,
                            Event = new ProgramEvent
                            {
                                ProgramTypeID = 2181,
                                Resident = resident,
                                ClearEndDate = admitEvent.ClearStartDate,
                                ClearStartDate = admitEvent.ClearStartDate,
                                Completed = true
                            },
                            Comment = "Resident admitted."
                        });
                    }

                    db.Benefits.Add(benefit);
                    resident.Benefit = benefit;

                    db.SaveChanges();
                }
            }
            catch (DataException dex)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError(dex.Message, "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            if (TryUpdateModel(residentIncomeModel, "",
                 new string[] { "LastName", "FirstMidName", "Ethnicity", "StateTerritoryID", "Gender", "Religion", "ClearBirthdate", "ServiceBranch", "Note", "IsNoncombat", "InVetCourt",
                     "Benefit", "MilitaryCampaigns", "TotalBenefitAmount" }))
            {
                try
                {
                    UpdateResidentCampaigns(selectedCampaigns, resident);
                    db.SaveChanges();

                    TempData["UserMessage"] = residentIncomeModel.LastName + " has been admitted into your center.  ";

                    return RedirectToAction("Index");
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

            return View(residentIncomeModel);
        }

        public JsonResult CheckResident(ResidentIncomeModel resident)
        {

            /* Check for the possible pre-existence of the resident in the system. */
            if (db.Residents.Any(r => r.ClearFirstMidName.Contains(resident.FirstMidName)
                && r.ClearLastName.Contains(resident.LastName)
                && r.ClearBirthdate.Date == resident.Birthdate.Date
                && r.ServiceBranch == resident.ServiceBranch))
            {

                // Found a match.
                return Json(new
                {
                    Success = false,
                    Message = "The resident, " + resident.FirstMidName + " " + resident.LastName
                    + " may already exist in the system. Please review the list."
                }, JsonRequestBehavior.AllowGet);

            }

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        // GET: Residents/Edit/5
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Resident resident = db.Residents
            .Include(c => c.MilitaryCampaigns)
            .Include(b => b.Benefit)
            .Include(s => s.StateTerritory)
            .Where(c => c.ResidentID == id)
            .Single();


            ViewBag.Campaigns = residentService.PopulateAssignedCampaignData(resident, db);


            if (resident == null)
            {
                return HttpNotFound();
            }

            var rooms = new List<int?> { null };
            rooms.AddRange(db.Rooms.Where(rm => rm.IsOccupied == false || rm.RoomNumber == resident.RoomNumber).Select(i => (int?)i.RoomNumber));

            ViewBag.RoomNumber = new SelectList(rooms, resident.RoomNumber.GetValueOrDefault());
            ViewBag.StateTerritoryID = new SelectList(db.States, "StateTerritoryID", "State", resident.StateTerritoryID);


            return View(resident);
        }

        // POST: Residents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id, string[] selectedCampaigns, int? RoomNumber, bool? Readmit)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Resident residentToUpdate = db.Residents
                .Include(c => c.MilitaryCampaigns)
                .Include(b => b.Benefit)
                .Where(c => c.ResidentID == id)
                .Single();

            if (TryUpdateModel(residentToUpdate, "",
               new string[] { "ClearLastName", "ClearFirstMidName", "Gender", "Religion", "Ethnicity", "StateTerritoryID", "ClearBirthdate",
                   "ServiceBranch", "Note", "InVetCourt", "IsNoncombat", "Benefit", "MilitaryCampaigns", "TotalBenefitAmount" }))
            {
                try
                {
                    UpdateResidentCampaigns(selectedCampaigns, residentToUpdate);

                    if (Readmit.HasValue)
                    {
                        if (Readmit == true)
                        {
                            var ev = residentToUpdate.ProgramEvents
                                .LastOrDefault(i => i.ProgramType.EventType == EnumEventType.DISCHARGE);    // Emergency Discharge to be readmited.
                            ev.ClearEndDate = DateTime.Now.Date;
                            db.Entry(ev).State = EntityState.Modified;

                            var readmitEvent = new ProgramEvent
                            {
                                ProgramTypeID = 3,
                                ClearStartDate = DateTime.Now.Date
                            };

                            residentToUpdate.ProgramEvents.Add(readmitEvent);

                            // Reassign the readmitted resident the given room number.
                            Room room = db.Rooms.Find(RoomNumber);

                            room.IsOccupied = true;
                            residentToUpdate.RoomNumber = RoomNumber;

                            db.RoomLogs.Add(new RoomLog
                            {
                                Resident = residentToUpdate,
                                Room = room,
                                Event = new ProgramEvent
                                {
                                    ProgramTypeID = 2181,
                                    ClearStartDate = readmitEvent.ClearStartDate,
                                    ClearEndDate = readmitEvent.ClearEndDate,
                                    Completed = true
                                },
                                Comment = "Resident readmitted."
                            });
                        }
                    }

                    // For the case when a resident is currently in the center.
                    if (Readmit == null || Readmit == false)
                    {
                        Room room = db.Rooms.Find(RoomNumber);

                        if (residentToUpdate.Room != null)
                        {
                            var oldRoom = residentToUpdate.Room;
                            if (residentToUpdate.Room.RoomNumber != RoomNumber)
                            {
                                /* Resident is changing rooms, if they have one */
                                if (residentToUpdate.Room != null)
                                {
                                    residentToUpdate.Room.IsOccupied = false;
                                    residentToUpdate.RoomNumber = RoomNumber;
                                    oldRoom.ClearLastResident = residentToUpdate.Fullname;

                                    if (room != null)
                                    {
                                        room.IsOccupied = true;
                                        oldRoom.IsOccupied = false;

                                        db.RoomLogs.Add(new RoomLog
                                        {
                                            Resident = residentToUpdate,
                                            Room = room,
                                            Event = new ProgramEvent
                                            {
                                                Resident = residentToUpdate,
                                                ClearStartDate = DateTime.Now,
                                                ClearEndDate = DateTime.Now,
                                                ProgramTypeID = 2181,
                                                Completed = true
                                            },
                                            Comment = "Resident moved from room " + oldRoom.RoomNumber
                                        });
                                    }

                                }
                            }
                        }
                        else
                        {
                            // resident does not have a room.
                            residentToUpdate.RoomNumber = RoomNumber;

                            if (room != null)
                            {
                                room.IsOccupied = true;

                                db.RoomLogs.Add(new RoomLog
                                {
                                    Resident = residentToUpdate,
                                    Room = room,
                                    Event = new ProgramEvent
                                    {
                                        Resident = residentToUpdate,
                                        ClearEndDate = DateTime.Now,
                                        ClearStartDate = DateTime.Now,
                                        ProgramTypeID = 2181,
                                        Completed = true
                                    },
                                    Comment = "Resident moved into room " + room.RoomNumber
                                });
                            }
                            else
                            {
                                db.RoomLogs.Add(new RoomLog
                                {
                                    Resident = residentToUpdate,
                                    Room = null,
                                    Event = new ProgramEvent
                                    {
                                        Resident = residentToUpdate,
                                        ClearEndDate = DateTime.Now,
                                        ClearStartDate = DateTime.Now,
                                        ProgramTypeID = 2181,
                                        Completed = true
                                    },
                                    Comment = "Resident moved out of room."
                                });
                            }

                        }
                    }

                    db.SaveChanges();

                    TempData["UserMessage"] = residentToUpdate.ClearLastName + " has been updated.  ";

                    return RedirectToAction("Index");
                }
                catch (DataException dex)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError(dex.Message, "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            var rooms = new List<int?> { null };
            rooms.AddRange(db.Rooms.Where(rm => rm.IsOccupied == false).Select(i => (int?)i.RoomNumber).ToList());

            ViewBag.RoomNumber = new SelectList(rooms, residentToUpdate.RoomNumber);
            ViewBag.StateTerritoryID = new SelectList(db.States, "StateTerritoryID", "State", residentToUpdate.StateTerritoryID);

            ViewBag.Campaigns = residentService.PopulateAssignedCampaignData(residentToUpdate, db);

            return View(residentToUpdate);
        }



        private void UpdateResidentCampaigns(string[] selectedCampaigns, Resident residentToUpdate)
        {
            if (selectedCampaigns == null)
            {
                residentToUpdate.MilitaryCampaigns = new List<MilitaryCampaign>();
                return;
            }

            var selectedCampaignsHS = new HashSet<string>(selectedCampaigns);
            var residentCampaigns = new HashSet<int>
                (residentToUpdate.MilitaryCampaigns.Select(c => c.MilitaryCampaignID));

            foreach (var campaign in db.MilitaryCampaigns)
            {
                if (selectedCampaignsHS.Contains(campaign.MilitaryCampaignID.ToString()))
                {
                    if (!residentCampaigns.Contains(campaign.MilitaryCampaignID))
                    {
                        residentToUpdate.MilitaryCampaigns.Add(campaign);
                    }
                }
                else
                {
                    if (residentCampaigns.Contains(campaign.MilitaryCampaignID))
                    {
                        residentToUpdate.MilitaryCampaigns.Remove(campaign);
                    }
                }
            }
        }

        // GET: Residents/AddCampaign/5
        [HttpGet]
        public ActionResult AddCampaign()
        {
            MilitaryCampaign NewCampaign = new MilitaryCampaign();

            return PartialView(NewCampaign);
        }

        // POST: Residents/AddCampaign/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCampaign(MilitaryCampaign model)
        {
            if (ModelState.IsValid)
            {
                db.MilitaryCampaigns.Add(model);
                db.SaveChanges();
                TempData["UserMessage"] = "A new campaign has been added.  ";

                return RedirectToAction("Index");
            }
            else
            {
                // Failed
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return PartialView(model);
        }

        // GET: Residents/Discharge/5
        [HttpGet]
        public ActionResult Discharge(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Discharge failed. Try again, and if the problem persists see your system administrator.";
            }

            var residentToDischarge = db.Residents
                .Include(p => p.ProgramEvents)
                .Where(r => r.ResidentID == id)
                .Single();

            Room roomToRelease = db.Rooms.Find(residentToDischarge.RoomNumber);

            if (roomToRelease != null)
            {
                ViewBag.releaseRoom = roomToRelease.RoomNumber;
            }
            // The room may not be assigned. 


            if (residentToDischarge == null)
            {
                return HttpNotFound();
            }

            ViewBag.ProgramTypeID = new SelectList(db.ProgramTypes
                .Where(t => t.EventType == EnumEventType.DISCHARGE), "ProgramTypeID", "ProgramDescription");

            return PartialView("_Discharge", residentToDischarge);
        }

        // POST: Residents/Discharge/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Discharge(int id, int ProgramTypeID, string DischargeDate)
        {
            DateTime date = DateTime.Parse(DischargeDate);

            try
            {
                Resident residentToDischarge = db.Residents
                .Include(p => p.ProgramEvents)
                .Where(r => r.ResidentID == id)
                .Single();

                Room roomToRelease = db.Rooms.Find(residentToDischarge.RoomNumber);

                // It is possible for the resident to not be assigned a room at this point.
                if (roomToRelease != null)
                {
                    residentToDischarge.RoomNumber = null;
                    roomToRelease.IsOccupied = false;
                    roomToRelease.ClearLastResident = residentToDischarge.Fullname;
                }

                // Discharge event
                residentToDischarge.ProgramEvents.Add(new ProgramEvent
                {
                    ProgramTypeID = ProgramTypeID,
                    ClearStartDate = date

                });

                // To close admission events
                var ev = db.ProgramEvents
                    .Where(i => i.ResidentID == id && i.ProgramType.EventType == EnumEventType.ADMISSION)
                    .ToList()
                    .LastOrDefault();
                ev.ClearEndDate = date;
                db.Entry(ev).State = EntityState.Modified;

                db.RoomLogs.Add(new RoomLog
                {
                    Resident = residentToDischarge,
                    Room = roomToRelease,
                    Event = new ProgramEvent
                    {
                        Resident = residentToDischarge,
                        ClearStartDate = date,
                        ClearEndDate = date,
                        ProgramTypeID = 2181,
                        Completed = true
                    },
                    Comment = "Resident discharged; room released."
                });

                if (residentToDischarge.ActualDaysStayed == null)
                {
                    residentToDischarge.ActualDaysStayed = residentToDischarge.DaysInCenter;
                }
                else
                {
                    residentToDischarge.ActualDaysStayed += residentToDischarge.DaysInCenter;
                }


                db.SaveChanges();
                TempData["UserMessage"] = residentToDischarge.ClearLastName + " has been discharged from your center.  ";
            }
            catch (DataException /*dex*/)
            {
                TempData["UserMessage"] = "Failed to discharge the resident from the center.";

                return RedirectToAction("Discharge", new { id = id, saveChangesError = true });
            }

            ViewBag.ProgramTypeID = new SelectList(db.ProgramTypes
                .Where(t => t.EventType == EnumEventType.DISCHARGE), "ProgramTypeID", "ProgramDescription");

            return RedirectToAction("Index");
        }

        // GET: Residents/Readmit/5
        [HttpGet]
        public ActionResult Readmit(int? id, int? RoomNumber, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Discharge failed. Try again, and if the problem persists see your system administrator.";
            }

            var residentToReadmit = db.Residents
                .Include(p => p.ProgramEvents)
                .Where(r => r.ResidentID == id)
                .Single();

            if (residentToReadmit == null)
            {
                return HttpNotFound();
            }

            var rooms = new List<int?> { null };
            rooms.AddRange(db.Rooms.Where(rm => rm.IsOccupied == false).Select(i => (int?)i.RoomNumber).ToList());
            ViewBag.RoomNumber = new SelectList(rooms);

            return PartialView("_Readmit", residentToReadmit);
        }

        // POST: Residents/Readmit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Readmit(int id, string ReadmitDate, int? RoomNumber)
        {
            var date = DateTime.Parse(ReadmitDate);

            var rooms = new List<int?> { null };
            rooms.AddRange(db.Rooms.Where(rm => rm.IsOccupied == false).Select(i => (int?)i.RoomNumber).ToList());
            ViewBag.RoomNumber = new SelectList(rooms);

            try
            {
                Resident residentToReadmit = db.Residents
                .Include(p => p.ProgramEvents)
                .Where(r => r.ResidentID == id)
                .Single();

                // To close discharge events
                var ev = residentToReadmit.ProgramEvents
                    .Where(i => i.ProgramType?.EventType == EnumEventType.DISCHARGE)
                    .LastOrDefault();
                if (ev != null)
                {
                    ev.ClearEndDate = DateTime.Parse(ReadmitDate);
                    db.Entry(ev).State = EntityState.Modified;
                }

                var admitEvent = new ProgramEvent
                {
                    ProgramTypeID = 3,
                    ClearStartDate = date
                };

                residentToReadmit.ProgramEvents.Add(admitEvent);

                residentToReadmit.RoomNumber = RoomNumber;
                Room room = db.Rooms.Find(RoomNumber);

                if (room != null)
                {
                    room.IsOccupied = true;

                    db.Entry(room).State = EntityState.Modified;

                    db.RoomLogs.Add(new RoomLog
                    {
                        Resident = residentToReadmit,
                        Room = room,
                        Event = new ProgramEvent
                        {
                            Resident = residentToReadmit,
                            ClearStartDate = date,
                            ClearEndDate = date,
                            ProgramTypeID = 2181,
                            Completed = true
                        },
                        Comment = "Resident readmitted."
                    });
                }

                db.SaveChanges();

                TempData["UserMessage"] = residentToReadmit.ClearLastName + " has been readmitted from your center.  ";
            }
            catch (DataException/* dex */)
            {
                TempData["UserMessage"] = "Failed to readmit the resident to the center.";

                return RedirectToAction("Readmit", new { id = id, saveChangesError = true });
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult GetRegionName(string id)
        {
            var RegionName = db.States.Find(Int32.Parse(id)).Region;

            return Json(RegionName, JsonRequestBehavior.AllowGet);
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
