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

namespace FIVESTARVC.Controllers
{
    [Authorize]
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

            var residents = from s in db.Residents
                            select s;




            if (!String.IsNullOrEmpty(searchString))
            {
                    residents = residents.Where(s => s.LastName.Contains(searchString)
                                       || s.FirstMidName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    residents = residents.OrderBy(s => s.LastName);
                    break;
                case "ServiceBranch":
                    residents = residents.OrderBy(s => s.ServiceBranch);
                    break;
                case "ServiceBranch_desc":
                    residents = residents.OrderByDescending(s => s.ServiceBranch);
                    break;
                default:
                    residents = residents.OrderByDescending(s => s.ID);
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
                .Where(r => r.ID == id).Single();

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
                                            .Include(t => t.ProgramType).Where(p => p.ProgramTypeID == 2)
                                            .OrderBy(d => d.StartDate)
                                            .Select(s => s.StartDate)
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
                FirstMidName = residentIncomeModel.FirstMidName,
                LastName = residentIncomeModel.LastName,
                Gender = residentIncomeModel.Gender,
                Ethnicity = residentIncomeModel.Ethnicity,
                Religion = residentIncomeModel.Religion,
                Birthdate = residentIncomeModel.Birthdate,
                ServiceBranch = residentIncomeModel.ServiceBranch,
                InVetCourt = residentIncomeModel.InVetCourt,
                RoomNumber = RoomNumber,
                StateTerritoryID = residentIncomeModel.StateTerritoryID,
                Note = residentIncomeModel.Note,
                MilitaryCampaigns = new List<MilitaryCampaign>(),
                ProgramEvents = new List<ProgramEvent>(),
                Benefit = new Benefit(),


            };

            try
            {
                if (ModelState.IsValid)
                {

                    
                    db.Residents.Add(resident);
                    resident.ProgramEvents.Add(new ProgramEvent
                    {
                        ProgramTypeID = AdmissionType,
                        StartDate = DateTime.Now,
                        EndDate = null

                    });

                    Room room = db.Rooms.Find(resident.RoomNumber);

                    if (room.IsOccupied == false)
                    {
                        room.IsOccupied = true;
                    }

                    db.SaveChanges();

                }


            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;

            }

                Benefit benefit = new Benefit
            {
                DisabilityPercentage = residentIncomeModel.DisabilityPercentage,
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
                 new string[] { "LastName", "FirstMidName", "Ethnicity", "StateTerritoryID", "Gender", "Religion", "Birthdate", "ServiceBranch", "Note", "InVetCourt",
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
            if (db.Residents.Any(r => r.FirstMidName.Contains(resident.FirstMidName)
                && r.LastName.Contains(resident.LastName)
                && r.Birthdate.GetValueOrDefault().Date == resident.Birthdate.GetValueOrDefault().Date
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
            .Where(c => c.ID == id)
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
            var residentToUpdate = db.Residents
                .Include(c => c.MilitaryCampaigns)
                .Include(b => b.Benefit)
                .Where(c => c.ID == id)
                .Single();

            if (TryUpdateModel(residentToUpdate, "",
               new string[] { "LastName", "FirstMidName", "Gender", "Religion", "Ethnicity", "StateTerritoryID", "Birthdate",
                   "ServiceBranch", "Note", "InVetCourt", "Benefit", "MilitaryCampaigns", "TotalBenefitAmount" }))
            {
                try
                {

                    UpdateResidentCampaigns(selectedCampaigns, residentToUpdate);

                    if (Readmit.HasValue)
                    {
                        if (Readmit == true)
                        {
                            residentToUpdate.ProgramEvents.Add(new ProgramEvent
                            {
                                ProgramTypeID = 3,
                                StartDate = DateTime.Now

                            });
                        }
                    }

                    if (RoomNumber.HasValue)
                    {
                        Room room = db.Rooms.Find(RoomNumber);

                        if (residentToUpdate.RoomNumber != RoomNumber)
                        {
                            /* Resident is changing rooms, if they have one */
                            if (residentToUpdate.Room != null)
                            {
                                residentToUpdate.Room.IsOccupied = false;
                            }
                            
                            residentToUpdate.RoomNumber = RoomNumber;
                            room.IsOccupied = true;
                        }

                    } 
                    db.SaveChanges();

                    TempData["UserMessage"] = residentToUpdate.LastName + " has been updated.  ";

                    return RedirectToAction("Index");
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

            ViewBag.Rooms = new SelectList(db.Rooms.Where(rm => rm.IsOccupied == false), "RoomNumber", "RoomNumber", residentToUpdate.RoomNumber);
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

            } else
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
                .Where(r => r.ID == id)
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
                .Where(t => t.ProgramTypeID < 8 && t.ProgramTypeID >= 4 ), "ProgramTypeID", "ProgramDescription");
           
            return PartialView("_Discharge", residentToDischarge);
        }

        // POST: Residents/Discharge/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Discharge(int id, int ProgramTypeID)
        {
            try
            {
                Resident residentToDischarge = db.Residents
                .Include(p => p.ProgramEvents)
                .Where(r => r.ID == id)
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
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now

                });

                db.SaveChanges();
                TempData["UserMessage"] = residentToDischarge.LastName + " has been discharged from your center.  ";
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


        // GET
        // Quick Event form (part of a modal dialog)
        [HttpGet]
        public ActionResult ViewQuickEvent(int id)
        {
            ViewBag.ResidentID = id;
            ViewBag.Fullname = db.Residents.Find(id).Fullname;
            ViewBag.ProgramTypeID = new SelectList(db.ProgramTypes
                .Where(t => t.ProgramTypeID >= 8), "ProgramTypeID", "ProgramDescription");

 
            return PartialView("_modalNewEvent");
        }

        /*
         * Save the Quick Event triggered on /Residents/Index
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewQuickEvent([Bind(Include = "ProgramEventID,ResidentID,ProgramTypeID,StartDate,EndDate,Completed")] int id, ProgramEvent programEvent)
        {
            if (ModelState.IsValid)
            {
                db.ProgramEvents.Add(programEvent);
                db.SaveChanges();
                TempData["UserMessage"] = db.Residents.Find(id).Fullname + " has a new event.  ";

            }

            ViewBag.ProgramTypeID = new SelectList(db.ProgramTypes
                .Where(t => t.ProgramTypeID >= 8), "ProgramTypeID", "ProgramDescription", programEvent.ProgramTypeID);
            ViewBag.ResidentID = id;

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
