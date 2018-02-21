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
using PagedList;
using FIVESTARVC.ViewModels;
using System.Data.Entity.Validation;

namespace FIVESTARVC.Controllers
{
    public class ResidentsController : Controller
    {
        private ResidentContext db = new ResidentContext();

        public static List<Models.Room> MyRoom = new List<Models.Room>();



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
                    residents = residents.OrderByDescending(s => s.LastName);
                    break;
                case "ServiceBranch":
                    residents = residents.OrderBy(s => s.ServiceBranch);
                    break;
                case "ServiceBranch_desc":
                    residents = residents.OrderByDescending(s => s.ServiceBranch);
                    break;
                default:
                    residents = residents.OrderByDescending(s => s.ResidentID);
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
                .Where(r => r.ResidentID == id).Single();

            if (resident == null)
            {
                return HttpNotFound();
            }

            PopulateAssignedCampaignData(resident);

            ViewBag.DateFirstAdmitted = db.ProgramEvents
                                            .Include(r => r.Resident).Where(r => r.ResidentID == id)
                                            .Include(t => t.ProgramType).Where(p => p.ProgramTypeID == 7)
                                            .OrderBy(d => d.StartDate)
                                            .Select(s => s.StartDate)
                                            .FirstOrDefault().ToLongDateString();

            return View(resident);
        }

        // GET: Residents/Create
        public ActionResult Create()
        {
            ResidentIncomeModel Rooms = new ResidentIncomeModel();

            MyRoom.Clear();
            Rooms.Rooms.Clear();

            //Query the database and store the rooms in roomToAssign
            var roomToAssign = from y in db.Rooms
                        .Where(y => y.IsOccupied == false)
                               select y;
            
            //Itterate the array and add it to Room MyRoom
            foreach (Models.Room y in roomToAssign)
            {
                MyRoom.Add(y);

            }

            //Itterate again and take it from Room.MyRoom to 
            //ResidentIncomeModel Rooms.Rooms
            
            foreach (var z in MyRoom)
            {
                if (z.WingName == "EastSouth")
                {
                    Rooms.Rooms.Add(z);
                }

                else if (z.WingName == "North")
                {
                    Rooms.Rooms.Add(z);
                }

                else
                {
                    Rooms.Rooms.Add(z);
                }

            }
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

            return View(Rooms);

        }



        // POST: Residents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ResidentIncomeModel residentIncomeModel, string[] selectedCampaigns)
        {
            //query rooms to retrieve the room object
            var AddRooms = from y in db.Rooms
                            .Where(y => y.RoomID == residentIncomeModel.RoomID)
                           select y;


            //explode the room object to get the items needed.
            foreach (var item in AddRooms)
            {

                residentIncomeModel.RoomID = item.RoomID;
                residentIncomeModel.RoomNum = item.RoomNum;
                residentIncomeModel.IsOccupied = item.IsOccupied = true;

            }

            //Save the room in the room table

            var roomToUpdate = db.Rooms.Find(residentIncomeModel.RoomID);

            if (TryUpdateModel(roomToUpdate))
            {
                roomToUpdate.IsOccupied = residentIncomeModel.IsOccupied;

                db.SaveChanges();
            }

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
                Birthdate = residentIncomeModel.Birthdate,
                ServiceBranch = residentIncomeModel.ServiceBranch,
                HasPTSD = residentIncomeModel.HasPTSD,
                InVetCourt = residentIncomeModel.InVetCourt,
                RoomID = residentIncomeModel.RoomID,
                Note = residentIncomeModel.Note,
                MilitaryCampaigns = new List<MilitaryCampaign>(),
                ProgramEvents = new List<ProgramEvent>(),
                Benefit = new Benefit()


            };

            try
            {
                if (ModelState.IsValid)
                {
                    db.Residents.Add(resident);
                    resident.ProgramEvents.Add(new ProgramEvent
                    {
                        ProgramTypeID = 7,
                        StartDate = DateTime.Now,
                        EndDate = null

                    });

                    db.SaveChanges();

                }


            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
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
                 new string[] { "LastName", "FirstMidName", "Birthdate", "ServiceBranch", "Note", "HasPTSD", "InVetCourt", "Benefit", "MilitaryCampaigns", "TotalBenefitAmount" }))
            {
                try
                {

                    UpdateResidentCampaigns(selectedCampaigns, resident);
                    db.SaveChanges();
                   
                    

                    TempData["UserMessage"] = residentIncomeModel.LastName + " has been admitted into your center.";

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
            .Where(c => c.ResidentID == id)
            .Single();


            PopulateAssignedCampaignData(resident);

            var roomToEdit = db.Rooms.Find(resident.RoomID);

            int roomToDisplay = roomToEdit.RoomNum;

            ViewBag.room = roomToDisplay;

            if (resident == null)
            {
                return HttpNotFound();
            }


            return View(resident);
        }



        // POST: Residents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id, string[] selectedCampaigns, string moveTo)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var residentToUpdate = db.Residents
                .Include(c => c.MilitaryCampaigns)
                .Include(b => b.Benefit)
                .Where(c => c.ResidentID == id)
                .Single();

            int newRoom = Convert.ToInt32(moveTo);

            var roomToChange = db.Rooms.Find(residentToUpdate.RoomID);

            roomToChange.IsOccupied = false;


            var changeRoom = from y in db.Rooms
                           .Where(y => y.RoomNum == newRoom)
                             select y;

            foreach (var z in changeRoom)
            {
                z.IsOccupied = true;
                residentToUpdate.RoomID = z.RoomID;

            }

            if (TryUpdateModel(residentToUpdate, "",
               new string[] { "LastName", "FirstMidName", "Birthdate", "ServiceBranch", "Note", "HasPTSD", "InVetCourt", "Benefit", "MilitaryCampaigns", "TotalBenefitAmount" }))
            {
                try
                {

                    UpdateResidentCampaigns(selectedCampaigns, residentToUpdate);
                    db.SaveChanges();

                    TempData["UserMessage"] = residentToUpdate.LastName + " has been updated.";

                    return RedirectToAction("Index");
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

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

            if (residentToDischarge == null)
            {
                return HttpNotFound();
            }

            ViewBag.ProgramTypeID = new SelectList(db.ProgramTypes.Where(t => t.ProgramTypeID >= 12 || t.ProgramTypeID == 2), "ProgramTypeID", "ProgramDescription");

            return View(residentToDischarge);
        }

        // POST: Residents/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Discharge(int id, int ProgramTypeID)
        {
            try
            {
                Resident residentToDischarge = db.Residents
                .Include(p => p.ProgramEvents)
                .Where(r => r.ResidentID == id)
                .Single();

                residentToDischarge.ProgramEvents.Add(new ProgramEvent
                {
                    ProgramTypeID = ProgramTypeID,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now

                });

                db.SaveChanges();
                TempData["UserMessage"] = residentToDischarge.LastName + " has been discharged from your center.";
            }
            catch (DataException/* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                TempData["UserMessage"] = "Failed to discharge the resident from the center.";

                return RedirectToAction("Discharge", new { id = id, saveChangesError = true });
            }

            ViewBag.ProgramTypeID = new SelectList(db.ProgramTypes.Where(t => t.ProgramTypeID >= 12), "ProgramTypeID", "ProgramDescription");

            
            return RedirectToAction("Index");
        }

        // GET: Residents/Delete/5
        //[HttpGet]
        //public ActionResult Delete(int? id, bool? saveChangesError = false)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    if (saveChangesError.GetValueOrDefault())
        //    {
        //        ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
        //    }
        //    Resident resident = db.Residents.Find(id);

        //    if (resident == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(resident);
        //}

        // POST: Residents/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id)
        //{
        //    try
        //    {
        //        Resident resident = db.Residents.Find(id);
        //        Benefit benefit = db.Benefits.Find(resident.BenefitID);

        //        if (benefit != null)
        //        {
        //            db.Benefits.Remove(benefit);
        //        } 

        //        db.Residents.Remove(resident);
        //        db.SaveChanges();
        //    }
        //    catch (DataException/* dex */)
        //    {
        //        //Log the error (uncomment dex variable name and add a line here to write a log.
        //        return RedirectToAction("Delete", new { id = id, saveChangesError = true });
        //    }
        //    return RedirectToAction("Index");
        //}

        // GET
        // Quick Event form (soon to be part of a modal dialog)
        public ActionResult ViewQuickEvent(int id, string lastname)
        {
            ViewBag.ResidentID = id;
            ViewBag.Lastname = lastname;
            ViewBag.ProgramTypeID = new SelectList(db.ProgramTypes, "ProgramTypeID", "ProgramDescription");

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
                return RedirectToAction("Index");
            }

            ViewBag.ProgramTypeID = new SelectList(db.ProgramTypes, "ProgramTypeID", "ProgramDescription", programEvent.ProgramTypeID);
            ViewBag.ResidentID = id;

            return View(programEvent);
        }

      

    }
}
