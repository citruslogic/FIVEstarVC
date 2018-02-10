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

namespace FIVESTARVC.Controllers
{
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
                    residents = residents.OrderByDescending(s => s.LastName);
                    break;
                case "ServiceBranch":
                    residents = residents.OrderBy(s => s.ServiceBranch);
                    break;
                case "ServiceBranch_desc":
                    residents = residents.OrderByDescending(s => s.ServiceBranch);
                    break;
                default:
                    residents = residents.OrderBy(s => s.LastName);
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

            /* Retrieve the first event where the veteran is admitted for the first time */
            ViewBag.DateFirstAdmitted = (from pgm in db.ProgramEvents
                                         join res in db.Residents on new { pgm.ResidentID } equals new { res.ResidentID }
                                         join pgmt in db.ProgramTypes on pgm.ProgramTypeID equals pgmt.ProgramTypeID
                                         where pgmt.ProgramTypeID.Equals(7)
                                         orderby pgm.ProgramEventID ascending
                                         select DbFunctions.TruncateTime(pgm.StartDate)).First().Value.ToShortDateString();

                                        
                                        
                                        


            return View(resident);
        }

        // GET: Residents/Create
        public ActionResult Create()
        {

            GetRoom();


            //Query database for IsOccupied flag//
            //Query for EastSouth Wing//
            //var availRoom = db.Rooms
            //                .Where(s => s.IsOccupied == false)
            //                .Select(r => new
            //                {
            //                    r.RoomNum,
            //                    r.WingName,
            //                });


            //ViewBag.rooms = new SelectList(availRoom, dataValueField: "RoomNum", dataTextField: "RoomNum",
            //                                   dataGroupField: "WingName", selectedValue: null);


            //var availRoom = from r in db.Rooms
            //            .Where(r => r.IsOccupied == false)
            //            .Include(r => r.RoomID)
            //            .Include(r => r.RoomNum)
            //            .OrderBy(r => r.WingName)
            //            .Select(r => r.RoomNum);

            //var roomToAssign = from y in db.Rooms
            //            .Where(y => y.IsOccupied == false)
            //            .Select(x => new ResidentIncomeModel { RoomNum = x.RoomNum, RoomID = x.RoomID, IsOccupied = x.IsOccupied })
            //            select y;






            return View(ViewBag.AssignRoom);

        }

        // POST: Residents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ResidentIncomeModel residentIncomeModel)
        {
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
                    db.SaveChanges();

                }


            }
            catch (DataException dex)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", dex.InnerException.Message);
            }

            Resident resident = new Resident
            {
                BenefitID = benefit.BenefitID,
                FirstMidName = residentIncomeModel.FirstMidName,
                LastName = residentIncomeModel.LastName,
                Birthdate = residentIncomeModel.Birthdate,
                ServiceBranch = residentIncomeModel.ServiceBranch,
                HasPTSD = residentIncomeModel.HasPTSD,
                InVetCourt = residentIncomeModel.InVetCourt,
                RoomID = residentIncomeModel.RoomID,
                Note = residentIncomeModel.Note
            };

            try
            {
                if (ModelState.IsValid)
                {
                    db.Residents.Add(resident);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }


            }
            catch (DataException dex)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", dex.InnerException.Message);
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
        public ActionResult EditPost(int? id, string[] selectedCampaigns)
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

            if (TryUpdateModel(residentToUpdate, "",
               new string[] { "LastName", "FirstMidName", "Birthdate", "ServiceBranch", "Note", "HasPTSD", "InVetCourt", "Benefit", "MilitaryCampaigns" }))
            {
                try
                {

                    UpdateResidentCampaigns(selectedCampaigns, residentToUpdate);
                    db.SaveChanges();

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



        // GET: Residents/Delete/5
        [HttpGet]
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            Resident resident = db.Residents.Find(id);

            if (resident == null)
            {
                return HttpNotFound();
            }
            return View(resident);
        }

        // POST: Residents/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                Resident resident = db.Residents.Find(id);
                Benefit benefit = db.Benefits.Find(resident.BenefitID);

                db.Benefits.Remove(benefit);
                db.Residents.Remove(resident);
                db.SaveChanges();
            }
            catch (DataException/* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            return RedirectToAction("Index");
        }

        // GET
        // Quick Event form (soon to be part of a modal dialog)
        public ActionResult ViewQuickEvent(int id, string lastname)
        {
            ViewBag.ResidentID = id;
            ViewBag.Lastname = lastname;
            ViewBag.ProgramTypeID = new SelectList(db.ProgramTypes, "ProgramTypeID", "ProgramDescription");

            return View("_modalNewEvent");
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

        public void GetRoom()
        {
            IQueryable<Room> AvailRoom =
            from AvailRooms in db.Rooms
            .Where(r => r.IsOccupied == false)
                 //.Include(r => r.RoomID)
                 .Include(r => r.RoomNum)
                 .OrderBy(r => r.WingName)
            select AvailRooms;



            //Initialize the AssignedRoom ViewModel//
            //AvailRoom = db.Rooms;



            var viewModel = new List<ResidentIncomeModel>();

            //Loop through the rooms and check to see if IsOccupied is checked or not//
            //if not checked, add it to the viewmodel//
            foreach (var Rooms in AvailRoom)
            {
                //if (Rooms.IsOccupied == false)
                //{
                viewModel.Add(new ResidentIncomeModel
                {
                    RoomNum = Rooms.RoomNum,
                    IsOccupied = Rooms.IsOccupied,
                    RoomID = Rooms.RoomID,
                    WingName = Rooms.WingName
                });
                //}

            }
            ViewBag.AssignRoom = viewModel;
        }

    }
}
