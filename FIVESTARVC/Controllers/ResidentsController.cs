using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using FIVESTARVC.DAL;
using FIVESTARVC.Models;
using PagedList;
using FIVESTARVC.ViewModels;
using System.Data.Entity.Validation;
using DelegateDecompiler;
using System.Globalization;

namespace FIVESTARVC.Controllers
{
    [Authorize]
    //[Authorize(Roles = "RTS-Group")]
    public class ResidentsController : Controller
    {
        private ResidentContext db = new ResidentContext();

        // GET: Residents
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
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

            var residents = (from s in db.Residents
                             select s).ToList();

            if (!String.IsNullOrEmpty(searchString))
            {
                residents = residents.Where(s => CultureInfo.CurrentCulture.CompareInfo.IndexOf
                                   (s.ClearLastName, searchString, CompareOptions.IgnoreCase) >= 0
                                   || CultureInfo.CurrentCulture.CompareInfo.IndexOf
                                   (s.ClearFirstMidName, searchString, CompareOptions.IgnoreCase) >= 0).ToList();
            }

            switch (sortOrder)
            {
                case "name_desc":
                    residents = residents.OrderBy(s => s.ClearLastName.Computed()).ToList();
                    break;
                case "ServiceBranch":
                    residents = residents.OrderBy(s => s.ServiceBranch).ToList();
                    break;
                case "ServiceBranch_desc":
                    residents = residents.OrderByDescending(s => s.ServiceBranch).ToList();
                    break;
                default:
                    residents = residents.OrderByDescending(s => s.ResidentID).ToList();
                    break;
            }

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
            Resident resident = db.Residents
                .Include(p => p.ProgramEvents)
                .Include(i => i.Benefit)
                .Where(r => r.ResidentID == id).Single();

            var room = db.Rooms.Find(resident.RoomNumber);

            if (resident.RoomNumber != null)
            {
                int roomNum = room.RoomNumber;

                ViewBag.room = roomNum;
            }

            else
            {
                ViewBag.room = "No Room Assigned";
            }

            if (resident == null)
            {
                return HttpNotFound();
            }

            PopulateAssignedCampaignData(resident);

            ViewBag.DateFirstAdmitted = db.ProgramEvents
                                            .Include(r => r.Resident).Where(r => r.ResidentID == id)
                                            .Include(t => t.ProgramType).Where(p => p.ProgramTypeID == 2).ToList()
                                            .OrderBy(d => d.ClearStartDate.Computed())
                                            .Select(s => s.ClearStartDate.Computed())
                                            .FirstOrDefault().ToLongDateString();

            return View(resident);
        }

        // GET: Residents/Create
        public ActionResult Create()
        {
            ResidentIncomeModel residentIncomeModel = new ResidentIncomeModel();

            ViewBag.StateTerritoryID = new SelectList(db.States, "StateTerritoryID", "State", residentIncomeModel.StateTerritoryID);

            ViewBag.AdmissionType = new SelectList(db.ProgramTypes
                .Where(t => t.ProgramTypeID <= 3), "ProgramTypeID", "ProgramDescription", 2);

            ViewBag.RoomNumber = new SelectList(db.Rooms.Where(rm => rm.IsOccupied == false), "RoomNumber", "RoomNumber");

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
            int AdmissionType, int RoomNumber)
        {
            TempData["Duplicate"] = null;
            ViewBag.AdmissionType = new SelectList(db.ProgramTypes
                .Where(t => t.ProgramTypeID <= 3), "ProgramTypeID", "ProgramDescription", AdmissionType);

            ViewBag.RoomNumber = new SelectList(db.Rooms.Where(rm => rm.IsOccupied == false), "RoomNumber", "RoomNumber");
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
                    resident.ProgramEvents.Add(new ProgramEvent
                    {
                        ProgramTypeID = AdmissionType,
                        ClearStartDate = residentIncomeModel.AdmitDate,

                    });

                    Room room = db.Rooms.Find(resident.RoomNumber);

                    if (room.IsOccupied == false)
                    {
                        room.IsOccupied = true;
                    }

                    db.Benefits.Add(benefit);
                    resident.Benefit = benefit;

                    db.SaveChanges();

                }


            }
            catch (DataException /*dex*/)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
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

        public ActionResult ConfirmCreate(Resident resident)
        {

            /* Check for the possible pre-existence of the resident in the system. */
            if (db.Residents.Any(r => r.ClearFirstMidName.Contains(resident.ClearFirstMidName)
                && r.ClearLastName.Contains(resident.ClearLastName)
                && r.ClearBirthdate.Date == resident.ClearBirthdate.Date
                && r.ServiceBranch == resident.ServiceBranch))
            {

                // Found a match.

            }

            return RedirectToAction("Create");
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


            PopulateAssignedCampaignData(resident);


            if (resident == null)
            {
                return HttpNotFound();
            }

            ViewBag.RoomNumber = new SelectList(db.Rooms.Where(rm => rm.IsOccupied == false
            || rm.RoomNumber == resident.RoomNumber), "RoomNumber", "RoomNumber", resident.RoomNumber.GetValueOrDefault().ToString());
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
                            var ev = residentToUpdate.ProgramEvents.LastOrDefault(i => i.ProgramType.ProgramTypeID == 4 || i.ProgramTypeID == 5 || i.ProgramTypeID == 6 || i.ProgramTypeID == 7);
                            ev.ClearEndDate = DateTime.Now;
                            db.Entry(ev).State = EntityState.Modified;


                            residentToUpdate.ProgramEvents.Add(new ProgramEvent
                            {
                                ProgramTypeID = 3,
                                ClearStartDate = DateTime.Now

                            });
                                       
                        }
                    }

                    if (RoomNumber.HasValue)
                    {
                        Room room = db.Rooms.Find(RoomNumber);

                        if (residentToUpdate.Room != null)
                        {
                            if (residentToUpdate.Room.RoomNumber != RoomNumber)
                            {
                                /* Resident is changing rooms, if they have one */
                                if (residentToUpdate.Room != null)
                                {
                                    residentToUpdate.Room.IsOccupied = false;
                                    residentToUpdate.RoomNumber = RoomNumber;

                                    room.IsOccupied = true;
                                }


                            }
                        } else
                        {
                            residentToUpdate.Room = room;
                        }


                    }

                    db.SaveChanges();

                    TempData["UserMessage"] = residentToUpdate.ClearLastName + " has been updated.  ";

                    return RedirectToAction("Index");
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

            ViewBag.RoomNumber = new SelectList(db.Rooms.Where(rm => rm.IsOccupied == false), "RoomNumber", "RoomNumber", residentToUpdate.RoomNumber);
            ViewBag.StateTerritoryID = new SelectList(db.States, "StateTerritoryID", "State", residentToUpdate.StateTerritoryID);

            PopulateAssignedCampaignData(residentToUpdate);

            return View(residentToUpdate);
        }

        private void PopulateAssignedCampaignData(Resident resident)
        {
            var allMilitaryCampaigns = db.MilitaryCampaigns;
            var residentCampaigns = new HashSet<int>(resident.MilitaryCampaigns.Select(c => c.MilitaryCampaignID));
            var viewModel = new List<AssignedCampaignData>();
            foreach (var militaryCampaign in allMilitaryCampaigns)
            {
                viewModel.Add(new AssignedCampaignData
                {
                    MilitaryCampaignID = militaryCampaign.MilitaryCampaignID,
                    MilitaryCampaign = militaryCampaign.CampaignName,
                    Assigned = residentCampaigns.Contains(militaryCampaign.MilitaryCampaignID)
                });
            }
            ViewBag.Campaigns = viewModel;
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
                .Where(t => t.ProgramTypeID < 8 && t.ProgramTypeID >= 4), "ProgramTypeID", "ProgramDescription");

            return PartialView("_Discharge", residentToDischarge);
        }

        // POST: Residents/Discharge/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Discharge(int id, int ProgramTypeID, string DischargeDate)
        {
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

                }

                residentToDischarge.ProgramEvents.Add(new ProgramEvent
                {
                    ProgramTypeID = ProgramTypeID,
                    ClearStartDate = DateTime.Parse(DischargeDate)

                });

                var ev = residentToDischarge.ProgramEvents.LastOrDefault(i => i.ProgramTypeID == 1 || i.ProgramTypeID == 2 || i.ProgramTypeID == 3);
                ev.ClearEndDate = DateTime.Parse(DischargeDate);
                db.Entry(ev).State = EntityState.Modified;

                db.SaveChanges();
                TempData["UserMessage"] = residentToDischarge.ClearLastName + " has been discharged from your center.  ";
            }
            catch (DataException/* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                TempData["UserMessage"] = "Failed to discharge the resident from the center.";

                return RedirectToAction("Discharge", new { id = id, saveChangesError = true });
            }

            ViewBag.ProgramTypeID = new SelectList(db.ProgramTypes
                .Where(t => t.ProgramTypeID >= 4 && t.ProgramTypeID <= 7), "ProgramTypeID", "ProgramDescription");


            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult GetRegionName(string id)
        {
            var RegionName = db.States.Find(Int32.Parse(id)).Region;

            return Json(RegionName, JsonRequestBehavior.AllowGet);
        }



    }
}
